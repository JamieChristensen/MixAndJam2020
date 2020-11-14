using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Sirenix.OdinInspector;
using System.Linq;

public class GridManager : SerializedMonoBehaviour
{
    [SerializeField]
    private LevelManager LevelManager;

    public Cell[] Path;

    private float2 CellSize;
    private Cell[,] Cells;

    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    [Button(ButtonSizes.Large)]
    private void Init()
    {
        Level CurrentLevel = LevelManager.GetCurrentLevel();
        Cell Cell = CurrentLevel.Grid[0, 0];

        var Renderer = Cell.GetComponent<Renderer>();
        var Bounds = Renderer.bounds;
        var BoundsDistance = Bounds.max - Bounds.min;
        CellSize.x = BoundsDistance.x;
        CellSize.y = BoundsDistance.z;


        GenerateGrid(CurrentLevel);

        GeneratePath(CurrentLevel);

        SpawnEnemies(CurrentLevel);
    }

    [Button(ButtonSizes.Large)]
    private void NukeGrid()
    {
        List<Transform> transforms = transform.GetComponentsInChildren<Transform>().ToList<Transform>();
        foreach (Transform trans in transforms)
        {
            if (trans != null && trans.gameObject != this.gameObject)
            {
                DestroyImmediate(trans.gameObject);
            }
        }
    }

    private void GenerateGrid(Level Level)
    {
        int LevelSizeX = Level.Grid.GetLength(0);
        int LevelSizeY = Level.Grid.GetLength(1);

        Cells = new Cell[LevelSizeX, LevelSizeY];

        for (int i = 0; i < LevelSizeX; i++)
        {
            for (int j = 0; j < LevelSizeY; j++)
            {
                Cells[i, j] = Instantiate(Level.Grid[i, j], new Vector3(CellSize.x * i, 0, CellSize.y * j), Quaternion.identity, this.transform);
                Cells[i, j].point = new int2(i,j);
            }
        }
    }





    private void SpawnEnemies(Level Level)
    {
        foreach (var EnemyEntry in Level.Enemies)
        {
            Cell Cell = Cells[EnemyEntry.Key.x, EnemyEntry.Key.y];
            Cell.SpawnStepUnit(EnemyEntry.Value);
        }
    }

    public Cell GetCellByPoint(int2 point)
    {
        return Cells[point.x, point.y];
    }

    private void GeneratePath(Level Level)
    {
        Path = new Cell[Level.Path.Length];
        for (int i = 0; i < Level.Path.Length; i++)
        {
            int2 Position = Level.Path[i];
            Path[i] = Cells[Position.x, Position.y];
        }
    }

    public Level GetCurrentLevel() => LevelManager.GetCurrentLevel();
}
