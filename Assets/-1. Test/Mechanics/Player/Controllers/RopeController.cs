using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Player.Controllers
{
    public class RopeController : Controller
    {
        #region Rope Properties

        [BoxGroup("Rope Properties")] public int breakIndex;
        [BoxGroup("Rope Properties")] public Vector3 breakForce;
        [BoxGroup("Rope Properties")] public bool canJumpOff = true;

        #endregion

        #region Player Movement

        [FoldoutGroup("Movement")] public Vector3 playerOffset;
        [FoldoutGroup("Movement")] public float transitionTime = 0.2f;
        [FoldoutGroup("Movement")] public float movementSpeed = 0.1f;

        [FoldoutGroup("Entry and Swing")] public float entryForce = 10;
        [FoldoutGroup("Movement")] public float swingInterval = 1f;
        [FoldoutGroup("Entry and Swing")] public Vector3 swingForce;
        [FoldoutGroup("Entry and Swing")] public Vector3 exitForce = new Vector3(0, 5, 5);

        #endregion

        public UnityEvent exitEvent;


        #region Unexposed Variables

        [SerializeField] private Rigidbody[] ropeSegments;
        [SerializeField] private LineRenderer[] lineRenderers;

        private bool _engaged;
        private float _closestIndex;
        private float _closestDistance = 100f;

        private Coroutine _engageCoroutine;
        private Coroutine _swingCoroutine;
        private Coroutine _exitCoroutine;

        private bool _broken;

        private static readonly int Jump = Animator.StringToHash("Jump");

        #endregion

        #region Editor Specific

        [SerializeField, FoldoutGroup("Editor - Rope Properties")]
        private int ropeResolution;

        [SerializeField, FoldoutGroup("Editor - Rope Properties")]
        private float ropeLength;

        [SerializeField, FoldoutGroup("Editor - Rope Properties")]
        private float ropeThickness;

        [SerializeField, FoldoutGroup("Editor - Rope Properties")]
        private Material ropeMaterial;


        [Tooltip(
            "TIt determines how strongly the joint will try to maintain its position. A higher spring value will make the joint stiffer and more resistant to rotational movement")]
        [SerializeField, FoldoutGroup("Editor - Rope Properties")]
        private float spring;

        [Tooltip(
            "It determines how quickly the joint will come to rest after being moved. A higher damping value will make the joint slow down and stop more quickly after being moved")]
        [SerializeField, FoldoutGroup("Editor - Rope Properties")]
        private float damp;


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
            Gizmos.DrawWireSphere(GetDesiredPosition(), 0.3f);
        }

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

        #region Entry

        public override void ControllerEnter(PlayerV1 player)
        {
            base.ControllerEnter(player);
            _engageCoroutine ??= StartCoroutine(EngagePlayer(player));
        }

        private IEnumerator EngagePlayer(PlayerV1 player)
        {
            Vector3 targetPos = GetInitialConnectPoint(player.transform);
            Quaternion targetRot = GetInitialRotation(player.transform);
            Vector3 initPlayerPos = player.transform.position;
            Quaternion initPlayerRot = player.transform.rotation;

            float timeElapsed = 0;

            while (timeElapsed < transitionTime)
            {
                player.transform.position = Vector3.Lerp(initPlayerPos, targetPos, timeElapsed / transitionTime);
                player.transform.rotation = Quaternion.Slerp(initPlayerRot, targetRot, timeElapsed / transitionTime);
                timeElapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            _engaged = true;
            _engageCoroutine = null;
        }
        
        private Vector3 GetInitialConnectPoint(Transform playerTransform)
        {
            _closestDistance = Mathf.Infinity;

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
        
        private Quaternion GetInitialRotation(Transform playerTransform)
        {
            var segmentRotation = ropeSegments[(int)Mathf.Floor(_closestIndex)].transform.eulerAngles;

            segmentRotation.y = playerTransform.eulerAngles.y;
            return Quaternion.Euler(segmentRotation);
        }

        #endregion


        #region Action

        #region General Movement
        public override void ControllerUpdate(PlayerV1 player)
        {
            if (!_engaged) return;
            MovePlayer(player);
        }


        private void MovePlayer(PlayerV1 player)
        {
            #region Player Position and Rotation

            Vector3 movementVector = GetMovementVector(player.transform, movementSpeed * Time.deltaTime);
            player.characterController.Move(movementVector);
            player.transform.rotation = GetMovementRotation(player.transform);

            #endregion

            #region player jump

            //Jump off the climbable
            if (Input.GetButtonDown("Jump") && canJumpOff)
            {
                _exitCoroutine ??= StartCoroutine(JumpOff(player));
            }

            #endregion
        }
        
        private Vector3 GetMovementVector(Transform playerTransform, float speed)
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

            return GetDesiredPosition() - playerTransform.position + playerOffset;
        }
        private Quaternion GetMovementRotation(Transform playerTransform)
        {
            var segmentRotation = ropeSegments[(int)Mathf.Floor(_closestIndex)].transform.eulerAngles;

            segmentRotation.y = playerTransform.eulerAngles.y;
            return Quaternion.Euler(segmentRotation);
        }

        
        private IEnumerator JumpOff(PlayerV1 player)
        {
            _engaged = false;

            #region Player Contstrains

            player.GroundCheck();
            player.AddForce(exitForce);

            //Reset Rotation
            player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);

            #endregion

            #region Trigger Jump off

            _engaged = false;
            player.animator.SetTrigger(Jump);

            #endregion

            #region apply gravity untill grounded

            while (!player.IsGrounded)
            {
                player.ApplyGravity();
                yield return null;
            }

            #endregion

            #region exit climbable action

            exitEvent.Invoke();
            _exitCoroutine = null;

            #endregion
        }
        
        #endregion
        
        private IEnumerator SwingRope()
        {
            var inputSign = Mathf.Sign(Input.GetAxis("Horizontal"));
            Rigidbody rb = ropeSegments[(int)Mathf.Floor(_closestIndex)];

            rb.AddForce(swingForce * inputSign, ForceMode.Acceleration);

            yield return new WaitForSeconds(swingInterval);

            _swingCoroutine = null;
        }

        #endregion
        
        #region Other Methods
        
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
            
            return newPos;
        }
        
        #endregion
    }
}