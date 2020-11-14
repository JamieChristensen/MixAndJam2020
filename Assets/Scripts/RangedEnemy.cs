using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Events;

public class RangedEnemy : StepUnit
{
    public int ShootStep;

    public int IndicatorSteps;

    public VoidEvent StartPrepareEvent;

    public VoidEvent ShootEvent;

    public PlayerAction CounterAction;


    // Start is called before the first frame update
    public override void OnStep()
    {
        base.OnStep();

        if (IsShootStep())
        {
            ShootEvent?.Raise();

            if (GameManager.instance.PreviousAction != CounterAction)
            {

            }
        }

        if (IsPrepareStep())
        {
            StartPrepareEvent?.Raise();
        }

    }

    public virtual bool IsShootStep() => GameManager.instance.stepCount == ShootStep;


    public virtual bool IsPrepareStep() => GameManager.instance.stepCount == ShootStep - IndicatorSteps;
}
