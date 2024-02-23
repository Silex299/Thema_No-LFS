using MyCamera;
using UnityEngine;
namespace Player_Scripts.Player_States
{

    [System.Serializable]
    public class UnderWaterMovement : PlayerBaseState
    {

        #region Variables


        #region exposed varibles 

        [SerializeField]
        private float maximumSpeed;
        [SerializeField] private float playerRotationSmoothness;
        [SerializeField] private float breathLostFactor = 1;

        #endregion

        #region Non exposed variables
        private float turnSmoothVelocity;
        private float playerDepth;
        private float waterDepth = 100;
        private float breath = 100;
        private bool isUnderWater;

        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        #endregion

        #region Overriden methods
        public override void OnStateEnter(PlayerController controller, int index = 0, float stateTransitionTime = 0.1f)
        {
            controller.Player.AnimationController.CrossFade("UW", stateTransitionTime, 0);
            controller.Player.PlayerController.height = 0.3f;
            Physics.gravity /= 2;
        }
        public override void OnGizmos(PlayerController controller)
        {

            CheckDepth(controller);

            //Upper raycast

            Gizmos.color = Color.green;
            Gizmos.DrawLine(controller.transform.position, controller.transform.position + Vector3.up * playerDepth);

            //Lower raycast

            Gizmos.color = Color.red;
            Gizmos.DrawLine(controller.transform.position, controller.transform.position + Vector3.down * waterDepth);



        }
        public override void OnStateUpdate(PlayerController controller)
        {
            Vector3 direction = Vector3.zero;

            direction.x = Input.GetAxis("Horizontal");
            direction.y = Input.GetAxis("Vertical");


            if (direction.y >= 0 && playerDepth < 1.1f)
            {
                direction.y = 0;
            }

            ChangeCameraOffset();

            RotatePlayer(controller, direction);

            controller.Player.AnimationController.SetFloat(Speed, direction.magnitude, 0.1f, Time.deltaTime);
            controller.Player.PlayerController.Move(direction * maximumSpeed * Time.deltaTime);


            //BREATH
            if (isUnderWater)
            {
                LoseBreath();
            }
            else
            {
                RefreshBreath();
            }

        }
        public override void OnStateFixedUpdate(PlayerController controller)
        {
            CheckDepth(controller);
        }


        public override void OnStateExit(PlayerController controller)
        {
            Physics.gravity *= 2;
            controller.Player.PlayerController.height = 1.43f;
        }

        #endregion

        #region unused methods

        public override bool Interact(PlayerController controller, InteractionType type, bool status = false, float value = 0)
        {
            throw new System.NotImplementedException();
        }

        public override void OnStateLateUpdate(PlayerController controller)
        {
        }
        public override void SimpleInteract(PlayerController controller, int value = 0)
        {
        }

        #endregion

        #region Custom Methods

        private void RotatePlayer(PlayerController controller, Vector3 direction)
        {

            float targetAngle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;

            Vector3 eularAngle = Vector3.zero;
            var rot = controller.transform.rotation;


            if (direction.x > 0)
            {
                eularAngle.y = 90;

                if (direction.y != 0)
                {
                    eularAngle.x = targetAngle;
                }
            }
            else if (direction.x < 0)
            {
                eularAngle.y = -90;

                if (direction.y != 0)
                {
                    eularAngle.x = 180 - targetAngle;
                }
            }
            else
            {
                eularAngle.y = rot.eulerAngles.y;
                if (direction.y != 0)
                {
                    eularAngle.x = targetAngle;
                }
            }


            controller.transform.rotation = Quaternion.Lerp(rot, Quaternion.Euler(eularAngle), Time.deltaTime * playerRotationSmoothness);

        }

        private void CheckDepth(PlayerController controller)
        {
            //TODO REMOVE
            var position = controller.transform.position;

            //Collides only with water surface
            if (Physics.Raycast(position, Vector3.up, out RaycastHit hit, 20f, 1 << 4))
            {
                playerDepth = hit.distance;

                if (playerDepth < 0.9f)
                {
                    controller.transform.position = Vector3.Lerp(position, hit.point + Vector3.down, Time.deltaTime * maximumSpeed);
                }
            }
            else
            {
                playerDepth = 0;
            }

            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit1, 20f, 1 << 7))
            {
                waterDepth = hit1.distance;

                if (playerDepth < 1.2f && waterDepth < 0.3f)
                {
                    //Change Exit State
                    //CameraFollow.Instance.ChangeOffset(new Vector3(0.9f, 3f, -10f));
                    controller.ChangeState(PlayerController.PlayerStates.BasicRestrictedMovement, 1);
                }
            }
            else
            {
                waterDepth = 100;
            }

        }
        private void ChangeCameraOffset()
        {

            //Make camera offsets less rigid
            if (isUnderWater && playerDepth < 1.2f)
            {
                //change to head above water camera offset;
                CameraManager.Instance.ChangeCameraFollowOffset(new Vector3(0.9f, 3f, -10f), 1);
                isUnderWater = false;
            }
            else if (!isUnderWater && playerDepth >= 1.2f)
            {
                //Change to underwater camera offset
                CameraManager.Instance.ChangeCameraFollowOffset(new Vector3(0.9f, -0.25f, -10f), 1);
                isUnderWater = true;
            }
        }
        private void LoseBreath()
        {
            if (breath <= 0)
            {
                PlayerController.Instance.Player.Health.TakeDamage(100);
            }
            breath -= breathLostFactor * Time.deltaTime;

            //TODO: Update UI and sound
        }
        private void RefreshBreath()
        {
            breath = 100;
        }

        #endregion

    }

}

