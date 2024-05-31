using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Analytics;
using WireBuilder;

// ReSharper disable once CheckNamespace
namespace Misc.Items
{
    public class Rope : MonoBehaviour
    {
        #region Rope Properties

        [SerializeField, BoxGroup("Rope Properties")]
        private int ropeResolution;

        [SerializeField, BoxGroup("Rope Properties")]
        private float ropeLength;

        [SerializeField, BoxGroup("Rope Properties")]
        private float ropeThickness;

        [SerializeField, BoxGroup("Rope Properties")]
        private Material ropeMaterial;

        [SerializeField, BoxGroup("Rope Properties")]
        private int breakIndex;

        [Tooltip(
            "TIt determines how strongly the joint will try to maintain its position. A higher spring value will make the joint stiffer and more resistant to rotational movement")]
        [SerializeField, BoxGroup("Rope Properties")]
        private float spring;

        [Tooltip(
            "It determines how quickly the joint will come to rest after being moved. A higher damping value will make the joint slow down and stop more quickly after being moved")]
        [SerializeField, BoxGroup("Rope Properties")]
        private float damp;

        #endregion

        #region Player Movement

        [SerializeField, BoxGroup("Movement")] private float climbSpeed = 2f;
        [SerializeField, BoxGroup("Movement")] private float swingForce = 200;
        [SerializeField, BoxGroup("Movement")] private float entryForce = 10;
        [SerializeField, BoxGroup("Movement")] internal float exitForce = 3;

        #endregion

        #region Unexposed Variables

        [SerializeField] private Rigidbody[] ropeSegments;
        [SerializeField] private LineRenderer[] lineRenderers;
        private bool _connected;
        private float _closestIndex;
        private float _closestDistance = 100f;
        private float _lastAttachedTime;

        #endregion

        #region Editor Specific

        private Vector3 _giz;

        [Button("Create Rope", ButtonSizes.Large), GUIColor(1, 0.3f, 0.3f)]
        public void CreateRopeSegments()
        {
            //delete all the children of the current object
            foreach (var segment in ropeSegments)
            {
                DestroyImmediate(segment.gameObject);
            }


            ropeSegments = new Rigidbody[ropeResolution];

            for (int i = 0; i < ropeResolution; i++)
            {
                //instantiate gameObject with current object being the parent
                GameObject obj = new GameObject(i.ToString())
                {
                    transform =
                    {
                        parent = transform,
                        position = new Vector3(transform.position.x,
                            transform.position.y - i * (ropeLength) / (ropeResolution - 1), transform.position.z)
                    },
                    //Set object layer to Ignore Player
                    layer = LayerMask.NameToLayer("Ignore Player")
                };

                ropeSegments[i] = obj.AddComponent<Rigidbody>();


                if (i > 0)
                {
                    #region Hinge Joint

                    CapsuleCollider addedCollider = ropeSegments[i].gameObject.AddComponent<CapsuleCollider>();
                    addedCollider.center = new Vector3(0, ((ropeLength / (ropeResolution - 1)) - 0.1f) / 2, 0);
                    addedCollider.height = (ropeLength / (ropeResolution - 1)) - 0.1f;
                    addedCollider.radius = ropeThickness / 2;

                    HingeJoint joint = ropeSegments[i].gameObject.AddComponent<HingeJoint>();

                    joint.useSpring = true;
                    joint.spring = new JointSpring()
                    {
                        spring = spring,
                        damper = damp
                    };

                    joint.anchor = new Vector3(0, ropeLength / (ropeResolution - 1), 0);
                    joint.connectedBody = ropeSegments[i - 1].GetComponent<Rigidbody>();

                    #endregion
                }
                else
                {
                    ropeSegments[i].gameObject.AddComponent<FixedJoint>();
                }
            }
        }


        [Button("Create LineRenderer")]
        public void CreateLineRenderer()
        {

            lineRenderers = new LineRenderer[ropeResolution];
            
            for(int i=1; i<ropeResolution; i++)
            {
                if (!ropeSegments[i].TryGetComponent<LineRenderer>(out lineRenderers[i]))
                {
                    lineRenderers[i] = ropeSegments[i].gameObject.AddComponent<LineRenderer>();
                }

                lineRenderers[i].startWidth = ropeThickness;
                lineRenderers[i].endWidth = ropeThickness;
                lineRenderers[i].material = ropeMaterial;
                lineRenderers[i].textureMode = LineTextureMode.Tile;
                lineRenderers[i].positionCount = 2;

                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_giz, 0.3f);
        }

        
        [Button("Break Rope", ButtonSizes.Large), GUIColor(1, 0.3f, 0.3f)]
        public void BreakRope()
        {
            Destroy(ropeSegments[breakIndex].GetComponent<HingeJoint>());
            Destroy(lineRenderers[breakIndex]);
            broken = true;
        }

        private bool broken;
        
        #endregion

        #region Bultin Methods

