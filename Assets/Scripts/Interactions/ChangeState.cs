using Player_Scripts;
using UnityEngine;

namespace Interactions
{
    public class ChangeState : MonoBehaviour
    {

        [SerializeField] private PlayerController.PlayerStates newState;
        [SerializeField] private int stateIndex;
        [SerializeField] private float transitionTime = 0.5f;

        public void ChangePlayerState()
        {
            PlayerController.Instance.ChangeState(newState, stateIndex, transitionTime);
        }

    }
}
