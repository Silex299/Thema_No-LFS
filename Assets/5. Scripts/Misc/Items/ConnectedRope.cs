using Sirenix.OdinInspector;
using UnityEngine;

namespace Misc.Items
{
    public class ConnectedRope : MonoBehaviour
    {
        public Rigidbody[] ropeSegments;
        public LineRenderer[] lineRenderers;

        [Space(10)] public int ropeResolution;
        public float ropeThickness;
        public Material ropeMaterial;
        public float ropeLength;

        [Space(10)] public bool useSpring;
        public float spring;
        public float damp;

        [Space(10)] public Transform start;
        public Transform end;
        

        [Button("Create Rope", ButtonSizes.Large), GUIColor(1, 0.3f, 0.3f)]
        public void CreateRopeSegments()
        {
            //delete all the children of the current object
            foreach (var segment in ropeSegments)
            {
                DestroyImmediate(segment.gameObject);
            }

            
            ropeSegments = new Rigidbody[ropeResolution];
            lineRenderers = new LineRenderer[ropeResolution];
            
            
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
                    layer = LayerMask.NameToLayer("Ignore Player")
                };

                ropeSegments[i] = obj.AddComponent<Rigidbody>();

                //For start and end fixed joint
                if (i == 0)
                {
                    
                }
                

                if (i > 0)
                {
                    #region Line Renderer

                    lineRenderers[i] = ropeSegments[i].gameObject.AddComponent<LineRenderer>();

                    lineRenderers[i].startWidth = ropeThickness;
                    lineRenderers[i].endWidth = ropeThickness;
                    lineRenderers[i].material = ropeMaterial;
                    lineRenderers[i].textureMode = LineTextureMode.Tile;
                    lineRenderers[i].positionCount = 2;

                    lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                    lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);

                    #endregion

                    #region Hinge Joint

                    CapsuleCollider addedCollider = ropeSegments[i].gameObject.AddComponent<CapsuleCollider>();
                    addedCollider.center = new Vector3(0, ((ropeLength / (ropeResolution - 1)) - 0.1f) / 2, 0);
                    addedCollider.height = (ropeLength / (ropeResolution - 1)) - 0.1f;
                    addedCollider.radius = ropeThickness / 2;

                    HingeJoint joint = ropeSegments[i].gameObject.AddComponent<HingeJoint>();

                    joint.useSpring = true;
                    joint.spring = new JointSpring()
                    {
                        spring = spring,
                        damper = damp
                    };

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

        
        
    }
}