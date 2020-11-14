using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using GameJam.Events;

public abstract class StepUnit : MonoBehaviour
{

    public bool shouldStep = true;
    public int2 gridPosition;

    public VoidEvent OnKillEvent;

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
        OnKillEvent?.Raise();
        Debug.Log("Unit is kill.");
    }

}
