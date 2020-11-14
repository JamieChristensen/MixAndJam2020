using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class Level : SerializedScriptableObject
{
    [TableMatrix(HorizontalTitle="Grid Layout", SquareCells=true)]
    public Cell[,] Grid;
    public int2[] Path;
}
