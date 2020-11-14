using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public abstract class StepUnit : MonoBehaviour
{
    public bool shouldStep = true;
    public int2 gridPosition;

    public virtual void OnStep()
    {
        if (!shouldStep)
        {
            return;
        }

        
        //Do other things in classes.
    }

}
