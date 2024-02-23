using System;
using UnityEngine;

namespace NPC
{
    public class AIVolumeSense : AISense
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(targetTag)) return;
            targetFound = true;
            target = other.transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(targetTag)) return;
            targetFound = false;
        }
    }
}
