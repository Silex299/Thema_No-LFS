using UnityEngine;

public class ProceduaralSpiderLeg : MonoBehaviour
{

    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 raycastPosition;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float stepDistance;
    [SerializeField] private float speed;
    [SerializeField] private float stepHeight;


#if UNITY_EDITOR
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(parent.position + raycastPosition.x * parent.right + raycastPosition.y * parent.up + raycastPosition.z * parent.forward, 0.02f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + offset.x * parent.right + offset.y * parent.up + offset.z * parent.forward, 0.02f);

        UnityEditor.Handles.color = new Color(0.1f,1f,0.1f, 0.2f);
        UnityEditor.Handles.DrawSolidDisc(transform.position + offset.x * parent.right + offset.y * parent.up + offset.z * parent.forward, Vector3.up, stepDistance);
    }

    
#endif

    private Vector3 newPosition;
    private Vector3 currentPosition;
    private float lerp = 1;



    private void Start()
    {

    }

    private void FixedUpdate()
    {
        var position = transform.position + offset.x * parent.right + offset.y * parent.up + offset.z * parent.forward;
        currentPosition = transform.position;


        if (Physics.Raycast(parent.position + raycastPosition.x * parent.right + raycastPosition.y * parent.up + raycastPosition.z * parent.forward, -parent.up, out RaycastHit hit, 0.5f))
        {
            //TODO remove
            Debug.DrawLine(parent.position + raycastPosition.x * parent.right + raycastPosition.y * parent.up + raycastPosition.z * parent.forward, hit.point);


            var distance = Vector2.Distance(new Vector2(hit.point.x, hit.point.z), new Vector2(position.x, position.z));
            if (distance >= stepDistance)
            {

                newPosition = hit.point;
                lerp = 0;
            }




        }

        if (lerp < 1)
        {
            currentPosition = Vector3.Lerp(currentPosition, newPosition, Time.deltaTime * speed);
            currentPosition.y = newPosition.y + Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            transform.position = currentPosition;

            lerp += Time.fixedDeltaTime * speed;
        }







    }



}
