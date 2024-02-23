using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageCollider : MonoBehaviour
{
	
	[SerializeField] private List<CollisionDectection> collisions;



    protected void OnCollisionEnter(Collision collision)
	{


		for(int i = 0; i<collisions.Count; i++)
        {
            if (collision.collider.CompareTag(collisions[i].colliderTag))
			{
				if (!collisions[i].executed)
                {
					if(collision.impulse.magnitude >= collisions[i].thresold)
					{
						collisions[i].collisionEvent?.Invoke();

						var col_ = collisions[i];
						col_.executed = true;
						collisions[i] = col_;
					}
                }
            }
        }
    }


    [System.Serializable]
	private struct CollisionDectection
	{
		public string colliderTag;
		public bool executed;
		public UnityEvent collisionEvent;
		public float thresold;
	}
	
    
}
