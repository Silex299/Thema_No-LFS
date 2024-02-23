using UnityEngine;


namespace NP_Interactions
{


    public class PlayerAvoidingInsect : MonoBehaviour
    {

        /// <summary>
        /// The minimum distance at where the insect feels safe
        /// </summary>
        public float safeDistance;
        /// <summary>
        /// minimum ditsance at where insect safe sense triggers, It Should be less than safe Distance
        /// </summary>
        public float reactDistance;

        public float insectMovementSpeed = 1;
        public float groundOffset;
        public LayerMask layerMask;


        [Space(10)] public Animator animator;

        private Vector3 _movePosition;
        private bool _isMoving;


        private void Start()
        {
            animator.Play("Insect Idle", 0, Random.Range(0f, 1f));
            GetGround();
        }
        public void UpdateInsect(Vector3 targetPosition)
        {

            //Calculate the Distance
            var targetPos = targetPosition;
            targetPos.y = 0;

            var insectPos = transform.position;
            insectPos.y = 0;

            var direction = insectPos - targetPos;
            var distance = direction.magnitude;




            //if distance is under react distance
            if (distance <= reactDistance)
            {
                _movePosition = direction.normalized;
                _isMoving = true;
            }

            //To make sure the insect is Grounded,
            //Raycast from a certain distance above the insect that only his the ground
            //Assign the y distance to the hit point with required offset


            //move the insect to safe distance in its direction from the player

            //else play idle

            Move(distance);
            if (animator)
            {
                animator.SetBool("isMoving", _isMoving);
            }


        }

        private void GetGround()
        {
            var raycastPos = transform.position + Vector3.up;

            if (Physics.Raycast(raycastPos, -Vector3.up, out RaycastHit hit, 4f, layerMask))
            {
                var newPos = transform.position;
                newPos.y = hit.point.y + groundOffset;

                transform.position = newPos;

            }

        }

        private void Move(float distance)
        {
            if (!_isMoving) return;
            if (distance > safeDistance) _isMoving = false;

            transform.position += _movePosition * insectMovementSpeed * Time.deltaTime;
            GetGround();
            Rotate();
        }

        private void Rotate()
        {
            if (!_isMoving) return;

            transform.forward = Vector3.MoveTowards(transform.forward, _movePosition, Time.deltaTime * 4f);

        }



    }



}
