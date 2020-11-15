using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class FeedbackManager : MonoBehaviour
{
    [SerializeField]
    AudioClip oneBeatAudio = default;
    [SerializeField]
    CinemachineVirtualCamera followCamera = default;
    [SerializeField]
    float beastShake = 1f, deathShake = 2f, shakeDuration = .2f;
    [SerializeField]
    Image panelWarning;
    [SerializeField]
    Color PanelStartColor, warningstartColor;
    [SerializeField]
    float panelWarningSpeed = 1.2f;
    AudioSource audSource;
    CinemachineBasicMultiChannelPerlin cameraNoiseChannel;

     bool lerpPanel;
    float startLerp = 0;

    void Start()
    {
        if (followCamera != null)
        {
            cameraNoiseChannel = followCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
        
        audSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        
        if (lerpPanel)
        {
            startLerp += Time.deltaTime * panelWarningSpeed;
            panelWarning.color = Color.Lerp(PanelStartColor, warningstartColor, startLerp);
        }
    }

    public void playClipOnBeat()
    {
        audSource.PlayOneShot(oneBeatAudio);
    }

    [ContextMenu("shake that camera booty")]
    public void BeatShake()
    {
        StartCoroutine(BeatCameraShake());
    }
    [ContextMenu("shake that camera booty")]
    public void DeathShake()
    {
        StartCoroutine(DeathCameraShake());
    }

    [ContextMenu("testWarningPanel")]
    public void WarningArrowIncoming()
    {
        lerpPanel = true;
    }
    [ContextMenu("testDeflectedArrow")]
    public void DeflectedArrow()
    {
        lerpPanel = false;
        panelWarning.color = PanelStartColor;
    }


    IEnumerator DeathCameraShake()
    {
        cameraNoiseChannel.m_AmplitudeGain = deathShake;
        yield return new WaitForSeconds(shakeDuration+ .5f);
        cameraNoiseChannel.m_AmplitudeGain = 0;
    }

    IEnumerator BeatCameraShake()
    {
        cameraNoiseChannel.m_AmplitudeGain = beastShake;
        yield return new WaitForSeconds(shakeDuration);
        cameraNoiseChannel.m_AmplitudeGain = 0;
    }
}
