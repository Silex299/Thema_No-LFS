using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    [SerializeField, BoxGroup("Reference")] private Transform target;

    [SerializeField, BoxGroup("Reference")] private Animator animator;
    [SerializeField, BoxGroup("Reference")] private Weapon.Rifle rifle;

    [SerializeField, BoxGroup("Sense Parameter")] private float eyeLevel = 1.5f;
    [SerializeField, BoxGroup("Sense Parameter")] private float patrolWaitTime = 1f;
    [SerializeField, BoxGroup("Sense Parameter")] private float stoppingDisatnce = 1f;
    [SerializeField, BoxGroup("Sense Parameter")] private float attackStoppingDistance = 4f;
    [SerializeField, BoxGroup("Sense Parameter")] private float accurateAttackDistance = 5f;
    [SerializeField, BoxGroup("Sense Parameter")] private float maximumAttackDistance = 10f;

    [SerializeField, BoxGroup("Movement Transforms")] private List<Transform> patrolPoints;
    [SerializeField, BoxGroup("Movement Transforms")] private List<Transform> chasePoints;

    [SerializeField, BoxGroup("Misc")] private float turnSmoothness;
    [SerializeField, BoxGroup("Misc")] private LayerMask layerMask;

    private int _currentPatrolIndex;

    [SerializeField, Space(10)] private GuardState state = GuardState.Patrol;
    private float _turnSmoothVelocity;


    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int isPatrolling = Animator.StringToHash("Is Patrolling");



    private void Start()
    {
        Player_Scripts.PlayerController.Instance.Player.Health.PlayerDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        Player_Scripts.PlayerController.Instance.Player.Health.PlayerDeath -= OnPlayerDeath;
    }

    private void Update()
    {
        if (state == GuardState.Patrol)
        {
            Patrol();
        }
        else if (state == GuardState.Chase)
        {
            Chase();
        }
        else if (state == GuardState.Action)
        {
            Action();
        }
    }


    private float _waitTime;
    private void Patrol()
    {
        animator.SetBool(isPatrolling, true);
        if (Mathf.Abs((patrolPoints[_currentPatrolIndex].position - transform.position).magnitude) < stoppingDisatnce)
        {
            animator.SetFloat(Speed, 0, 0.3f, Time.deltaTime);

            if (_waitTime == 0)
            {
                _waitTime = Time.time;
            }
            else if (Time.time > _waitTime + patrolWaitTime)
            {
                _waitTime = 0;
                _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Count;
            }
        }
        else
        {
            animator.SetFloat(Speed, 1, 0.3f, Time.deltaTime);
            RotatePlayer(patrolPoints[_currentPatrolIndex].position, 1);
        }


    }


    /// <summary>
    /// The time player can stay in guards firing range before it gets shot
    /// </summary>
    private float _inaccurateFireTime;
    /// <summary>
    /// For comparing previous postion to current position
    /// </summary>
    private Vector3 _delayedPosition;

    /// <summary>
    /// Called when the guard is in chase state
    /// </summary>
    private void Chase()
    {
        if (!target)
        {
            state = GuardState.Patrol;
            return;
        }

        RotatePlayer(GetPathToPlayer(out bool playerInSight, out bool chase), 1);
           
        var distance = Vector3.Distance(target.position, transform.position);

        animator.SetBool(isPatrolling, false);

        //If player is in guards line of sight
        if (playerInSight)
        {

            //If Distance between guard and player is more than guards shooting range
            //The Guard doesn't fire and run towards the player
            if (distance > maximumAttackDistance)
            {
                //No Attack
                if (rifle.Firing)
                {
                    rifle.FireWeapon(false, Vector3.zero);
                }
                animator.SetFloat(Speed, 2, 0.3f, Time.deltaTime);
            }

            //If the player is within the shooting range but is not in accurate shoting range
            // The guard fires accurately if player is stationary for more than 2 sec
            // otherwise it fires inaccurately

            else if (distance > accurateAttackDistance)
            {
                //Inaccurate Attack

                if (_inaccurateFireTime == 0)
                {
                    _inaccurateFireTime = Time.time;
                    _delayedPosition = target.position;
                }

                if (Time.time > _inaccurateFireTime + 2f)
                {
                    _delayedPosition = target.position;
                    _inaccurateFireTime = Time.time;
                }


                if (Vector3.Distance(target.position, _delayedPosition) < 1f)
                {
                    rifle.FireWeapon(true, target.position + 1f * Vector3.up);
                }
                else
                {
                    rifle.FireWeapon(true, target.position - new Vector3(Random.Range(-1f, 1f), Random.Range(0.2f, 1f), Random.Range(-0.5f, 0.5f)));
                }

                animator.SetFloat(Speed, 1, 0.3f, Time.deltaTime);

            }
            
            //If the player is within guards accurate attacking range
            //The guard shoots down the player
            else if (distance < accurateAttackDistance && distance > attackStoppingDistance)
            {
                rifle.FireWeapon(true, target.position + 1f * Vector3.up);
                //Accurate Attack
            }

            //If the player is within stopping range of the guard 
            //The guard stops moving but still fires its weapon
            else
            {
                rifle.FireWeapon(true, target.position + 1f * Vector3.up);
                animator.SetFloat(Speed, 0, 0.3f, Time.deltaTime);
            }

        }
        //if not
        else
        {
            // If weapon is firing then stop firing;
            if (rifle.Firing)
            { 
                rifle.FireWeapon(false, Vector3.zero);
            }
            if (chase)
            {
                animator.SetFloat(Speed, 2, 0.3f, Time.deltaTime);
            }
            else
            {
                animator.SetFloat(Speed, 0, 0.3f, Time.deltaTime);
            }
        }


    }



    private void Action()
    {
        rifle.FireWeapon(false, Vector3.zero);
        //animator.SetBool(isPatrolling, true);
        animator.SetFloat(Speed, 0, 0.3f, Time.deltaTime);
    }

    /// <summary>
    /// Finds guards path to player using chase points
    /// </summary>
    private Vector3 GetPathToPlayer(out bool playerInSight, out bool chase)
    {
        playerInSight = false;
        chase = true;


        //TODO: REMOVE DEBUG

        var raycastPosition = transform.position + Vector3.up * eyeLevel;

        if (Physics.Raycast(raycastPosition, ((target.position + new Vector3(0, 0.4f, 0)) - raycastPosition), out RaycastHit hit, Mathf.Infinity, layerMask))
        {

            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Player_Main"))
            {
                Debug.DrawLine(raycastPosition, hit.point, Color.green);
                playerInSight = true;
            }
            else if (Physics.Raycast(raycastPosition, ((target.position + new Vector3(0, 1.3f, 0)) - raycastPosition), out RaycastHit hit1, Mathf.Infinity, layerMask))
            {
                if (hit1.collider.CompareTag("Player") || hit.collider.CompareTag("Player_Main"))
                {
                    playerInSight = true;
                    Debug.DrawLine(raycastPosition, hit.point, Color.red);
                    Debug.DrawLine(raycastPosition, hit1.point, Color.green);
                }
                else
                {
                    playerInSight = false;

                    Debug.DrawLine(raycastPosition, hit.point, Color.red);
                    Debug.DrawLine(raycastPosition, hit1.point, Color.red);
                }
            }
        }
        else if (Physics.Raycast(raycastPosition, ((target.position + new Vector3(0, 1f, 0)) - raycastPosition), out RaycastHit hit1, Mathf.Infinity, layerMask))
        {
            if (hit1.collider.CompareTag("Player") || hit.collider.CompareTag("Player_Main"))
            {
                playerInSight = true;
                Debug.DrawLine(raycastPosition, hit1.point, Color.green);
            }
            else
            {
                Debug.DrawLine(raycastPosition, hit1.point, Color.red);
                Debug.DrawLine(raycastPosition, hit.point, Color.red);
                playerInSight = false;
            }
        }




        if (playerInSight)
        {
            return target.position;
        }
        else
        {
            float minimumDistance = Mathf.Infinity;
            int minimumDistanceIndex = 0;
            int i = 0;

            foreach (var point in chasePoints)
            {
                var distance = Vector3.Distance(point.position, target.position);

                if (distance < minimumDistance)
                {

                    if (!Physics.Linecast(transform.position + Vector3.up * eyeLevel, point.position, layerMask) && !Physics.Linecast(target.position + 1f * Vector3.up, point.position, layerMask))
                    {
                        minimumDistance = distance;
                        minimumDistanceIndex = i;
                    }
                }

                i++;
            }

            if (minimumDistance == Mathf.Infinity)
            {
                chase = false;
                return transform.position;
            }

            Debug.DrawLine(transform.position + Vector3.up * eyeLevel, chasePoints[minimumDistanceIndex].position, Color.blue);
            Debug.DrawLine(target.position + 1f * Vector3.up, chasePoints[minimumDistanceIndex].position, Color.blue);


            return chasePoints[minimumDistanceIndex].position;

        }
    }

    private void RotatePlayer(Vector3 targetPosition, float multiplier)
    {
        Vector3 direction = targetPosition - this.transform.position;

        float tangentAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, tangentAngle, ref _turnSmoothVelocity, turnSmoothness * multiplier * Time.deltaTime);

        this.transform.rotation = Quaternion.Euler(0, angle, 0);
    }


    private void OnPlayerDeath()
    {
        if (state != GuardState.Patrol)
        {
            state = GuardState.Action;
        }
    }

    private void OnDrawGizmos()
    {

        GetPathToPlayer(out bool playerInSight, out bool chase);

        Gizmos.color = new Color(0.8f, 0, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, accurateAttackDistance);
        Gizmos.color = new Color(0, 0.8f, 0, 0.1f);
        Gizmos.DrawSphere(transform.position, maximumAttackDistance);
        return;



    }

    public void ChangeState(int index)
    {

        switch (index)
        {
            case 0:
                state = GuardState.Patrol;
                break;
            case 1:
                state = GuardState.Chase;
                break;
            case 2:
                state = GuardState.Action;
                break;
        }

    }

    public enum GuardState
    {
        Patrol,
        Chase,
        Action
    }

}
