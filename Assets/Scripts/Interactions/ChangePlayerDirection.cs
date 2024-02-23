using Player_Scripts;
using UnityEngine;

namespace Interactions
{
    public class ChangePlayerDirection : MonoBehaviour
    {
        [SerializeField] private float direction;
        static private readonly int Direction = Animator.StringToHash("Direction");

        public void ChangeDirection()
        {
            PlayerController.Instance.Player.AnimationController.SetFloat(Direction, direction, 0.5f, Time.deltaTime);
        }
    }
}
