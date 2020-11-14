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

    private GameManager _GameManager;

    private void Start()
    {
        _GameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    public override void OnStep()
    {
        base.OnStep();

        if (IsShootStep())
        {
            ShootEvent?.Raise();

            if (_GameManager.PreviousAction != CounterAction)
            {

            }
        }

        if (IsPrepareStep())
        {
            StartPrepareEvent?.Raise();
        }

    }

    public virtual bool IsShootStep() => _GameManager.stepCount == ShootStep;


    public virtual bool IsPrepareStep() => _GameManager.stepCount == ShootStep - IndicatorSteps;
}
