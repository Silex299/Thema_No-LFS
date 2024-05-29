using Sirenix.OdinInspector;
using UnityEngine;

public class TearableWire : MonoBehaviour
{
    [SerializeField, BoxGroup("Rope Properties")]
    private Transform connectedPoint;


    [SerializeField, BoxGroup("Rope Properties")]
    private Transform endPoint;

    [SerializeField, BoxGroup("Rope Properties")]
    private int wireResolution;

    [SerializeField, BoxGroup("Rope Properties")]
    private float wireWidth;

    [SerializeField, BoxGroup("Rope Properties")]
    private int tearIndex;
    
    [SerializeField, BoxGroup("Rope Properties")] 
    private Material ropeMaterial;
    
    [SerializeField] private Rigidbody[] ropeSegments;
    [SerializeField] private LineRenderer _lineRenderer1;
    [SerializeField] private LineRenderer _lineRenderer2;

    [SerializeField] private HingeJoint _tearJoint;
    private bool _isConnected = true;


#if  UNITY_EDITOR
    
    
    [Button("Create Wire", ButtonSizes.Large)]
    public void CreateWire()
    {
        ropeSegments = new Rigidbody[wireResolution + 1];

        #region add rigidBody and fixed joint Start point

        if (!connectedPoint.gameObject.TryGetComponent<Rigidbody>(out ropeSegments[0]))
        {
            ropeSegments[0] = connectedPoint.gameObject.AddComponent<Rigidbody>();
        }

        if (!connectedPoint.gameObject.TryGetComponent<FixedJoint>(out var joint))
        {
            connectedPoint.gameObject.AddComponent<FixedJoint>();
        }

        #endregion

        //Create rope segments

        float distance = Vector3.Distance(connectedPoint.position, endPoint.position);
        float step = distance / wireResolution;
        Vector3 direction = endPoint.position - connectedPoint.position;
        direction = direction.normalized;

        for (int i = 1; i <= wireResolution; i++)
        {
            GameObject obj = new GameObject()
            {
                transform =
                {
                    parent = transform,
                    position = connectedPoint.position + direction * step * i,
                    rotation = Quaternion.LookRotation(direction),
                }
            };

            ropeSegments[i] = obj.AddComponent<Rigidbody>();

            HingeJoint newJoint = obj.AddComponent<HingeJoint>();
            newJoint.anchor = new Vector3(0, 0, -step);
            newJoint.connectedBody = ropeSegments[i - 1];

            if (i == tearIndex)
            {
                newJoint.breakForce = 1000f;
                _tearJoint = newJoint;
            }

            CapsuleCollider capsuleCollider = obj.AddComponent<CapsuleCollider>();
            capsuleCollider.direction = 2;
            capsuleCollider.center = new Vector3(0, 0, -step / 2);
            capsuleCollider.height = step - 0.01f;
            capsuleCollider.radius = wireWidth;
        }


        #region Add rb and fixed joint to end point

        if (!endPoint.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            endPoint.gameObject.AddComponent<Rigidbody>();
        }

        if (!endPoint.gameObject.TryGetComponent<FixedJoint>(out var joint2))
        {
            var join = endPoint.gameObject.AddComponent<FixedJoint>();
            join.connectedBody = ropeSegments[wireResolution];
        }
        else
        {
            endPoint.gameObject.GetComponent<FixedJoint>().connectedBody = ropeSegments[wireResolution];
        }

        #endregion
    }


    [Button("Create LineRenderer", ButtonSizes.Large)]
    public void CreateLineRenderer()
    {
        if (!connectedPoint.gameObject.TryGetComponent<LineRenderer>(out _lineRenderer1))
        {
            _lineRenderer1 = connectedPoint.gameObject.AddComponent<LineRenderer>();
        }

        if (!endPoint.gameObject.TryGetComponent<LineRenderer>(out _lineRenderer2))
        {
           _lineRenderer2 = endPoint.gameObject.AddComponent<LineRenderer>();
        }

        _lineRenderer1.material = ropeMaterial;
        _lineRenderer1.startWidth = wireWidth * 2;
        _lineRenderer1.endWidth = wireWidth * 2;
        
        _lineRenderer2.material = ropeMaterial;
        _lineRenderer2.startWidth = wireWidth * 2;
        _lineRenderer2.endWidth = wireWidth * 2;
        
        _lineRenderer1.positionCount = tearIndex+1;
        _lineRenderer2.positionCount = wireResolution + 1 - tearIndex;

        for (int i = 0; i <= tearIndex; i++)
        {
            _lineRenderer1.SetPosition(i, ropeSegments[i].position);
        }

        for (int i = tearIndex; i <= wireResolution; i++)
        {
            _lineRenderer2.SetPosition(i-tearIndex, ropeSegments[i].position);
        }
    }

#endif
    
    private void Update()
    {
        //Check if it is still connected
        if (!_tearJoint && _isConnected)
        {
            _isConnected = false;
            _lineRenderer1.positionCount = tearIndex;
        }

        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (!_isConnected)
        {
            for (int i = 0; i < tearIndex; i++)
            {
                _lineRenderer1.SetPosition(i, ropeSegments[i].position);
            }
        }
        else
        {
            
            print(_tearJoint.connectedBody.GetComponent<ConstantForce>());
            
            for (int i = 0; i <= tearIndex; i++)
            {
                _lineRenderer1.SetPosition(i, ropeSegments[i].position);
            }
        }

        for (int i = tearIndex; i <= wireResolution; i++)
        {
            _lineRenderer2.SetPosition(i-tearIndex, ropeSegments[i].position);
        }
    }
}