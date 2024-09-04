using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc.Items
{
    public class SimpleRopeFollower : MonoBehaviour
    {
        [FoldoutGroup("Rope property")] public int ropeResolution = 10;

        [FoldoutGroup("Reference")] public LineRenderer[] lineRenderers;
        [FoldoutGroup("Reference")] public Rigidbody[] ropeSegments;

        [FoldoutGroup("Other")] public int breakIndex;
        [FoldoutGroup("Other")] public Vector3 breakForce;


        private bool _broken = false;
        
        #region Editor

#if UNITY_EDITOR


        [FoldoutGroup("Rope property")] public float ropeLength;
        [FoldoutGroup("Rope property")] public float ropeThickness;
        [FoldoutGroup("Rope property")] public Material ropeMaterial;

        [Button("Create Rope", ButtonSizes.Large), GUIColor(1, 0.3f, 0.3f)]
        public void CreateRopeSegments()
        {
            //delete all the children of the current object
            foreach (var segment in ropeSegments)
            {
                DestroyImmediate(segment.gameObject);
            }


            ropeSegments = new Rigidbody[ropeResolution];

            for (int i = 0; i < ropeResolution; i++)
            {
                //instantiate gameObject with current object being the parent
                GameObject obj = new GameObject(i.ToString())
                {
                    transform =
                    {
                        parent = transform,
                        position = new Vector3(transform.position.x,
                            transform.position.y - i * (ropeLength) / (ropeResolution - 1), transform.position.z)
                    },
                    //Set object layer to Ignore Player
                    layer = LayerMask.NameToLayer("Rope")
                };

                ropeSegments[i] = obj.AddComponent<Rigidbody>();


                if (i > 0)
                {
                    #region Hinge Joint

                    CapsuleCollider addedCollider = ropeSegments[i].gameObject.AddComponent<CapsuleCollider>();
                    addedCollider.center = new Vector3(0, ((ropeLength / (ropeResolution - 1)) - 0.1f) / 2, 0);
                    addedCollider.height = (ropeLength / (ropeResolution - 1)) - 0.1f;
                    addedCollider.radius = ropeThickness / 2;

                    HingeJoint joint = ropeSegments[i].gameObject.AddComponent<HingeJoint>();

                    joint.anchor = new Vector3(0, ropeLength / (ropeResolution - 1), 0);
                    joint.connectedBody = ropeSegments[i - 1].GetComponent<Rigidbody>();

                    #endregion
                }
                else
                {
                    ropeSegments[i].gameObject.AddComponent<FixedJoint>();
                }
            }
        }

        [Button("Create LineRenderer")]
        public void CreateLineRenderer()
        {
            lineRenderers = new LineRenderer[ropeResolution];

            for (int i = 1; i < ropeResolution; i++)
            {
                if (!ropeSegments[i].TryGetComponent<LineRenderer>(out lineRenderers[i]))
                {
                    lineRenderers[i] = ropeSegments[i].gameObject.AddComponent<LineRenderer>();
                }

                lineRenderers[i].startWidth = ropeThickness;
                lineRenderers[i].endWidth = ropeThickness;
                lineRenderers[i].material = ropeMaterial;
                lineRenderers[i].textureMode = LineTextureMode.Tile;
                lineRenderers[i].positionCount = 2;

                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }

#endif

        #endregion

        
        private void Update()
        {
            //for each line render segment set the 2nd position to the previous segment position and 1st position to current segment position
            for (int i = 1; i < ropeResolution; i++)
            {
                if (_broken && i == breakIndex)
                {
                    continue;
                }

                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }

        [Button]
        public void BreakRope()
        {
            Destroy(ropeSegments[breakIndex].GetComponent<HingeJoint>());
            Destroy(lineRenderers[breakIndex]);

            ropeSegments[breakIndex + 1].AddForce(breakForce, ForceMode.Impulse);

            _broken = true;
        }
    }
}