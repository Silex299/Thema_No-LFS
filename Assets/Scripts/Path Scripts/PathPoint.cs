using Sirenix.OdinInspector;
using UnityEngine;

namespace Path_Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class PathPoint : MonoBehaviour
    {

        public int nextPointIndex;
        public int currentIndex;
        public int prevPointIndex;

#if UNITY_EDITOR
        [Button("Move player to this point")]
        public void MovePlayer()
        {
            Transform player = FindObjectOfType<Player_Scripts.PlayerMovementController>().transform;

            player.position = transform.position;

        }

#endif

        /// <summary>
        /// Sets the next and previous destination for the point 
        /// </summary>
        /// <param name="next"> next position index </param>
        /// <param name="prev"> previous position index </param>
        /// <para name="current"> current position index </para>
        public void SetPoint(int next, int prev, int current)
        {
            nextPointIndex = next;
            prevPointIndex = prev;
            currentIndex = current;
        }



        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player_Main"))
            {
                //Set next and previous destination
                PlayerPathController.Instance.previousDestination = prevPointIndex;
                PlayerPathController.Instance.nextDestination = nextPointIndex;
            }
        }

    }
}