        private void Update()
        {
            //for each line render segment set the 2nd position to the previous segment position and 1st position to current segment position
            for (int i = 1; i < ropeResolution; i++)
            {
                if (broken && i == breakIndex)
                {
                    continue;
                }
                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }

        #endregion

        #region Custom Methods For Rope Movement

        /// <summary>
        /// Moves the player along the rope based on the input.
        /// </summary>
        /// <param name="input">The vertical input from the player.</param>
        public void MovePlayer(float input)
        {
            if (!_connected) return;

            // If the player is moving upwards along the rope
            if (input > 0.2f)
            {
                _closestIndex = Mathf.MoveTowards(_closestIndex, _closestIndex - 1, Time.deltaTime * climbSpeed);
            }
            // If the player is moving downwards along the rope
            else if (input < -0.2f)
            {
                _closestIndex = Mathf.MoveTowards(_closestIndex, _closestIndex + 1, Time.deltaTime * climbSpeed);
            }

            // Ensure the closest index is within the bounds of the rope resolution
            _closestIndex = Mathf.Clamp(_closestIndex, 0, ropeResolution - 1);

            var playerInstance = PlayerMovementController.Instance;
            var playerInstanceTransform = playerInstance.transform;
            Vector3 newOffset = playerInstance.player.ropeMovement.offset;

            newOffset = newOffset.x * playerInstanceTransform.right + newOffset.y * playerInstanceTransform.up +
                        newOffset.z * playerInstanceTransform.forward;

            playerInstanceTransform.position = GetDesiredPosition() + newOffset;
            playerInstanceTransform.rotation = ropeSegments[(int)Mathf.Floor(_closestIndex)].transform.rotation;
        }

        /// <summary>
        /// Applies a force to the rope segment closest to the player, causing the rope to swing.
        /// The direction and magnitude of the force are determined by the player's input.
        /// </summary>
        /// <param name="input">The horizontal input from the player.</param>
        public void SwingRope(float input)
        {
            if (_connected)
            {
                Rigidbody rb = ropeSegments[(int)Mathf.Floor(_closestIndex)];
                print(input);
                rb.AddForce(0, 0, swingForce * input * Time.deltaTime);
            }
        }

        /// <summary>
        /// Calculates the desired position of the player on the rope.
        /// </summary>
        /// <returns>The desired position of the player on the rope.</returns>
        private Vector3 GetDesiredPosition()
        {
            // Get the index of the closest rope segment
            int closestIndexFloor = (int)Mathf.Floor(_closestIndex);
            Vector3 newPos = ropeSegments[closestIndexFloor].transform.position;

            // If there is a next rope segment, interpolate the position between the current and next segment
            if (closestIndexFloor + 1 < ropeResolution)
            {
                Vector3 direction = ropeSegments[closestIndexFloor].transform.position -
                                    ropeSegments[closestIndexFloor + 1].transform.position;
                float distance = _closestIndex - closestIndexFloor;
                newPos -= direction * distance;
            }

            // Debugging line, can be removed
            _giz = newPos;
            return newPos;
        }

        /// <summary>
        /// Calculates the distance from the player to each rope segment to find the closest one.
        /// </summary>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        private IEnumerator InitialConnect()
        {
            for (int i = 1; i < ropeResolution; i++)
            {
                var distance =
                    Vector3.Distance(PlayerMovementController.Instance.player.ropeMovement.handSocket.position,
                        ropeSegments[i].transform.position);

                // If the current segment is closer than the previous closest, update the closest distance and index
                if (distance < _closestDistance)
                {
                    _closestDistance = distance;
                    _closestIndex = i;
                }

                yield return null;
            }

            //direction of the current closest rope segment from player

            //apply an impulse force in that direction
            ropeSegments[(int)Mathf.Floor(_closestIndex)]
                .AddForce(PlayerMovementController.Instance.transform.forward * entryForce, ForceMode.Impulse);
            // Once all segments have been checked, set the rope as connected
            _connected = true;
        }

        public Rigidbody CurrentRopeSegment()
        {
            return ropeSegments[(int)Mathf.Floor(_closestIndex)];
        }

        /// <summary>
        /// Attempts to attach the player to the rope.
        /// </summary>
        public void AttachPlayer()
        {
            // If the last attachment was less than 1.5 seconds ago, do not attach again
            if (Time.time - _lastAttachedTime < 1.5f) return;

            // If the player can be attached to the rope
            if (PlayerMovementController.Instance.player.ropeMovement.AttachRope(this))
            {
                print("Fuck Passed");
                // Start the initial connection process
                StartCoroutine(InitialConnect());
                _lastAttachedTime = Time.time;
            }
        }

        /// <summary>
        /// Detaches the player from the rope and resets the relevant variables.
        /// </summary>
        public void Detached()
        {
            // Record the time of detachment
            _lastAttachedTime = Time.time;
            // Reset the closest distance to a large value
            _closestDistance = 100;
            // Set the connection status to false
            _connected = false;
        }

        #endregion
    }
}