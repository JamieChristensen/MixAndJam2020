using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;

public class FeedbackCanvas : MonoBehaviour
{
    public TMPro.TextMeshProUGUI TextObject;


    private void Awake()
    {
        InstantDeactive();
    }

    [Button(ButtonSizes.Small)]
    public void InstantDeactive()
    {
        TextObject.alpha = 0f;
    }

    public void SetText(string Text)
    {
        TextObject.text = Text;
    }

    [Button(ButtonSizes.Large)]
    [ContextMenu("BOOM")]
    public void ActivateFeedback()
    {
        // Default 
        var DefaultScale = TextObject.rectTransform.localScale;
        DefaultScale.y = 0;
        TextObject.rectTransform.localScale = DefaultScale;
        TextObject.alpha = 1;

        // BAM! In!
        TextObject.rectTransform.DOScaleY(1, 0.2f);
        TextObject.DOFade(1.0f, 0.2f);
        TextObject.rectTransform.DOShakeRotation(0.4f, 40, 5);

        // Slow fade out
        DeactivateFeedback();
    }

    [Button(ButtonSizes.Large)]
    [ContextMenu("MOOB")]
    public void DeactivateFeedback()
    {
        TextObject.DOFade(0.0f, 2.0f);
    }
}
