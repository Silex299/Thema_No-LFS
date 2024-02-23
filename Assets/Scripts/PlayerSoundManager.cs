using Player_Scripts;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundManager : SerializedMonoBehaviour
{
    [SerializeField, BoxGroup("Sources")] private AudioSource impactSounds;
    [SerializeField, BoxGroup("Sources")] private AudioSource playerSounds;


    [SerializeField, TabGroup("AudioClips", "Steps")]
    private Dictionary<string, AudioClip[]> taggedStepsClips;

    [SerializeField, TabGroup("AudioClips", "Steps")]
    private Dictionary<string, AudioClip[]> taggedRunningStepsClips;

    [SerializeField, TabGroup("AudioClips", "Actions")]
    private Dictionary<string, AudioClip> jumpClips;
    [SerializeField, TabGroup("AudioClips", "Actions")]
    private Dictionary<string, AudioClip> landClips;


    [SerializeField, TabGroup("AudioClips", "Player Sounds")]
    private AudioClip standingJumpClip;

    [SerializeField, TabGroup("AudioClips", "Player Sounds")]
    private AudioClip runningJumpClip;



    private bool _sourceEngaged;

    public void StandingJumpSound()
    {
        if (_sourceEngaged) return;

        playerSounds.PlayOneShot(standingJumpClip, 0.5f);

        if (!jumpClips.TryGetValue(CheckGroundType(), out AudioClip clip))
        {
            clip = jumpClips.GetValueOrDefault("Untagged");
        }
        impactSounds.PlayOneShot(clip, 1);
    }


    public void RunningJumpSound()
    {
        print("Hello");
        if (_sourceEngaged) return;

        playerSounds.PlayOneShot(runningJumpClip, 0.5f);

        if (!jumpClips.TryGetValue(CheckGroundType(), out AudioClip clip))
        {
            clip = jumpClips.GetValueOrDefault("Untagged");
        }
        impactSounds.PlayOneShot(clip, 1);

    }
    public void LandJump()
    {
        if (_sourceEngaged) return;

        if (!landClips.TryGetValue(CheckGroundType(), out AudioClip clip))
        {
            clip = landClips.GetValueOrDefault("Untagged");
        }
        impactSounds.PlayOneShot(clip, 1);
    }

    public void FootSteps()
    {
        //TODO : Simplify these
        if (Input.GetButton("Sprint")) return;

        var axis = PlayerController.Instance.Player.invertedAxis;
        if (axis && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.5f) return;
        if (!axis && Mathf.Abs(Input.GetAxis("Vertical")) < 0.5f) return;


        if (taggedStepsClips.Count < 0) return;

        if(!taggedStepsClips.TryGetValue(CheckGroundType(), out AudioClip[] clips))
        {
            clips = taggedStepsClips.GetValueOrDefault("Untagged");
        }

        int rand = Random.Range(0, clips.Length);
        impactSounds.PlayOneShot(clips[rand], 0.75f);
    }

    public void FootStepsRunning()
    {
        //TODO : Simplify these
        if (!Input.GetButton("Sprint")) return;
        var axis = PlayerController.Instance.Player.invertedAxis;
        if (axis && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.5f) return;
        if (!axis && Mathf.Abs(Input.GetAxis("Vertical")) < 0.5f) return;


        if (taggedRunningStepsClips.Count < 0) return;

        if (!taggedRunningStepsClips.TryGetValue(CheckGroundType(), out AudioClip[] clips))
        {
            clips = taggedRunningStepsClips.GetValueOrDefault("Untagged");
        }

        int rand = Random.Range(0, clips.Length);
        impactSounds.PlayOneShot(clips[rand], 0.8f);
    }

    private string CheckGroundType()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            return hit.collider.tag;
        }

        return "Untagged";
    }



}
