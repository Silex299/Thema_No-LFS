using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NPC.LightBot
{
    public class LightBot : MonoBehaviour
    {

        [SerializeField, BoxGroup("Reference")] private Animator animator;
        [SerializeField, BoxGroup("Reference")] private new Light light;


        [SerializeField, BoxGroup("Follow Properties")] private Vector3 followOffset;
        [SerializeField, BoxGroup("Follow Properties")] private Transform target;
        [SerializeField, BoxGroup("Follow Properties")] private float followSmoothness;
        [SerializeField, BoxGroup("Follow Properties")] private float followDelay = 2f;
        [SerializeField, BoxGroup("Follow Properties")] private float collisionOffset = 1f;

        [SerializeField, BoxGroup("Other Properties")] private float maximumLightIntensity = 3;
        
        

        private bool _followTarget;
        private bool _isActive = true;

        private IEnumerator ActiveTargetFollow()
        {
            yield return new WaitForSeconds(followDelay);
            _followTarget = true;
        }


        private void Start()
        {
            target = PlayerController.Instance.transform;
            StartCoroutine(ActiveTargetFollow());
        }

        private void Update()
        {
            if(!_followTarget) return;

            if (Input.GetButtonDown("Interaction_3"))
            {
                _isActive = !_isActive;
                
                if (_isActive)
                {
                    Shine();
                }
                else
                {
                    Shrink();
                }
            }

            if (_isActive && light.intensity < 3)
            {
                light.intensity = Mathf.Lerp(light.intensity, maximumLightIntensity, Time.deltaTime * 10f);
            }
            else if (!_isActive && light.intensity > 0)
            {
                light.intensity = Mathf.Lerp(light.intensity, 0, Time.deltaTime * 10f);
            }
            
            
            Vector3 newPos = target.transform.position + followOffset;
            
            if(Physics.Linecast(target.position + new Vector3(0,1.5f, 0), newPos, out  RaycastHit hit, ~(1 << LayerMask.NameToLayer("Player") << LayerMask.NameToLayer("Ignore Raycast"))))
            {
                newPos = hit.point + (target.position + new Vector3(0, 1.5f, 0) - newPos) * collisionOffset;
            }
            
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * followSmoothness);
            
        }

        private void Shrink()
        {
            animator.Play("Disappear");
        }

        private void Shine()
        {
            animator.Play("Appear");
        }
        
        
        
    }
}
