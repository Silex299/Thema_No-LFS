using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Mechanics.Player.Controllers
{
    public class RopeControllerV1 : Controller
    {
        #region DEBUG

#if UNITY_EDITOR


        [FoldoutGroup("Rope Properties")] public int ropeResolution;
        [FoldoutGroup("Rope Properties")] public float ropeLength;
        [FoldoutGroup("Rope Properties")] public float ropeWidth;
        [FoldoutGroup("Rope Properties")] public Material ropeMaterial;

        [Button]
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
                    addedCollider.radius = ropeWidth / 2;
                    HingeJoint joint = ropeSegments[i].gameObject.AddComponent<HingeJoint>();
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

        [Button]
        public void CreateLineRenderer()
        {
            lineRenderers = new LineRenderer[ropeResolution];

            for (int i = 1; i < ropeResolution; i++)
            {
                if (!ropeSegments[i].TryGetComponent<LineRenderer>(out lineRenderers[i]))
                {
                    lineRenderers[i] = ropeSegments[i].gameObject.AddComponent<LineRenderer>();
                }

                lineRenderers[i].startWidth = ropeWidth;
                lineRenderers[i].endWidth = ropeWidth;
                lineRenderers[i].material = ropeMaterial;
                lineRenderers[i].textureMode = LineTextureMode.Tile;
                lineRenderers[i].positionCount = 2;

                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }


        private void OnDrawGizmos()
        {
            GetDesiredTransform(out var desiredPos, out var desiredRot);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(desiredPos, 0.2f);
        }

#endif

        #endregion

        #region Player Movement

        [FoldoutGroup("Player Movement")] public Vector3 playerPosOffset;
        [FoldoutGroup("Player Movement")]public float transitionTime;
        [FoldoutGroup("Player Movement")]public float movementSpeed;
        
        
        [FoldoutGroup("Rope Physics")]public float entryForce;
        [FoldoutGroup("Rope Physics")]public float exitForceMultiplier;
        [FoldoutGroup("Rope Physics")]public float swingTime;
        [FoldoutGroup("Rope Physics")]public float swingDelay;
        
        [FoldoutGroup("Rope Physics")]public Vector3 swingSpeed;

        #region Rope Segments

        [SerializeField, FoldoutGroup("Misc")] private Rigidbody[] ropeSegments;
        [SerializeField, FoldoutGroup("Misc")] private LineRenderer[] lineRenderers;

        #endregion
        
        public UnityEvent exitEvent;

        #endregion
        #region Static and private variables
        
        private Vector3 _initialRot;
        private float _closestIndex;
        private float _closestDistance;
        private bool _engaged;

        private Coroutine _engageCoroutine;
        private Coroutine _swingCoroutine;
        private Coroutine _jumpCoroutine;
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private static readonly int Jump = Animator.StringToHash("Jump");

        #endregion

        #region Enter

        public override void ControllerEnter(PlayerV1 player)
        {
            if (_engaged) return;

            base.ControllerEnter(player);
            _engageCoroutine ??= StartCoroutine(EngagePlayer(player));
        }


        private IEnumerator EngagePlayer(PlayerV1 player)
        {
            Transform playerTransform = player.transform;
            

            InitialConnect(playerTransform, out var targetPos, out var targetRot);
            
            var initPlayerPos = playerTransform.position;
            var initPlayerRot = playerTransform.rotation;
            targetPos += playerPosOffset.x * playerTransform.right + playerPosOffset.y * playerTransform.up + playerPosOffset.z * playerTransform.forward;
            
            float timeElapsed = 0;
            while (timeElapsed < transitionTime)
            {
                timeElapsed += Time.deltaTime;
                playerTransform.position = Vector3.Lerp(initPlayerPos, targetPos, timeElapsed / transitionTime);
                playerTransform.rotation = Quaternion.Slerp(initPlayerRot, targetRot, timeElapsed / transitionTime);

                yield return new WaitForEndOfFrame();
            }

            _engaged = true;
            _engageCoroutine = null;
        }

        private void InitialConnect(Transform playerTransform, out Vector3 desiredPos, out Quaternion desiredRot)
        {
            _closestDistance = Mathf.Infinity;

            //Calculate closest rope Segment
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
            
            _initialRot = playerTransform.rotation.eulerAngles;
            GetDesiredTransform(out desiredPos, out desiredRot);
        }

        #endregion
        
        #region Action

        public override void ControllerUpdate(PlayerV1 player)
        {
            if (!_engaged) return;
            MovePlayer(player);
        }

        private void MovePlayer(PlayerV1 player)
        {
            var input = Input.GetAxis("Vertical");

            #region  Player movement and rotation
            
            GetMovementTransform(input, out var desiredPos, out var desiredRot);

            var playerTransform = player.transform;
            desiredPos += playerPosOffset.x * playerTransform.right + playerPosOffset.y * playerTransform.up + playerPosOffset.z * playerTransform.forward;
            
            player.characterController.Move(desiredPos - player.transform.position);
            
            playerTransform.rotation = desiredRot;
            player.animator.SetFloat(Speed, input);

            #endregion

            #region Swing and Jump
            
            if (Input.GetButtonDown("Horizontal")) _swingCoroutine ??= StartCoroutine(SwingCoroutine(player));
            if (Input.GetButtonDown("Jump")) _jumpCoroutine ??= StartCoroutine(JumpOff(player));
            
            #endregion
        }

        private IEnumerator SwingCoroutine(PlayerV1 player)
        {
            var input = Input.GetAxis("Horizontal");
            player.animator.SetFloat(Direction, input);
            Rigidbody rb = ropeSegments[(int)Mathf.Floor(_closestIndex)];
            float timeElapsed = 0;

            while (timeElapsed < swingTime)
            {
                timeElapsed += Time.deltaTime;
                rb.AddForce(swingSpeed * input, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(swingDelay - swingTime);
            _swingCoroutine = null;
        }

        private IEnumerator JumpOff(PlayerV1 player)
        {
            #region playerConstarin

            player.transform.rotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
            player.GroundCheck();
            GetCurrentSegmentVelocity(out var exitForce);
            player.AddForce(exitForce * exitForceMultiplier);

            #endregion

            #region jump trigger

            _engaged = false;
            player.animator.SetTrigger(Jump);

            #endregion

            #region Apply gravity untill grounded

            while (!player.IsGrounded)
            {
                player.ApplyGravity();
                yield return null;
            }

            #endregion

            #region Exit Controller

            exitEvent.Invoke();
            _jumpCoroutine = null;

            #endregion
        }

        private void Update()
        {
            for (int i = 1; i < ropeResolution; i++)
            {
                //TODO: ROPE BREAK
                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }

        #endregion

        #region Helper Methods

        
        private void GetCurrentSegmentVelocity(out Vector3 velocity)
        {
            Rigidbody currentSegment = ropeSegments[(int)Mathf.Floor(_closestIndex)];
            velocity = currentSegment.velocity;
        }

        private void GetMovementTransform(float input, out Vector3 desiredPos, out Quaternion desiredRot)
        {
            _closestIndex -= (input * movementSpeed * Time.deltaTime);
            _closestIndex = Mathf.Clamp(_closestIndex, 0, ropeResolution - 1);
            GetDesiredTransform(out desiredPos, out desiredRot);
        }

        private void GetDesiredTransform(out Vector3 desiredPos, out Quaternion desiredRot)
        {
            // Get the index of the closest rope segment
            int closestIndexFloor = (int)Mathf.Floor(_closestIndex);
            Transform currentRoeSegmentTransform = ropeSegments[closestIndexFloor].transform;

            Vector3 newPos = currentRoeSegmentTransform.position;

            if (closestIndexFloor + 1 < ropeResolution)
            {
                Vector3 direction = currentRoeSegmentTransform.position - ropeSegments[closestIndexFloor + 1].transform.position;
                float distance = _closestIndex - closestIndexFloor;
                newPos -= direction * distance;
            }

            desiredPos = newPos;
            
            var newRotation = ropeSegments[(int)Mathf.Floor(_closestIndex)].transform.rotation.eulerAngles;
            newRotation = _initialRot.y > 180 ? -newRotation : newRotation;
            
            if (_initialRot.y is > 0 and < 180)
            {
                newRotation.y = 90;
            }
            else
            {
                newRotation.y = -90;
            }
            

            
            desiredRot = Quaternion.Euler(newRotation);
        }


        #endregion

        #region Exit

        public override void ControllerExit(PlayerV1 player)
        {
            _engaged = false;
            player.ResetMovement();
        }

        #endregion
    }
}