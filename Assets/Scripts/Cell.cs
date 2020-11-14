using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public void SpawnStepUnit(StepUnit Unit)
    {
        Instantiate(Unit, transform.position + new Vector3(0,1,0), Quaternion.identity, this.transform);
    }
}
