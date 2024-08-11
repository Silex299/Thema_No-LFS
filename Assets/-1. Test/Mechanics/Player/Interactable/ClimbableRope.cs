using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player.Interactable
{
    public class ClimbableRope : ClimbableBase
    {
        #region Rope Properties

        [SerializeField, BoxGroup("Rope Properties")]
        private int breakIndex;

        [SerializeField, BoxGroup("Rope Properties")]
        private Vector3 breakForce;

        #endregion

        #region Player Movement

        [SerializeField, BoxGroup("Movement")] private float climbSpeed = 2f;
        [SerializeField, BoxGroup("Movement")] private float swingForce = 200;
        [SerializeField, BoxGroup("Movement")] private float entryForce = 10;
        [SerializeField, BoxGroup("Movement")] internal Vector3 exitForce = new Vector3(0, 5, 5);

        #endregion

        #region Unexposed Variables

        [SerializeField] private Rigidbody[] ropeSegments;
        [SerializeField] private LineRenderer[] lineRenderers;
        public Vector3 initialRotation;
        private bool _connected;
        private float _closestIndex;
        private float _closestDistance = 100f;
        private float _lastAttachedTime;
        private bool _canAttach = true;


        public bool Connected
        {
            get => _connected;
            set { _connected = value; }
        }

        #endregion

        #region Editor Specific

        [SerializeField, BoxGroup("Editor - Rope Properties")]
        private int ropeResolution;

        [SerializeField, BoxGroup("Editor - Rope Properties")]
        private float ropeLength;

        [SerializeField, BoxGroup("Editor - Rope Properties")]
        private float ropeThickness;

        [SerializeField, BoxGroup("Editor - Rope Properties")]
        private Material ropeMaterial;


        [Tooltip(
            "TIt determines how strongly the joint will try to maintain its position. A higher spring value will make the joint stiffer and more resistant to rotational movement")]
        [SerializeField, BoxGroup("Editor - Rope Properties")]
        private float spring;

        [Tooltip(
            "It determines how quickly the joint will come to rest after being moved. A higher damping value will make the joint slow down and stop more quickly after being moved")]
        [SerializeField, BoxGroup("Editor - Rope Properties")]
        private float damp;


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
                    layer = LayerMask.NameToLayer("Rope")
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

            for (int i = 1; i < ropeResolution; i++)
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

        /// <summary>
        /// Breaks the rope
        /// </summary>
        /// <param name="exitFall"> if false, doesn't initiate exit animation after player fell on the ground </param>
        [Button("Break Rope", ButtonSizes.Large), GUIColor(1, 0.3f, 0.3f)]
        public void BreakRope(bool exitFall = true)
        {
            Destroy(ropeSegments[breakIndex].GetComponent<HingeJoint>());
            Destroy(lineRenderers[breakIndex]);

            ropeSegments[breakIndex + 1].AddForce(breakForce, ForceMode.Impulse);

            _broken = true;
            _canAttach = false;

            /**
            if (PlayerMovementController.Instance.VerifyState(PlayerMovementState.Rope))
            {
                StartCoroutine(PlayerMovementController.Instance.player.ropeMovement.BrokRope(this, exitFall));
            }
            **/
        }

        private bool _broken;

        #endregion

        #region Bultin Methods

        private void Update()
        {
            //for each line render segment set the 2nd position to the previous segment position and 1st position to current segment position
            for (int i = 1; i < ropeResolution; i++)
            {
                if (_broken && i == breakIndex)
                {
                    continue;
                }

                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }

        #endregion


        public float swingInterval = 1f;
        public float swingDelay;
        public float swingTime;
        
        private Coroutine _swingCoroutine;
        private bool _isSwinging;

        public override Vector3 GetMovementVector(Transform playerTransform, float speed)
        {
            var input = Input.GetAxis("Vertical");

            if (input > 0.2f)
            {
                _closestIndex = Mathf.MoveTowards(_closestIndex, _closestIndex - 1, speed);
            }
            else if (input < -0.2f)
            {
                _closestIndex = Mathf.MoveTowards(_closestIndex, _closestIndex + 1, speed);
            }

            _closestIndex = Mathf.Clamp(_closestIndex, 0, ropeResolution - 1);
            
            return GetDesiredPosition() - playerTransform.position;
        }

        public override Vector3 GetInitialConnectPoint(Transform playerTransform)
        {
            for (int i = 1; i < ropeResolution; i++)
            {
                var distance = Vector3.Distance(playerTransform.position, ropeSegments[i].transform.position);

                // If the current segment is closer than the previous closest, update the closest distance and index
                if (distance < _closestDistance)
                {
                    _closestDistance = distance;
                    _closestIndex = i;
                }

                //direction of the current closest rope segment from player
                Rigidbody currentSegment = ropeSegments[(int)Mathf.Floor(_closestIndex)];

                if (Mathf.Abs(currentSegment.velocity.magnitude) < 1f)
                {
                    //apply an impulse force in that direction
                    currentSegment.AddForce(playerTransform.forward * entryForce, ForceMode.Impulse);
                }
            }

            return GetDesiredPosition();
        }

        public override Quaternion GetInitialRotation(Transform playerTransform)
        {
            var segmentRotation = ropeSegments[(int)Mathf.Floor(_closestIndex)].transform.eulerAngles;

            segmentRotation.y = playerTransform.eulerAngles.y;
            return Quaternion.Euler(segmentRotation);
        }

        public override Quaternion GetMovementRotation(Transform playerTransform)
        {
            var segmentRotation = ropeSegments[(int)Mathf.Floor(_closestIndex)].transform.eulerAngles;

            segmentRotation.y = playerTransform.eulerAngles.y;
            return Quaternion.Euler(segmentRotation);
        }

        public override void UpdateClimbable(PlayerV1 player)
        {
            if (Input.GetButton("Horizontal"))
            {
                _swingCoroutine ??= StartCoroutine(SwingRope());
            }
        }

        private IEnumerator SwingRope()
        {
            
            var inputSign = Mathf.Sign(Input.GetAxis("Horizontal"));
            Rigidbody rb = ropeSegments[(int)Mathf.Floor(_closestIndex)];

            _isSwinging = true;
            
            
            rb.AddForce(swingForce * inputSign, 0 , 0, ForceMode.Acceleration);
            
            yield return new WaitForSeconds(swingInterval);

            _swingCoroutine = null;
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
    }
}