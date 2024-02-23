using System.Collections;
using System.Collections.Generic;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;

namespace NPC.Patrolling_Guards
{
    public class PatrollingGuard : MonoBehaviour
    {

        #region Exposed Variables

        [SerializeField, FoldoutGroup("References")] private Animator animationController;
        [SerializeField, FoldoutGroup("References")] private AISense sensor;
        [SerializeField, FoldoutGroup("References")] private AudioSource audioSource;


        [SerializeField, FoldoutGroup("Constant Positions")] private List<Transform> patrolPoints;
        [SerializeField, FoldoutGroup("Constant Positions")] private List<Transform> chasePoints;


        [SerializeField, FoldoutGroup("Chase Variables")] private float stoppingDistance;
        [SerializeField, FoldoutGroup("Chase Variables")] private float fireDistance;

        [SerializeField, FoldoutGroup("Action Variables")] private float fireRate = 60;
        [SerializeField, FoldoutGroup("Action Variables")] private int maximumBulletsPerMagazine = 30;
        [SerializeField, FoldoutGroup("Action Variables")] private float reloadDelay = 1f;
        [SerializeField, FoldoutGroup("Action Variables")] private VisualEffect muzzleFlash;
        [SerializeField, FoldoutGroup("Action Variables")] private AudioClip fireSound;

        [SerializeField, FoldoutGroup("Misc")] private bool stopAtDestination;
        [SerializeField, FoldoutGroup("Misc")] private float characterRotationSmoothness = 0.1f;
        [SerializeField, FoldoutGroup("Misc")] private float secondRaycastOffset = 0.5f;



        #endregion

        #region Non Exposed Variables

        private bool _playerIsInSight;

        private int _currentPatrollingIndex;
        private float _turnSmoothVelocity;

        private bool _changePatrolPoint = true;



        private float _lastFireTime;
        private int _bullets = 30;

        private static readonly int IsPatrolling = Animator.StringToHash("Is Patrolling");
        private static readonly int Speed = Animator.StringToHash("Speed");

        #endregion

        #region Builtin methods

        private void Start()
        {
            _bullets = maximumBulletsPerMagazine;
        }

        private void Update()
        {
            //Target Found
            if (sensor.targetFound && sensor.target)
            {


                //Change animation state
                if (animationController.GetBool(IsPatrolling))
                {
                    animationController.SetBool(IsPatrolling, false);
                }


                #region Check if player is in guard's sight

                if (Physics.Linecast(this.transform.position, sensor.target.position + new Vector3(0, 0.05f, 0), out RaycastHit hit))
                {
                    _playerIsInSight = hit.collider.CompareTag("Player");

                    //TODO REMOVE
                    Debug.DrawLine(this.transform.position, hit.point, Color.green, 0.1f);
                }
                else if (Physics.Linecast(this.transform.position, sensor.target.position + new Vector3(0, secondRaycastOffset, 0), out RaycastHit hit1))
                {
                    _playerIsInSight = hit1.collider.CompareTag("Player");

                    //TODO REMOVE
                    Debug.DrawLine(this.transform.position, hit1.point, Color.green, 0.1f);
                }
                else
                {
                    _playerIsInSight = false;
                }

                #endregion


                if (_playerIsInSight)
                {
                    float distance = Mathf.Abs((sensor.target.position - this.transform.position).magnitude);

                    //If guard is beyond fire distance run to the target
                    if (distance > fireDistance)
                    {
                        animationController.SetFloat(Speed, 2, 0.2f, Time.deltaTime);
                    }
                    //If guard is in shooting range then slow walk and shoot
                    else if (distance > stoppingDistance)
                    {
                        animationController.SetFloat(Speed, 1, 0.2f, Time.deltaTime);
                        Fire();
                    }
                    //If guard is in stopping range then stop and fire
                    else
                    {
                        animationController.SetFloat(Speed, 0, 0.2f, Time.deltaTime);
                        Fire();
                    }


                    //Rotate Guard
                    RotateCharacter(sensor.target.position, 0.1f);
                }

                //If player is not in sight
                else
                {
                    animationController.SetFloat(Speed, 2, 0.2f, Time.deltaTime);

                    //Go to already defined chase points that is best for chasing the player; 

                    RotateCharacter(chasePoints[GetPointToMoveTo()].position, 0.1f);
                }

            }

            //Patrolling
            else
            {
                RotateCharacter(patrolPoints[_currentPatrollingIndex].position);

                if (!animationController.GetBool(IsPatrolling))
                {
                    animationController.SetBool(IsPatrolling, true);
                }


                float distance = Mathf.Clamp(Mathf.Abs((this.transform.position - patrolPoints[_currentPatrollingIndex].position).magnitude), 0, 1);

                //If stop at destination is disabled
                if (!stopAtDestination)
                {
                    if (distance < 0.7f)
                    {
                        _currentPatrollingIndex = (_currentPatrollingIndex + 1) % patrolPoints.Count;
                        return;
                    }
                }

                //If stop at destination is enabled
                if (distance < 0.7f)
                {
                    if (_changePatrolPoint)
                    {
                        StartCoroutine(ChangePatrolPoint());
                    }

                    animationController.SetFloat(Speed, 0, 0.5f, Time.deltaTime);

                }
                else
                {
                    animationController.SetFloat(Speed, 1, 0.5f, Time.deltaTime);
                }


            }

        }

