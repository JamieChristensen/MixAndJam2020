using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

[RequireComponent(typeof(AudioSource))]
public class FeedbackTest : MonoBehaviour
{
    [SerializeField]
    AudioClip clip = default;

    [SerializeField]
    CinemachineVirtualCamera followCamera = default;

    [SerializeField]
    float cameraShakeAmount = 1f, shakeDuration = 1f;

    AudioSource audSource;
    CinemachineBasicMultiChannelPerlin cameraNoiseChannel;

    void Start()
    {
        cameraNoiseChannel = followCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        audSource = GetComponent<AudioSource>();
    }

    public void playClipOnBeat()
    {
        audSource.PlayOneShot(clip);
    }

    [ContextMenu("shake that camera booty")]
    public void CameraShake()
    {
        StartCoroutine(TimeDelay());
    }

    IEnumerator TimeDelay()
    {
        cameraNoiseChannel.m_AmplitudeGain = cameraShakeAmount;
        yield return new WaitForSeconds(shakeDuration);
        cameraNoiseChannel.m_AmplitudeGain = 0;
    }
}
