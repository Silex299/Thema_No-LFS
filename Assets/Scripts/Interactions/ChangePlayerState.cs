using UnityEngine;
using Sirenix.OdinInspector;
using Player_Scripts;

namespace Interactions
{
	public class ChangePlayerState : MonoBehaviour
	{
   
		[SerializeField] private string triggerTag = "Player_Main";
		[SerializeField] private PlayerController.PlayerStates changeStateTo;
		[SerializeField] private int stateIndex;
		[SerializeField] private float transitionTime;
		
		
		
		[SerializeField, Space(10)] private bool isAutomatic;
		[SerializeField, HideIf("isAutomatic")] private string InputButton;

		[SerializeField] private bool isConditional;
		[SerializeField, ShowIf("isConditional")] private PlayerController.PlayerStates conditionalState;


		private bool _playerIsInTrigger;

		
		// OnTriggerEnter is called when the Collider other enters the trigger.
		protected void OnTriggerEnter(Collider other)
		{
			if(other.CompareTag(triggerTag))
			{

                if (!isAutomatic)
                {
					_playerIsInTrigger = true;

					return;
                }

                if (!isConditional)
                {
					ChangeState();
                }
                else
                {
					if(PlayerController.Instance.initState == conditionalState)
                    {
						ChangeState();
                    }
                }

			}
		}
		
		
		public void ChangeState()
		{
			Player_Scripts.PlayerController.Instance.ChangeState(changeStateTo, stateIndex, transitionTime);
		}

        private void Update()
        {
			if (!_playerIsInTrigger) return;


            if (Input.GetButtonDown(InputButton))
            {
                if (!isConditional)
                {
					ChangeState();
                }
                else
                {
					if(PlayerController.Instance.initState == conditionalState)
                    {
						ChangeState();
                    }
                }
            }

        }

    }
	
}


