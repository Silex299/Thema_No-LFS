using UnityEngine;
using UnityEngine.Playables;

// ReSharper disable once CheckNamespace
namespace Misc
{
    public class PlayerSceneAnimatonManager : MonoBehaviour
    {

        [SerializeField] private PlayableDirector m_director;
        [SerializeField] private PlayableAsset[] m_sceneAnimations;


        //Create a singleton 
        public static PlayerSceneAnimatonManager Instance { get; private set; }
    
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    

        public void PlayPlayerSceneAnimation(int index)
        {
            m_director.Play(m_sceneAnimations[index]);
        }



    }
}
