using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource mainTrackAudioSource;
    [SerializeField]
    AudioSource speechAudiosource;
    [SerializeField]
    AudioSource soundSFXAudioSource;


    public AudioClip[] speechClips;
    [SerializeField]
    AudioClip[] MainTracks;
    [SerializeField]
    AudioClip[] playerDeathSounds;
    [SerializeField]
    AudioClip[] killSounds;
    [SerializeField]
    AudioClip[] deflectSounds;
    [SerializeField]
    AudioClip[] winSounds;
    [SerializeField]
    AudioClip[] shootSounds;


    public bool PlayCurrentClipStory(int storyClipIndex)
    {
        StartCoroutine(PlayAudioSequencially(storyClipIndex));
        return true;
    }
    public void PlaySelectMusicTrack(int index)
    {
        mainTrackAudioSource.clip = MainTracks[index];
        mainTrackAudioSource.Play();
    }
    public void Playerdeath()
    {
        soundSFXAudioSource.clip = playerDeathSounds[Random.Range(0, playerDeathSounds.Length)];
        soundSFXAudioSource.Play();
    }
    public void EnemyDeath()
    {
        soundSFXAudioSource.clip = killSounds[Random.Range(0, killSounds.Length)];
        soundSFXAudioSource.Play();
    }

    public void Deflect()
    {
        soundSFXAudioSource.clip = deflectSounds[Random.Range(0, deflectSounds.Length)];
        soundSFXAudioSource.Play();
    }
    public void Win()
    {
        soundSFXAudioSource.clip = winSounds[Random.Range(0, winSounds.Length)];
        soundSFXAudioSource.Play();
    }

    public void ArcherShoots()
    {
        soundSFXAudioSource.clip = shootSounds[Random.Range(0, shootSounds.Length)];
        soundSFXAudioSource.Play();
    }


    IEnumerator PlayAudioSequencially(int storyClipIndex)
    {
        speechAudiosource.clip = speechClips[storyClipIndex];
        speechAudiosource.Play();
            while (speechAudiosource.isPlaying)
            {
                yield return null;
            }
        }
    }



