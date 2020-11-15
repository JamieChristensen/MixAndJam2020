using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PathMarker : StepUnit
{
    public GridManager GridManager;

    public int Offset;

    private void Awake()
    {
        transform.localScale = new Vector3(3, 3, 3);
        transform.Rotate(90, 0, 0);
    }

    public override void OnStep()
    {
        base.OnStep();

        int StepToVisualze = GameManager.instance.currentPathPosIndex + Offset;
        if (StepToVisualze > GridManager.Path.Length)
        {
            enabled = false;
            return;
        }

        //add path markers
        var Cell = GridManager.Path[StepToVisualze];
        var Position = Cell.transform.position + new Vector3(0, 0.1f, 0);
        transform.position = Position;
    }
}
