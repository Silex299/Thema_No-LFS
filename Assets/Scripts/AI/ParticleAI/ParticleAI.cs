using System.Collections;
using UnityEngine;

namespace AI.ParticleAI
{
    public class ParticleAI : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float speed;
        [SerializeField] private float avoidObjectDistance = 2f;




        [SerializeField] private Transform target;
        private Transform avoidObject;


        [SerializeField] private ParticleState _particleState;


        private static readonly int State = Animator.StringToHash("state");



        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Divine_Light"))
            {
                avoidObject = other.transform;
            }
        }


        private void Update()
        {


            switch (_particleState)
            {
                case ParticleState.Chase:
                    Chase();
                    break;
                case ParticleState.Attack:
                    Attack();
                    break;
                case ParticleState.Avoid:
                    AvoidLight();
                    break;
                case ParticleState.End:
                    End();
                    break;
                default:
                    break;
            }
        }



        private void Chase()
        {
            if (_particleState != ParticleState.Chase)
            {
                _particleState = ParticleState.Chase;
                animator.SetInteger(State, 1);
            }

            if (target)
            {


                if (avoidObject)
                {
                    if (Vector3.Distance(avoidObject.position, transform.position) < avoidObjectDistance)
                    {
                        AvoidLight();
                    }
                }

                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);

                if (Vector3.Distance(transform.position, target.position) < 0.3f)
                {
                    if (avoidObject)
                    {
                        if(Vector3.Distance(avoidObject.position, transform.position) > avoidObjectDistance)
                        {
                            Attack();
                        }
                    }
                    else
                    {
                        Attack();
                    }
                }

            }

        }

        private Vector3 _avoidPos;
        private void AvoidLight()
        {

            if (!avoidObject)
            {
                Chase();
                return;
            }


            if(_particleState!= ParticleState.Avoid)
            {
                _particleState = ParticleState.Avoid;

                var direction = (transform.position - avoidObject.position).normalized;
                _avoidPos = avoidObject.position + direction * avoidObjectDistance;
            }

            var distance = Vector3.Distance(avoidObject.position, transform.position);
            if (distance < avoidObjectDistance-1)
            {
                var direction = (transform.position - avoidObject.position).normalized;
                _avoidPos = avoidObject.position + direction * avoidObjectDistance;
            }
            else if(distance>avoidObjectDistance + 1)
            {
                Chase();
                return;
            }


            transform.position = Vector3.Lerp(transform.position, _avoidPos, Time.deltaTime * speed);
        }
        private void Attack()
        {
            if (_particleState != ParticleState.Attack)
            {
                _particleState = ParticleState.Attack;
                animator.SetInteger(State, 2);
                Player_Scripts.PlayerController.Instance.Player.Health.DeathByParticles();
                End();
            }
        }
        private void End()
        {
            if (_particleState != ParticleState.End)
            {
                _particleState = ParticleState.End;
                //animator.SetInteger(State, 1);
            }

            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * speed);
        }

        public void DisableParticle()
        {
            _particleState = ParticleState.Idle;
            StartCoroutine(StopParticle());
        }

        private IEnumerator StopParticle()
        {
            GetComponentInChildren<UnityEngine.VFX.VisualEffect>().Stop();

            yield return new WaitForSeconds(10f);

            Destroy(this.gameObject);
        }

        [System.Serializable]
        public enum ParticleState
        {
            Idle,
            Chase,
            Avoid,
            Attack,
            End
        }


    }
}