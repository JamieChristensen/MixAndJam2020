using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingRangedEnemy : RangedEnemy
{
    public override bool IsPrepareStep()
    {
        int PrepareStep = ShootStep - IndicatorSteps;
        return GameManager.instance.stepCount >= PrepareStep && GameManager.instance.stepCount % ShootStep == PrepareStep;
    }

    public override bool IsShootStep() => GameManager.instance.stepCount >= ShootStep && GameManager.instance.stepCount % ShootStep == 0;
}
