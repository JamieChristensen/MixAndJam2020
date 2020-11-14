using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using GameJam.Events;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;


public class GameManager : SerializedMonoBehaviour
{

    public static GameManager instance;

    [BoxGroup("Dependencies")]
    [SerializeField]
    private GridManager gridManager;

    [BoxGroup("Player stuff")]
    private int2 currentPlayerPosition;

    [BoxGroup("Step Timer")]
    [Header("Step related stuff")]
    [Tooltip("The current step in this round. For score, eventually.")]
    [ReadOnly]
    public int stepCount;

    [BoxGroup("Step Timer")]
    [SerializeField]
    private float2 stepInputInterval;

    [BoxGroup("Step Timer")]
    [SerializeField]
    public float stepDuration;

    [BoxGroup("Step Timer")]
    [ReadOnly]
    [ShowInInspector]
    private float stepTimer;

    [BoxGroup("Step Timer")]
    [ShowInInspector]
    [ReadOnly]
    private bool hasRaisedStepEventThisStep;

    [BoxGroup("Player stuff")]
    [ReadOnly]
    public int currentPathPosIndex;

    [BoxGroup("Player stuff")]
    public PlayerAction defaultAction;

    [Header("Player and Units")]
    [BoxGroup("Player stuff")]
    [SerializeField]
    public GameObject playerGO;

    [ReadOnly]
    [HideInEditorMode]
    public List<StepUnit> stepUnits;

    [Header("Events")]
    [BoxGroup("Step Timer")]
    public VoidEvent managerStepEvent;

    public PlayerAction PreviousAction;

    bool HasDonePlayerAction = false;

    [SerializeField]
    private LineRenderer playerCellVisualizer;

    private void Start()
    {
        Init(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += Init;
    }

    private void Init(Scene scene, LoadSceneMode mode)
    {
        //Get level-data and units.
        hasRaisedStepEventThisStep = false;

        stepUnits.Clear();
        stepUnits.AddRange(FindObjectsOfType<StepUnit>());

        playerGO = GameObject.FindGameObjectWithTag("Player");
        MovePlayerToNextPointOnPath();



    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //GetPlayerInput and assign to recentInput:
        stepTimer += Time.deltaTime;
        UpdateCellVisualizer();

        if (stepTimer > stepDuration - stepInputInterval.x)
        {
            if (stepTimer >= stepDuration && !hasRaisedStepEventThisStep)
            {
                //Debug.Log("In here");
                managerStepEvent.Raise();
                hasRaisedStepEventThisStep = true;
                if (managerStepEvent != null)
                {
                    Debug.Log("Raised step-event ");
                }
            }

            //Grace period for Input to player:
            if (stepTimer >= stepDuration + stepInputInterval.y && !HasDonePlayerAction)
            {
                PlayerStep(defaultAction);
                return;
            }

            if (stepTimer > stepDuration && HasDonePlayerAction)
            {
                stepTimer = stepDuration - stepTimer; //Instead of setting to 0, which would cause minor beat-offsets over time.
                stepCount++;
                hasRaisedStepEventThisStep = false;
                HasDonePlayerAction = false;
            }
        }
    }

    private void UpdateCellVisualizer()
    {
        if (playerCellVisualizer == null)
        {
            playerCellVisualizer = GetComponent<LineRenderer>();
        }

        Cell activeCell = CellPlayerIsOn();

        Vector3[] points = new Vector3[4];
        Bounds bounds = activeCell.GetComponent<MeshRenderer>().bounds;

        Vector3 center = new Vector3(playerGO.transform.position.x, 0f, playerGO.transform.position.z);
        float t = Map(Mathf.Clamp(stepTimer, 0, stepDuration), 0 - stepInputInterval.x, stepDuration, 0, 1);

        t = Mathf.Clamp(t, 0.001f, 0.999f);

        float tWidth = Mathf.Clamp01(1.2f - t);
        playerCellVisualizer.startWidth = tWidth;
        playerCellVisualizer.endWidth = tWidth;

        var offset = new Vector3(playerGO.transform.position.x, 0, playerGO.transform.position.z);



        points[0] = offset + new Vector3(bounds.max.x - bounds.center.x, 0f, bounds.max.z - bounds.center.z);
        points[0] = Vector3.Lerp(points[0], center, t);
        points[1] = offset + new Vector3(bounds.max.x - bounds.center.x, 0f, bounds.min.z - bounds.center.z);
        points[1] = Vector3.Lerp(points[1], center, t);
        points[2] = offset + new Vector3(bounds.min.x - bounds.center.x, 0f, bounds.min.z - bounds.center.z);
        points[2] = Vector3.Lerp(points[2], center, t);
        points[3] = offset + new Vector3(bounds.min.x - bounds.center.x, 0f, bounds.max.z - bounds.center.z);
        points[3] = Vector3.Lerp(points[3], center, t);

        playerCellVisualizer?.SetPositions(points);
    }

    private float Map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    public bool IsInRange(float time)
    {
        if (time < stepDuration - stepInputInterval.x)
        {
            return false;
        }

        if (time > stepDuration + stepInputInterval.y)
        {
            return false;
        }

        return true;
    }

    public void InRangePlayerStep(PlayerAction action)
    {
        if (IsInRange(stepTimer))
        {
            PlayerStep(action);
        }
    }

    public void PlayerStep(PlayerAction action)
    {
        action.ResolvePlayerAction(this);

        foreach (StepUnit stepUnit in stepUnits)
        {
            stepUnit.OnStep();
        }

        PreviousAction = action;

        HasDonePlayerAction = true;
    }

    public void MovePlayerToNextPointOnPath()
    {
        if (currentPathPosIndex >= gridManager.Path.Length - 1)
        {
            Debug.Log("End of path");
            return;
        }

        Vector3 newPos = gridManager.Path[currentPathPosIndex + 1].transform.position;
        newPos.y = 0;

        currentPathPosIndex++;

        //Visual movement
        playerGO.transform.LookAt(newPos);
        StartCoroutine(LerpToPositon(newPos));
        playerGO.GetComponent<Animator>().Play("Sprint");

    }

    IEnumerator LerpToPositon(Vector3 pos)
    {
        Vector3 fromPos = playerGO.transform.position;
        float timeElapsed = 0;
        float lerpDuration = 0.5f;
        while (timeElapsed < lerpDuration)
        {
            playerGO.transform.position = Vector3.Lerp(fromPos, pos, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

    }

    private Cell CellPlayerIsOn()
    {
        return gridManager.Path[currentPathPosIndex];
    }
    private int2 PlayerPositionOnGrid()
    {
        return gridManager.Path[currentPathPosIndex].point;
    }

    public void PlayerAttackAction()
    {
        int2 playerPos = PlayerPositionOnGrid();

        Debug.Log(playerPos);
        Debug.Log(gridManager.GetCellByPoint(playerPos).point);

        StepUnit disguydead;
        disguydead = gridManager.GetCellByPoint(playerPos).transform.GetComponentInChildren<StepUnit>();
        //Destroy(gridManager.GetCellByPoint(playerPos));

        Debug.Log(gridManager.GetCellByPoint(playerPos).transform.GetComponentInChildren<StepUnit>());

        disguydead?.IsKill();

    }

}
