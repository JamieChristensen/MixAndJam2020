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

    [SerializeField] //Assigning in inspector
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

        playerGO = Instantiate(playerGO);
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
        stepTimer += Time.deltaTime;
        UpdateCellVisualizer();


        if (stepTimer > stepDuration - stepInputInterval.x)
        {
            if (stepTimer >= stepDuration && !hasRaisedStepEventThisStep)
            {
                Debug.Log("In here");
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
        Cell activeCell = CellPlayerIsOn();

        Vector3[] points = new Vector3[5];
        Bounds bounds = activeCell.GetComponent<MeshRenderer>().bounds;

        points[0] = new Vector3(bounds.max.x, 0.2f, bounds.max.z);
        points[1] = new Vector3(bounds.max.x, 0.2f, bounds.min.z);
        points[2] = new Vector3(bounds.min.x, 0.2f, bounds.min.z);
        points[3] = new Vector3(bounds.min.x, 0.2f, bounds.max.z);
        points[4] = new Vector3(bounds.max.x, 0.2f, bounds.max.z);

        playerCellVisualizer?.SetPositions(points);
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
        if (currentPathPosIndex >= gridManager.Path.Length)
        {
            Debug.Log("End of path");
            return;
        }
        Vector3 newPos = gridManager.Path[currentPathPosIndex].transform.position;
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

        StepUnit disguydead;
        disguydead = gridManager.GetCellByPoint(playerPos).GetComponentInChildren<StepUnit>();

        disguydead?.IsKill();

    }

}