        #endregion

        #region Custom methods

        /// <summary>
        /// Changes patrol point after a delay
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChangePatrolPoint()
        {
            _changePatrolPoint = false;
            yield return new WaitForSeconds(2f);
            _currentPatrollingIndex = (_currentPatrollingIndex + 1) % patrolPoints.Count;
            _changePatrolPoint = true;
        }

        /// <summary>
        /// Fires weapon
        /// </summary>
        private void Fire()
        {
            //Check if angle is less than 10 degrees
            Transform transform1 = this.transform;
            if (Vector3.Angle(sensor.target.position - transform1.position, transform1.forward) > 15) return;


            if (_lastFireTime > Time.time) return;

            if (_bullets == 0) _bullets = maximumBulletsPerMagazine;

            _bullets--;

            if (PlayerController.Instance)
            {
                PlayerController.Instance.Player.Health.TakeDamage(50);
            }

            _lastFireTime = Time.time + (60 / fireRate);

            if (_bullets == 0) _lastFireTime += reloadDelay;

            audioSource.PlayOneShot(fireSound);
            muzzleFlash.Play();

        }

        /// <summary>
        /// Rotates the guard in desired Direction
        /// </summary>
        /// <param name="position"></param>
        /// <param name="multiplier"></param>
        private void RotateCharacter(Vector3 position, float multiplier = 1)
        {

            Vector3 direction = position - this.transform.position;

            float tangentAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, tangentAngle, ref _turnSmoothVelocity, characterRotationSmoothness * multiplier);

            this.transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        /// <summary>
        /// Finds the better point to move to for the guard
        /// </summary>
        /// <returns></returns>
        private int GetPointToMoveTo()
        {
            float chaseDistance = 1000;
            int index = 0;
            //Loop through all the chase point and find which is the better option to move to

            for (int i = 0; i < chasePoints.Count; i++)
            {
                float distance = Mathf.Abs((chasePoints[i].position - sensor.target.position).magnitude);

                if (distance < chaseDistance)
                {
                    //If there is some obstacle between guard and chase point, then ignore it
                    if (Physics.Linecast(this.transform.position, chasePoints[i].position))
                    {
                        continue;

                    }

                    //If you can't see the player from selected chase point, then ignore it
                    if (Physics.Linecast(chasePoints[i].position, sensor.target.position + new Vector3(0, 0.2f, 0), out RaycastHit hit3))
                    {
                        if (!hit3.collider.CompareTag("Player"))
                        {
                            continue;
                        }
                        //TODO REMOVE
                        Debug.DrawLine(chasePoints[i].position, hit3.point, Color.yellow, 0.2f);
                    }

                    chaseDistance = distance;
                    index = i;
                }
            }

            return index;
        }


        #endregion




    }
}
