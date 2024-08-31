using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Mechanics.Npc
{
    public class ProximityDetection : MonoBehaviour
    {
        
        
        [BoxGroup] public float proximityThreshold;
        [BoxGroup] public float height;
        [BoxGroup] public float heightOffset;
        [BoxGroup] public int castResolution;
        [BoxGroup] public LayerMask layerMask;
        
        [BoxGroup, Space(5)] public bool checkFront = true;
        [BoxGroup] public bool checkSides;
        
        [BoxGroup, Space(5)] public ProximityFlags proximityFlag = ProximityFlags.None;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.TransformPoint(0, height, 0));

            if (!Application.isPlaying)
            {
                FrontProximity();
                LeftProximity();
                RightProximity();
            }
            
        }


        private void FixedUpdate()
        {
            if(checkFront) FrontProximity();
            if(checkSides) LeftProximity();
            if(checkSides) RightProximity();
        }

        private void FrontProximity()
        {
            Vector3 direction = transform.forward;
            Vector3 origin = transform.position + transform.up * heightOffset;
            float step = (height - heightOffset) / castResolution;


            //Raycast in forward direction with step intervals from the origin
            for (int i = 0; i < castResolution; i++)
            {
                origin += transform.up * step;
                if (Physics.Raycast(origin, direction, out var hit, proximityThreshold, layerMask))
                {
                    Debug.DrawLine(origin, hit.point, Color.red);
                    proximityFlag = proximityFlag | ProximityFlags.Front;
                    break;
                }
                else
                {
                    Debug.DrawLine(origin, origin + direction * proximityThreshold, Color.green);
                    if (i == castResolution - 1)
                    { 
                        proximityFlag &= ~ProximityFlags.Front;
                    }
                }
            }
            
        }
        private void LeftProximity()
        {
            Vector3 direction = -transform.right;
            Vector3 origin = transform.position + transform.up * heightOffset;
            float step = (height - heightOffset) / castResolution;

            //Raycast in forward direction with step intervals from the origin
            for (int i = 0; i < castResolution; i++)
            {
                origin += transform.up * step;
                if (Physics.Raycast(origin, direction, out var hit, proximityThreshold, layerMask))
                {
                    Debug.DrawLine(origin, hit.point, Color.red);
                    proximityFlag = proximityFlag | ProximityFlags.Left;
                    break;
                }
                else
                {
                    Debug.DrawLine(origin, origin + direction * proximityThreshold, Color.green);
                    if (i == castResolution - 1)
                    {
                        proximityFlag &= ~ProximityFlags.Left;
                    }
                }
            }
        }
        private void RightProximity()
        {
            Vector3 direction = transform.right;
            Vector3 origin = transform.position + transform.up * heightOffset;
            float step = (height - heightOffset) / castResolution;

            //Raycast in forward direction with step intervals from the origin
            for (int i = 0; i < castResolution; i++)
            {
                origin += transform.up * step;
                if (Physics.Raycast(origin, direction, out var hit, proximityThreshold, layerMask))
                {
                    Debug.DrawLine(origin, hit.point, Color.red);
                    proximityFlag = proximityFlag | ProximityFlags.Right;
                    break;
                }
                else
                {
                    Debug.DrawLine(origin, origin + direction * proximityThreshold, Color.green);
                    if (i == castResolution - 1)
                    {
                        proximityFlag &= ~ProximityFlags.Right;
                    }
                }
            }
        }

        [Flags][System.Serializable]
        public enum ProximityFlags
        {
            None = 0b_0000, //0
            Front = 0b_1000, //8
            Back = 0b_0100, //4
            Left = 0b_0010, //2
            Right = 0b_0001, //1
        }
    }
}