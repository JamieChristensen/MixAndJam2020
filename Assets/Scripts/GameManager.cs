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

    private int currentPathPosIndex;


    [Header("Inputs")]
    public KeyCode attackKey;
    public KeyCode moveKey, deflectKey;

    public Action defaultAction;

    [Header("Player and Units")]
    [SerializeField]
    private GameObject playerGO;
    public List<StepUnit> stepUnits;

    [Header("Events")]
    public VoidEvent managerStepEvent;


    public enum Action
    {
        Stand, Attack, Move, Deflect
    }

    // Start is called before the first frame update
    private void Awake()
    {

    }

    private void Start()
    {
        //Get level-data and units.
        hasRaisedStepEventThisStep = false;
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




    public void PlayerStep(Action action)
    {

        switch (action)
        {
            case Action.Attack:

                break;

            case Action.Stand:

                break;

            case Action.Move:
                MovePlayerToNextPointOnPath(currentPathPosIndex);
                break;

            case Action.Deflect:

                break;

            default:

                break;
        }


        foreach (StepUnit stepUnit in stepUnits)
        {
            stepUnit.OnStep();
        }
        hasRaisedStepEventThisStep = false;
    }

    void MovePlayerToNextPointOnPath(int pathIndex)
    {   
        Vector3 newPos = gridManager.Path[pathIndex].transform.position;

        playerGO.transform.Translate(newPos.x, 0, newPos.z);

        currentPathPosIndex++;
    
    }

    private Action ActionFromInput()
    {
        if (Input.GetKeyDown(attackKey))
        {
            return Action.Attack;
        }

        if (Input.GetKeyDown(moveKey))
        {
            return Action.Move;
        }

        if (Input.GetKeyDown(deflectKey))
        {
            return Action.Deflect;
        }

        return defaultAction;
    }

}
