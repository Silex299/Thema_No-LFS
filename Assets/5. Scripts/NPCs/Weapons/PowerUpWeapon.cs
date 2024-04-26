using System.Collections.Specialized;
using Player_Scripts;
using UnityEngine;


// ReSharper disable once CheckNamespace
namespace NPCs.Weapons
{
    public class PowerUpWeapon : MonoBehaviour
    {

        public void LookForPlayer(Transform target)
        {
            //look at the target, but looking speed at 3
            var transform1 = transform;
            transform.rotation = Quaternion.Slerp(transform1.rotation, Quaternion.LookRotation(target.position - transform1.position), 1f * Time.deltaTime);
        }
    }
}


