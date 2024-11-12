using NPCs.New.V1;
using UnityEngine;

namespace Sounds
{
    public class NpcStateControlledSound : MonoBehaviour
    {


        public V1ChangeSoundSourceProperties sound;
        public V1Npc[] npcs;
        public int chaseStateIndex;


        private bool _isSourcesPlaying = false;

        private void OnEnable()
        {
            foreach (var npc in npcs)
            {
                npc.onStateChange += OnNpcStateChange;
            }
        }

        private void OnDisable()
        {
            foreach (var npc in npcs)
            {
                npc.onStateChange -= OnNpcStateChange;
            }
        }


        private void OnNpcStateChange(int stateIndex)
        {
            bool isAnyoneChasing = false;
            
            foreach (var npc in npcs)
            {
                if (npc.CurrentStateIndex == chaseStateIndex)
                {
                    isAnyoneChasing = true;
                    break;
                }
            }

            if (_isSourcesPlaying != isAnyoneChasing)
            {
                if(isAnyoneChasing) sound.PlaySource();
                else sound.StopSource();
                
                _isSourcesPlaying = isAnyoneChasing;
            }
            
            
        }




    }
}
