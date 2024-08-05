using PlasticPipe.PlasticProtocol.Messages;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Player
{
    public class BasicController : Controller
    {
        [FoldoutGroup("Movement")] public float movementSpeed = 10;
        [FoldoutGroup("Movement")] public float rotationSpeed = 10;
        [FoldoutGroup("Movement")] public float jumpHeight = 10;

        [FoldoutGroup("Movement")] public LayerMask groundMask;
        [FoldoutGroup("Movement")] public float groundDistance = 0.4f;
        [FoldoutGroup("Movement")] public float proximityThreshold = 1f;
        [FoldoutGroup("Movement")] public Transform groundCheck;

        private PlayerPathManager _pathManager;
        private Vector3 _movementDirection;
        private Vector3 _lastGroundedMove;

        private void Start()
        {
            _pathManager = PlayerPathManager.Instance;
        }
        
        public override void UpdateController(PlayerV1 player)
        {
            MovePlayer(player);
            AnimatePlayer(player);
        }
        
        
        
        private void MovePlayer(PlayerV1 player)
        {
            if (player.isGrounded && _movementDirection.y < 0)
            {
                _movementDirection.y = -2f;
            }

            var horizontalInput = Input.GetAxis("Horizontal");
            if (player.isGrounded)
            {
                _lastGroundedMove = player.transform.forward * (Mathf.Abs(horizontalInput) * movementSpeed);
            }

            if (Input.GetButtonDown("Jump") && player.isGrounded)
            {
                _movementDirection.y = Mathf.Sqrt(jumpHeight * -2f * -9.8f);
            }
            _movementDirection.y += -9.8f * Time.deltaTime;

            player.characterController.Move((_lastGroundedMove + _movementDirection) * Time.deltaTime);

            
            Vector3 lookAt = _pathManager.GetDestination(player.transform.position, horizontalInput > 0);
            if (player.isGrounded && !Mathf.Approximately(horizontalInput, 0))
            {
                Rotate(player.transform, lookAt);
            }
            GroundCheck(player);
            
        }
        private void GroundCheck(PlayerV1 player)
        {
            player.isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            player.isInGroundProximity = Physics.CheckSphere(groundCheck.position, proximityThreshold, groundMask);
            
            Debug.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * (player.isGrounded? groundDistance : proximityThreshold),
                player.isGrounded ? Color.green : player.isInGroundProximity? Color.blue : Color.red, 1f);
        }
        private void Rotate(Transform target, Vector3 lookAt)
        {
            Vector3 direction = (lookAt - target.position).normalized;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            target.rotation = Quaternion.Slerp(target.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        private void AnimatePlayer(PlayerV1 player)
        {
            
        }
        
    }
}