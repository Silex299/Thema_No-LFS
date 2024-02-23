using System;
using UnityEngine;

namespace NPC
{
    public class AISense : MonoBehaviour
    {
        public string targetTag = "Player";

        public Transform target;
        public bool targetFound;
        protected bool isDisabled;
        
        //Disable the sensor
        protected virtual void StopSensor()
        {
            targetFound = false;
            isDisabled = true;
        }
    }
}
