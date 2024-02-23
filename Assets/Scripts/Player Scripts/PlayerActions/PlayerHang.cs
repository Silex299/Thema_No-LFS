using System.Collections;
using UnityEngine;

namespace Player_Scripts.Player_Actions
{
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerHang : MonoBehaviour
	{
    	
        private readonly static int _jump = Animator.StringToHash("Jump");
        [SerializeField] private Transform movePlayerTo;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;

        [SerializeField] private bool _actOnPlayer;
        [SerializeField]private bool _movePlayer;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {

	            var angle = Quaternion.Angle(movePlayerTo.rotation, PlayerController.Instance.transform.rotation);
	            
	            if(angle<30)
	            {
		            _actOnPlayer = true;
		            _movePlayer = true;
		            PlayerController.Instance.CrossFadeAnimation("Jump To Hang", 0.3f);
		            PlayerController.Instance.Player.PlayerController.enabled = false;
	            }

            }

        }
        

        private void Update()
        {
            if (!_actOnPlayer) return;



            if (!_movePlayer)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    StartCoroutine(Jump());
                }

                return;
            }

            var targetPos = PlayerController.Instance.transform.position;
            var destinationPos = movePlayerTo.position;

            var targetRot = PlayerController.Instance.transform.rotation;
            var destinationRot = movePlayerTo.rotation;



            if (Vector3.Distance(targetPos, destinationPos) < 0.001f)
            {
                if (Quaternion.Angle(targetRot, destinationRot) < 1f)
                {
	                _movePlayer = false;
                }
            }


            PlayerController.Instance.transform.position = Vector3.MoveTowards(targetPos, destinationPos, Time.deltaTime * movementSpeed);
            PlayerController.Instance.transform.rotation = Quaternion.RotateTowards(targetRot, destinationRot, Time.deltaTime * rotationSpeed);


        }


        private IEnumerator Jump()
        {
	        PlayerController.Instance.Player.AnimationController.SetTrigger(_jump);
	        _actOnPlayer =  false;

            yield return new WaitForSeconds(1f);

            PlayerController.Instance.Player.AnimationController.ResetTrigger(_jump);
        }


        private IEnumerator HangPlayer()
        {

            yield break;

        }

    }
}