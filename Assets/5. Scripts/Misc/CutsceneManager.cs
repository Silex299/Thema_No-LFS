using UnityEngine;
using UnityEngine.Playables;

// ReSharper disable once CheckNamespace
namespace Misc
{
    public class CutsceneManager : MonoBehaviour
    {
        [SerializeField] private PlayableAsset[] clip;
        [SerializeField] private PlayableDirector director;
 
        
        //Create singleton instance
        public static CutsceneManager Instance { get; private set; }
        
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
    
        public void PlayClip(int index)
        {
            Debug.Log(index);
            director.Play(clip[index]);
        }

    }
}
