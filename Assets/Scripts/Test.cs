
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Test:MonoBehaviour
    {
        private Vector3 pointOfContact;
        
        //set point of contact with the object of the my player with name tag Player_Main
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player_Main"))
            {
                pointOfContact = other.contacts[0].point;
            }
        }

        private IEnumerator EffectPlatform()
        {
            //Move/Rotate around the point of contact as if it is floating on water 
            while (true)
            {
                transform.position = new Vector3(pointOfContact.x + Mathf.Sin(Time.time), pointOfContact.y, pointOfContact.z + Mathf.Cos(Time.time));
                
                //Rotate sinusoidally the platform
                //Around the point of contact not around the pivot point
                transform.rotation = Quaternion.Euler(0, Mathf.Sin(Time.time) * 10, 0);
                
                
                yield return null;
            }
        }

    }
}