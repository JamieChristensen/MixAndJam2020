using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine.SceneManagement;

public class GridManager : SerializedMonoBehaviour
{
    [SerializeField]
    private LevelManager LevelManager;

    public Cell[] Path;
    public GameObject pathMarker;

    private float2 CellSize;
    private Cell[,] Cells;



    void Awake()
    {
        var Children = GetComponentInChildren<Transform>();
        if (Children == null) Init();
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
        if (Level.MeleeEnemies != null)
        {
            foreach (var EnemyEntry in Level.MeleeEnemies)
            {
                Cell Cell = Cells[EnemyEntry.Key.x, EnemyEntry.Key.y];
                Cell.SpawnStepUnit(EnemyEntry.Value);
            }
        }

        if (Level.RangedEnemies != null)
        {
            foreach (var EnemyEntry in Level.RangedEnemies)
            {
                Cell Cell = Cells[EnemyEntry.Key.x, EnemyEntry.Key.y];
                var RangedEnemyEntry = EnemyEntry.Value.Enemy;
                RangedEnemyEntry.ShootStep = EnemyEntry.Value.StepArgument;

                Cell.SpawnStepUnit(RangedEnemyEntry);
            }
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
            
            //add path markers
            Vector3 pos = Cells[Position.x, Position.y].transform.position + new Vector3(0,0.1f,0);
            GameObject inst = Instantiate(pathMarker, pos, quaternion.identity);
            inst.transform.localScale = new Vector3(3, 3, 3);
            inst.transform.Rotate(90,0,0);
        }
    }

    public Level GetCurrentLevel() => LevelManager.GetCurrentLevel();
}
