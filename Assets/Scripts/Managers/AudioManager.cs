using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource source;


    public void PlayOneShotClip(AudioClip clipToPlay)
    {
        source.PlayOneShot(clipToPlay);
    }
}