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



        [Button]
        public void Shoot()
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            
            
            Vector3 randomError = new Vector3(Random.Range(-fireError.x, fireError.x), Random.Range(-fireError.y, fireError.y), Random.Range(-fireError.z, fireError.z));
            
            projectile.GetComponent<Rigidbody>().velocity = (projectile.transform.forward + randomError) * projectileSpeed;
            
            projectile.transform.rotation *= Quaternion.Euler(rotationOffset);
            
            Destroy(projectile, projectileLifeTime);
        }

    }
}
