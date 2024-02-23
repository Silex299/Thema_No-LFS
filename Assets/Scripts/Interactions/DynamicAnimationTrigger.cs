using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Interactions
{
    public class DynamicAnimationTrigger : MonoBehaviour
    {
        [SerializeField, BoxGroup] private string animationName;
        [SerializeField, BoxGroup] private float movementSmoothness;
        [SerializeField, BoxGroup] private float rotationSmoothness;
        [SerializeField, BoxGroup] private float initialDelay;
        [SerializeField, BoxGroup] private float endingDelay;


        private Transform _target;
        private bool _movePlayer;

        private void LateUpdate()
        {
            if (!_movePlayer) return;

            var position = _target.position;
            var position1 = transform.position;

            _target.position = Vector3.MoveTowards(position, position1, Time.deltaTime * movementSmoothness);

            _target.rotation = Quaternion.RotateTowards(_target.rotation, transform.rotation, Time.deltaTime * rotationSmoothness);

            //TODO REMOVE
            var distance = Mathf.Abs((position - position1).magnitude) < 0.001f;
            var rotation = Mathf.Abs((_target.eulerAngles - transform.eulerAngles).magnitude) < 0.01f;
            _movePlayer = !(distance && rotation);

        }

        public void TriggerAnimation(bool moveWithAnimation)
        {
            var playerController = PlayerController.Instance;
            _target = playerController.transform;
            playerController.Player.canRotate = false;

            if (!moveWithAnimation) StartCoroutine(Trigger_1(playerController));
            else StartCoroutine(Trigger_2(playerController));

        }

        private IEnumerator Trigger_1(PlayerController controller)
        {
            yield return new WaitForSeconds(initialDelay);
            _movePlayer = true;
            yield return new WaitUntil(() => !_movePlayer);

            controller.Player.AnimationController.CrossFade(animationName, 0.2f, 1);
        }

        private IEnumerator Trigger_2(PlayerController controller)
        {
            controller.CrossFadeAnimation(animationName);

            yield return new WaitForSeconds(initialDelay);
            _movePlayer = true;

            yield return new WaitUntil(() => !_movePlayer);

            _movePlayer = false;
        }
    }
}
