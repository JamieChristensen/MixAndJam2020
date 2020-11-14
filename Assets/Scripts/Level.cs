using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    public Cell[,] Grid;
    public Vector2Int[] Path;
}
