using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StepUnit : MonoBehaviour
{
    public bool shouldStep = true;

    public virtual void OnStep()
    {
        if (!shouldStep)
        {
            return;
        }

        //Do other things in classes.
    }

}
