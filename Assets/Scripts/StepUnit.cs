using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using GameJam.Events;
using Sirenix.OdinInspector;
using UnityEngine.Animations;

public abstract class StepUnit : SerializedMonoBehaviour
{

    public bool shouldStep = true;
    public int2 gridPosition;

    public VoidEvent OnKillEvent;

    private void Update()
    {
        LookAt();
        
    }

    public virtual void LookAt()
    {
        transform.LookAt(GameManager.instance.playerGO.transform.position);
    }
    public virtual void OnStep()
    {
        if (!shouldStep)
        {
            return;
        }



        //Do other things in classes.
    }

    public virtual void IsKill()
    {
        shouldStep = false;
        OnKillEvent?.Raise();
        Debug.Log("Unit is kill.");
    }

}
