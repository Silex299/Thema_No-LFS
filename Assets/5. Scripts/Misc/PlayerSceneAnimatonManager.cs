using UnityEngine;
using UnityEngine.Playables;

// ReSharper disable once CheckNamespace
namespace Misc
{
    public class PlayerSceneAnimatonManager : MonoBehaviour
    {

        [SerializeField] private PlayableDirector m_director;
        [SerializeField] private PlayableAsset[] m_sceneAnimations;

        public void PlayPlayerSceneAnimation(int index)
        {
            m_director.Play(m_sceneAnimations[index]);
        }



    }
}
