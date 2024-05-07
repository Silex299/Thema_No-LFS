using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Misc.Items
{
    public class Rope : MonoBehaviour
    {
        [SerializeField, BoxGroup("Rope Properties")] private int ropeResolution;
        [SerializeField, BoxGroup("Rope Properties")] private float ropeLength;
        [SerializeField, BoxGroup("Rope Properties")] private float ropeThickness;
        [SerializeField, BoxGroup("Rope Properties")] private Material ropeMaterial;

        [SerializeField, BoxGroup("HingeJoint")] private float spring, damp;

        [SerializeField, Space(10)] private GameObject[] ropeSegments;
        [SerializeField] private LineRenderer[] lineRenderers;

        private bool _swing;
        private Coroutine[] _coroutines = new Coroutine[3];

        public int overriderIndex;

        [Button("Create Rope", ButtonSizes.Large), GUIColor(1, 0.3f, 0.3f)]
        public void CreateRopeSegments()
        {
            //delete all children
            foreach (GameObject obj in ropeSegments)
            {
                DestroyImmediate(obj);
            }

            ropeSegments = new GameObject[ropeResolution];
            lineRenderers = new LineRenderer[ropeResolution];

            for (int i = 0; i < ropeResolution; i++)
            {
                //instantiate gameObject with current object being the parent
                ropeSegments[i] = new GameObject("RopeSegment_" + i)
                {
                    transform =
                    {
                        parent = transform,
                        position = new Vector3(transform.position.x,
                            transform.position.y - i * (ropeLength) / (ropeResolution - 1), transform.position.z)
                    }
                };


                if (i > 0)
                {

                    #region  Line Renderer

                    lineRenderers[i] = ropeSegments[i].AddComponent<LineRenderer>();

                    lineRenderers[i].startWidth = ropeThickness;
                    lineRenderers[i].endWidth = ropeThickness;
                    lineRenderers[i].material = ropeMaterial;
                    lineRenderers[i].textureMode = LineTextureMode.Tile;
                    lineRenderers[i].positionCount = 2;
                    
                    lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                    lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);

                    #endregion

                    #region Hinge Joint

                    ropeSegments[i].AddComponent<Rigidbody>();
                    
                    CapsuleCollider collider = ropeSegments[i].AddComponent<CapsuleCollider>();
                    collider.center = new Vector3(0, ((ropeLength / (ropeResolution - 1)) - 0.1f) / 2, 0);
                    collider.height = (ropeLength / (ropeResolution - 1)) - 0.1f;
                    collider.radius = ropeThickness/2;
                    
                    HingeJoint joint = ropeSegments[i].AddComponent<HingeJoint>();
                    
                    joint.useSpring = true;
                    joint.spring = new JointSpring()
                    {
                        spring = spring,
                        damper = damp
                    };
                    
                    joint.anchor =new Vector3(0, ropeLength / (ropeResolution - 1), 0);
                    joint.connectedBody = ropeSegments[i - 1].GetComponent<Rigidbody>();

                    #endregion
                }
                else
                {
                    ropeSegments[i].AddComponent<Rigidbody>();
                    ropeSegments[i].AddComponent<FixedJoint>();
                }

                
            }
        }


        private void Start()
        {
            _coroutines = new Coroutine[ropeResolution];
        }

        private void Update()
        {
            //for each line render segment set the 2nd position to the previous segment position and 1st position to current segment position
            for (int i = 1; i < ropeResolution; i++)
            {
                lineRenderers[i].SetPosition(0, ropeSegments[i].transform.position);
                lineRenderers[i].SetPosition(1, ropeSegments[i - 1].transform.position);
            }
        }


        /**
        if (_swing) return;

        var distance = ropeSegments[2].position.z - ropeSegments[1].position.z;

        if (Mathf.Abs(distance) > 0.1f)
        {
            _coroutines[1] = StartCoroutine(SwaySegment(1));
        }

    }

    private IEnumerator SwaySegment(int index)
    {
        _swing = true;

        float timeElapsed = 0;
        Vector3 pos1 = ropeSegments[index].position;
        Vector3 pos0 = ropeSegments[index - 1].position;
        float amp = pos0.z - pos1.z;

        while (timeElapsed < stiffness)
        {
            Vector3 segmentPos = ropeSegments[index].position;
            Vector3 prevSegmentPos = ropeSegments[index - 1].position;

            if (index < ropeResolution - 1)
            {
                Vector3 nextSegmentPos = ropeSegments[index + 1].position;
                if (Vector3.Distance(nextSegmentPos, segmentPos) > 0.2f)
                {
                    if (_coroutines[index + 1] == null)
                    {
                        _coroutines[index + 1] = StartCoroutine(SwaySegment(index + 1));
                    }
                }
            }

            float segmentDistance = (ropeLength) / (ropeResolution - 1);


            if (index == overriderIndex)
            {
                segmentPos.z = 0.5f;
            }
            else
            {
                float z = Thema.Spring(timeElapsed, amp, damp, stiffness);

                segmentPos.z = prevSegmentPos.z + Mathf.Clamp(z, -segmentDistance, segmentDistance);
            }


            float y = prevSegmentPos.y -
                      Mathf.Sqrt(Mathf.Pow(segmentDistance, 2) - Mathf.Pow(segmentPos.z - prevSegmentPos.z, 2));

            segmentPos.y = y;

            ropeSegments[index].position = segmentPos;
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        _coroutines[index] = null;
        _swing = false;
    }

        **/
    }
}