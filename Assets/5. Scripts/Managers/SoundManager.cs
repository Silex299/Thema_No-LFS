using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudio;
    [SerializeField] private string musicClipFolder;
    [SerializeField] private float musicTransitionTime = 2;

    [SerializeField] private float maximumMusicVolume = 1;

    public void ChangeClip(string clip)
    {
        var clipPath = musicClipFolder + clip;

        AudioClip audioClip = Resources.Load<AudioClip>(clipPath);
        

        if (audioClip)
        {
            StartCoroutine(TransitionMusic(audioClip, musicTransitionTime));
        }
        else
        {
            Debug.LogWarning("No resource found named " + clip + " in folder : " + clipPath);
        }
    }



    private void Start()
    {
        //musicAudio.volume = maximumMusicVolume;
    }

    private IEnumerator TransitionMusic(AudioClip newClip, float transitionTime)
    {
        while(musicAudio.volume > 0)
        {
            musicAudio.volume -= (0.2f / transitionTime);
            yield return new WaitForSeconds(0.1f);
        }

        musicAudio.clip = newClip;
        musicAudio.Play();

        while (musicAudio.volume < maximumMusicVolume)
        {
            musicAudio.volume += (0.2f / transitionTime);
            yield return new WaitForSeconds(0.1f);
        }

    }

}
