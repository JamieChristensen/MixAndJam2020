using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Events;
using DG.Tweening;

public class BeatHighligther : MonoBehaviour
{

    Material mat;

    [Header("shader settings")]
    [SerializeField]
    float glowBurstAmount; 
    [SerializeField]
    float  normalBustAmount;


    string highlightReference = "FresnelPower";


    public VoidEvent testRaise;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    public void SetFresnelPower()
    {
        mat.DOKill();
        mat.SetFloat(highlightReference, glowBurstAmount);
        mat.DOFloat(normalBustAmount, highlightReference, 1f);
    }

    [ContextMenu("raise a test event")]
    public void RaiseTestEvent()
    {
        testRaise.Raise();
    }

}
