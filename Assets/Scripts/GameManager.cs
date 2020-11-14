using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using GameJam.Events;
using Sirenix.OdinInspector;


public class GameManager : SerializedMonoBehaviour
{

    [BoxGroup("Dependencies")]
    [SerializeField]
    private GridManager gridManager;

    [BoxGroup("Player stuff")]
    private int2 currentPlayerPosition;

    [BoxGroup("Step Timer")]
    [Header("Step related stuff")]
    [Tooltip("The current step in this round. For score, eventually.")]
    [ReadOnly]
    [SerializeField]
    private int stepCount;

    [BoxGroup("Step Timer")]
    [SerializeField]
    private float2 stepInputInterval;

    [BoxGroup("Step Timer")]
    [SerializeField]
    private float stepDuration;

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
    private GameObject playerGO;

    [ReadOnly]
    public List<StepUnit> stepUnits;

    [Header("Events")]
    [BoxGroup("Step Timer")]
    public VoidEvent managerStepEvent;

    private void Start()
    {
        //Get level-data and units.
        hasRaisedStepEventThisStep = false;

        stepUnits.Clear();
        stepUnits.AddRange(FindObjectsOfType<StepUnit>());

        playerGO = Instantiate(playerGO);
        MovePlayerToNextPointOnPath();
    }

    // Update is called once per frame
    private void Update()
    {
        //GetPlayerInput and assign to recentInput:
        stepTimer += Time.deltaTime;

        if (stepTimer < stepDuration - stepInputInterval.x)
        {
            return;
        }


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
        if (stepTimer >= stepDuration && stepTimer <= stepDuration + stepInputInterval.y)
        {
            Debug.Log("SUP");
            return;
        }

        //Grace period over
        PlayerStep(defaultAction);
        //Prepare next step-waiting:
        stepTimer = stepDuration - stepTimer; //Instead of setting to 0, which would cause minor beat-offsets over time.
        stepCount++;
        hasRaisedStepEventThisStep = false;
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

    public void PlayerStep(PlayerAction action)
    {
        if (!hasRaisedStepEventThisStep && IsInRange(stepTimer))
        {
            hasRaisedStepEventThisStep = true;

            action.ResolvePlayerAction(this);

            foreach (StepUnit stepUnit in stepUnits)
            {
                stepUnit.OnStep();
            }

            hasRaisedStepEventThisStep = false;
        }
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

        playerGO.transform.position = newPos;

        currentPathPosIndex++;
    }

}
