using System.Collections;
using Player_Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons
{
    public class ProjectileShooter : MonoBehaviour
    {


        [Required] public GameObject projectilePrefab;
        public float projectileSpeed = 10f;
        public float projectileLifeTime = 2f;
        public Vector3 rotationOffset;
        public Vector3 fireError;

        private Coroutine _alignmentCoroutine;
        [field: SerializeField] public bool CanShoot { get; set; } = true;



        [Button]
        public void Shoot()
        {
            if(!CanShoot) return;
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            
            Vector3 randomError = new Vector3(Random.Range(-fireError.x, fireError.x), Random.Range(-fireError.y, fireError.y), Random.Range(-fireError.z, fireError.z));
            
            projectile.GetComponent<Rigidbody>().velocity = (projectile.transform.forward + randomError) * projectileSpeed;
            
            projectile.transform.rotation *= Quaternion.Euler(rotationOffset);
            
            Destroy(projectile, projectileLifeTime);
        }


        public void AlignAndShoot(float alignmentTime = 0)
        {
            if(!CanShoot) return;
            if (_alignmentCoroutine != null)
            {
                StopCoroutine(_alignmentCoroutine);
            }

            _alignmentCoroutine = StartCoroutine(Align(PlayerMovementController.Instance.transform.position, alignmentTime));
        }

        public void AlignAndShoot(Transform target, float alignmentTime = 0)
        {
            if(!CanShoot) return;
            if (_alignmentCoroutine != null)
            {
                StopCoroutine(_alignmentCoroutine);
            }

            _alignmentCoroutine = StartCoroutine(Align(target.position, alignmentTime));
        }

        private IEnumerator Align(Vector3 targetPos, float alignmentTime)
        {

            Vector3 intiForward = transform.forward;
            Vector3 direction = targetPos - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float elapsedTime = 0f;
            
            while (elapsedTime < alignmentTime)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / alignmentTime);
                yield return null;
            }
            
            transform.rotation = targetRotation;
            Shoot();
        }
    }
}
