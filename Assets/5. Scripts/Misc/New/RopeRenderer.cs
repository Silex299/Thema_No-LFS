using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc.New
{
    public class RopeRenderer : MonoBehaviour
    {
        public Transform[] segments;
        public LineRenderer[] lineRenderers;
        public float ropeWidth;
        public Material ropeMaterial;

#if UNITY_EDITOR


        [Button]
        public void CreateRenderer()
        {
            //remove previous line renderers
            if (lineRenderers != null)
            {
                foreach (var t in lineRenderers)
                {
                    DestroyImmediate(t);
                }
            }

            //create a line renderer for each segments other than the first one
            lineRenderers = new LineRenderer[segments.Length - 1];

            for (int i = 0; i < segments.Length - 1; i++)
            {
                lineRenderers[i] = segments[i].gameObject.AddComponent<LineRenderer>();
                lineRenderers[i].positionCount = 2;

                lineRenderers[i].SetPosition(0, segments[i].position);
                lineRenderers[i].SetPosition(1, segments[i + 1].position);

                lineRenderers[i].startWidth = ropeWidth;
                lineRenderers[i].endWidth = ropeWidth;
                lineRenderers[i].material = ropeMaterial;
            }
        }

#endif

        private void Update()
        {
            UpdateRopePositions();
        }

        private void UpdateRopePositions()
        {
            for (int i = 0; i < segments.Length - 1; i++)
            {
                lineRenderers[i].SetPosition(0, segments[i].position);
                lineRenderers[i].SetPosition(1, segments[i + 1].position);
            }
        }

        [Button]
        public void BreakRope(int segmentIndex)
        {
            lineRenderers[segmentIndex].enabled = false;
        }

        public void Reset()
        {
            foreach (var t in lineRenderers)
            {
                t.enabled = true;
            }
        }
    }
}