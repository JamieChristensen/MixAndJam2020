using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FeedbackTest : MonoBehaviour
{
    [SerializeField]
    AudioClip clip;


    AudioSource audSource;

    void Start()
    {
        audSource = GetComponent<AudioSource>();
    }

    public void playClipOnBeat()
    {
        audSource.PlayOneShot(clip);
    }
}
