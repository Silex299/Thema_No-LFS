using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace NPC
{
    [RequireComponent(typeof(Collider))]
    public class AIVision : AISense
    {
        protected bool _playerInRange;

        [SerializeField, Space(10)] protected float secondRaycastOffset = 1f;

        [SerializeField, BoxGroup("Event")] protected UnityEvent onTargetFound;
        [SerializeField, BoxGroup("Event")] protected UnityEvent onReset;

        private void Start()
        {
            #region Subscribe to events

            if (GameManager.instance)
            {
                GameManager.instance.OnGameOver += StopSensor;
            }

            #endregion

        }
        private void OnDisable()
        {
            #region Unsubscribe to events

            if (GameManager.instance)
            {
                GameManager.instance.OnGameOver -= StopSensor;
            }

            #endregion
        }

        //Set playerInRange to true if the target is inside the trigger volume
        protected virtual void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(targetTag) || isDisabled) return;

            _playerInRange = true;
        }

        //Set PlayerInRage to false if target leaves the trigger volume
        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(targetTag) || isDisabled) return;

            _playerInRange = false;
        }

        protected virtual void Update()
        {
            if (targetFound)
            {
                transform.LookAt(target);
            }
        }

        protected virtual void FixedUpdate()
        {
            if (!_playerInRange || isDisabled || targetFound) return;

            // Raycast to the target
            // If the path is not blocked by any obstacle, set targetFound to true

            if (Physics.Raycast(this.transform.position, target.position + new Vector3(0, 0.1f, 0) - this.transform.position, out RaycastHit hit))
            {
                //TODO REMOVE
                Debug.DrawLine(this.transform.position, hit.point, Color.cyan, 5f);

                if (hit.collider.CompareTag(targetTag))
                {
                    targetFound = true;
                    onTargetFound?.Invoke();
                }
            }

            if (!targetFound)
            {
                if (Physics.Raycast(this.transform.position, target.position + new Vector3(0, secondRaycastOffset, 0) - this.transform.position, out RaycastHit hit1))
                {
                    //TODO REMOVE
                    Debug.DrawLine(this.transform.position, hit1.point, Color.green, 5f);

                    if (hit1.collider.CompareTag(targetTag))
                    {
                        targetFound = true;
                        onTargetFound?.Invoke();
                    }
                }
            }


        }

        protected override void StopSensor()
        {
            base.StopSensor();
            onReset?.Invoke();
        }

    }
}
