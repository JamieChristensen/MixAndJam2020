using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;
using GameJam.Events;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Inputs")]
    public KeyCode attackKey;
    public KeyCode moveKey, deflectKey;

    public Action defaultAction;

    [Header("Step-related stuff")]
    public List<StepUnit> stepUnits;

    [SerializeField]
    private float stepInputInterval;

    [SerializeField]
    private float stepDuration;
    private float stepTimer;

    [Tooltip("The current step in this round. For score, eventually.")]
    [SerializeField]
    private int stepCount;



    [Header("Player object related stuff")]
    [SerializeField]
    private GameObject playerGO;
    private int2 playerPosition;

    [Header("Events")]
    public VoidEvent managerStepEvent;


    public enum Action
    {
        Stand, Attack, Move, Deflect
    }

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Get level-data and units.
    }

    // Update is called once per frame
    private void Update()
    {
        stepTimer += Time.deltaTime;

        bool isTimerOverLowerBound = stepTimer > stepDuration - stepInputInterval;
        bool isTimerBelowUpperBound = stepTimer < stepDuration + stepInputInterval;

        if (!isTimerOverLowerBound)
        {
            //Early quit, since timing is outside interval.
            return;
        }

        if (ActionFromInput() == defaultAction && isTimerBelowUpperBound)
        {
            return;
        }
        //Upper bound is now either exceeded or input within interval has been received:


        PlayerStep();


        foreach (StepUnit stepUnit in stepUnits)
        {
            stepUnit.OnStep();
        }

        managerStepEvent?.Raise();



        //Prepare next step-waiting:
        stepTimer = stepDuration - stepTimer; //Instead of setting to 0, which would cause minor beat-offsets over time.
        stepCount++;
    }


    private void PlayerStep()
    {
        Action action = ActionFromInput();

        int xMove = stepCount % 2 == 1 ? -1 : 1;
        playerGO.transform.Translate(xMove, 0, 0);

        switch (action)
        {
            case Action.Attack:

                break;

            case Action.Stand:

                break;

            case Action.Move:
                int yMove = stepCount % 2 == 1 ? -1 : 1;
                playerGO.transform.Translate(0, yMove, 0);
                break;

            case Action.Deflect:

                break;

            default:

                break;
        }



        //TODO: Do something to playerPosition based on input and action. 
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
