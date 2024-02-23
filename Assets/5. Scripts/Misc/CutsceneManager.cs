using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private PlayableAsset[] clip;
    [SerializeField] private PlayableDirector director;
 
    
    public void PlayClip(int index)
    {
        director.Play(clip[index]);
    }

}
