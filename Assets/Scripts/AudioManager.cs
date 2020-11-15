using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource adSource;
    [SerializeField]
    AudioClip[] adClips;

    public bool PlayCurrentClipStoryClip(int storyClipIndex)
    {

        StartCoroutine(PlayAudioSequencially(storyClipIndex));
        return true;

    }

    IEnumerator PlayAudioSequencially(int storyClipIndex)
    {

            adSource.clip = adClips[storyClipIndex];
            adSource.Play();
            while (adSource.isPlaying)
            {
                yield return null;
            }
            
        }
    }



