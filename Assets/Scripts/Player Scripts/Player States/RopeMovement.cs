using System.Collections;
using Interactable_Items;
using UnityEngine;

namespace Player_Scripts.Player_States
{
    [System.Serializable]
    public class RopeMovement : PlayerBaseState
    {
        #region Variables
        
            public RopeSegment connectedBody;
            public float force;
            
            private bool _detachFall;
            private Vector3 _detachVelocity;

            private static readonly int Vertical = Animator.StringToHash("Speed");
            private static readonly int Horizontal = Animator.StringToHash("Direction");
            private static readonly int JumpPlayer = Animator.StringToHash("Jump");

        #endregion
        

        #region  Unused Methods

            public override bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0)
            {
                return false;
            }
            public override void OnGizmos(PlayerController controller)
            {
            }
            public override void OnStateExit(PlayerController controller)
            {
            }
            public override void SimpleInteract(PlayerController controller, int value = 0)
            {
            }
            public override void OnStateLateUpdate(PlayerController controller)
            {


        
            }

        #endregion


        #region Overriden Methods

            public override void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.5f)
            {

                controller.Player.AnimationController.CrossFade("ROPE", stateTransitionTime);
                controller.Player.PlayerController.enabled = false;
                _detachFall = false;

            }


            public override void OnStateUpdate(PlayerController controller)
            {
       
                //Player fall on detaching from the rope
                if (_detachFall)
                {
                    var movePlayer = Vector3.zero;
                   
                    _detachVelocity.y -= 10f * Time.deltaTime;

                    movePlayer.y = _detachVelocity.y;
                    movePlayer.x = _detachVelocity.x;
                    movePlayer.z = _detachVelocity.z;


                    controller.Player.PlayerController.Move(movePlayer * Time.deltaTime);
                    GroundCheck(controller); 
                    
                    return;

                }


                if (_detachFall) return;

                //If player is connected to the rope and jump key is pressed -> detach the player from the rope
                if (Input.GetButtonDown("Jump"))
                {
                    controller.StartCoroutine(Jump(controller));
                }

            }

            public override void OnStateFixedUpdate(PlayerController controller)
            {
                //Test in update
                var verticalInput = Input.GetAxis("Vertical");
                var horizontalInput = Input.GetAxis("Horizontal");

                var angle = Vector3.SignedAngle(controller.transform.forward, Vector3.forward, Vector3.up);
                var isForward = angle < 0;
                
                //Update animation
                controller.Player.AnimationController.SetFloat(Vertical, verticalInput);
                controller.Player.AnimationController.SetFloat(Horizontal, (isForward ? 1 : -1) * horizontalInput);


                var transform = controller.transform;
                var position = transform.position;
                
                
                if(!connectedBody) return;

                //VERTICAL: add a little force on moving up and down
                if (verticalInput > 0.5f)
                {
                    connectedBody.rb.AddForceAtPosition(Vector3.up * 0.1f, position, ForceMode.Impulse);
                }
                

                //HORIZONTAL: apply force according to player's direction and input by the user
                switch (horizontalInput)
                {
                    case > 0.1f:
                        connectedBody.rb.AddForceAtPosition( transform.forward * ((isForward?1:-1) * force), position, ForceMode.Impulse);
                        break;
                    case < -0.1f:
                        connectedBody.rb.AddForceAtPosition(-transform.forward * ((isForward ? 1 : -1) * force), position,
                            ForceMode.Impulse);
                        break;
                }
            }

        #endregion


        #region Custom methods

        
            //Checks if player hits any surface on falling and change state accordingly
            private void GroundCheck(PlayerController controller)
            {

                LayerMask mask = 1 << 0 | 1<<4;

                if (!Physics.Raycast(controller.transform.position, -controller.transform.up, out RaycastHit hit, 0.25f,
                        mask)) return;
                
                
                _detachFall = false;
                connectedBody = null;
                
                //Change player state according to what it hit
                if (hit.collider.CompareTag("Water"))
                {
                    controller.Player.AnimationController.CrossFade("Falling into the pool", 0.2f, 1);
                }
                else
                {
                    controller.ChangeState(PlayerController.PlayerStates.BasicFreeMovement);
                    controller.Player.AnimationController.CrossFade("Hard Landing", 0.2f, 1);
                }
            }
            
            public bool SetConnectedRigidBody(RopeSegment rope)
            {
                
                //Return false if call is coming from the same connected rope to the player
                if (rope == connectedBody) return false;
                
                connectedBody = rope;
                
                //Apply force to rope on connecting
                if (PlayerController.Instance.initState == PlayerController.PlayerStates.RopeMovement)
                {
                    var transform = PlayerController.Instance.transform;
                    var angle = Vector3.Angle(transform.forward, _detachVelocity);

                    //Force direction according to player direction and player velocity
                    if (angle > 90)
                    {
                        connectedBody.rb.AddForceAtPosition(-transform.forward * 10f, transform.position, ForceMode.Acceleration);
                    }
                    else
                    {
                        connectedBody.rb.AddForceAtPosition(transform.forward * 10f, transform.position, ForceMode.Acceleration);
                    }
                    
                }
                
                return true;
            }

            private IEnumerator Jump(PlayerController controller)
            {
                
                controller.Player.AnimationController.SetTrigger(JumpPlayer);
                
                var transform = controller.transform;
                
                //Enable CharacterController, so to perform falling action
                controller.Player.PlayerController.enabled = true;
                
                //Detach player from the rope
                transform.parent = Managers.GameManager.instance.transform;
                connectedBody.isAttached = false;
                
                Vector3 euler = transform.eulerAngles;
                
                //Zero out player's rotation except rotation in y axis
                controller.transform.rotation = Quaternion.Euler(0, euler.y, 0); 
                

                //Velocity vector for falling player
                _detachVelocity = (transform.position - connectedBody.rb.position) * (connectedBody.rb.angularVelocity.magnitude * 0.5f);
                _detachVelocity.y *= -1;
                _detachFall = true;
                
                yield return new WaitForSeconds(2f);
                
                controller.Player.AnimationController.ResetTrigger(JumpPlayer);
                


            }

        #endregion

        

    }
}