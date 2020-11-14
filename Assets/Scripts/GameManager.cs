using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using GameJam.Events;


public class GameManager : MonoBehaviour
{

    [Header("Level related")]
    [SerializeField]
    private GridManager gridManager;

    private int2 currentPlayerPosition;



    [Header("Step related stuff")]
    [Tooltip("The current step in this round. For score, eventually.")]
    [SerializeField]
    private int stepCount;

    [SerializeField]
    private float2 stepInputInterval;

    [SerializeField]
    private float stepDuration;
    private float stepTimer;

    private bool hasRaisedStepEventThisStep;

    public int currentPathPosIndex;


    [Header("Inputs")]
    public KeyCode attackKey;
    public KeyCode moveKey, deflectKey;

    public PlayerAction defaultAction;

    [Header("Player and Units")]
    [SerializeField]
    private GameObject playerGO;
    public List<StepUnit> stepUnits;

    [Header("Events")]
    public VoidEvent managerStepEvent;

    private void Start()
    {
        //Get level-data and units.
        hasRaisedStepEventThisStep = false;

        stepUnits.Clear();
        stepUnits.AddRange(FindObjectsOfType<StepUnit>());
    }

    // Update is called once per frame
    private void Update()
    {
        //GetPlayerInput and assign to recentInput:


        stepTimer += Time.deltaTime;


        if (stepTimer > stepDuration && !hasRaisedStepEventThisStep)
        {
            managerStepEvent?.Raise();
            hasRaisedStepEventThisStep = true;
        }

        //Grace period for Input to player:
        if (stepTimer > stepDuration && stepTimer < stepDuration + stepInputInterval.y)
        {
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

    public void MovePlayerToNextPointOnPath(int pathIndex)
    {   
        Vector3 newPos = gridManager.Path[pathIndex].transform.position;

        playerGO.transform.Translate(newPos.x, 0, newPos.z);

        currentPathPosIndex++;
    }

}
