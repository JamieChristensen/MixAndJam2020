using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class Cell : MonoBehaviour
{
    public int2 point;
    public void SpawnStepUnit(StepUnit Unit)
    {
        Instantiate(Unit, transform.position + new Vector3(0, 1, 0), Unit.transform.rotation, transform);
    }

    public void SpawnStepUnit(StepUnit Unit, int stepArgument)
    {
        Instantiate(Unit, transform.position + new Vector3(0, 1, 0), Unit.transform.rotation, transform).GetComponent<RangedEnemy>().ShootStep = stepArgument;
    }
}
