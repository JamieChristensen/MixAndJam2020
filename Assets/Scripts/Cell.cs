using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public void SpawnStepUnit(StepUnit Unit)
    {
        Instantiate(Unit, transform.position, Quaternion.identity, this.transform);
    }
}
