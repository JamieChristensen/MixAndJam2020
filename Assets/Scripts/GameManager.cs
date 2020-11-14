using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


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



    [Header("Player related stuff")]
    [SerializeField]
    private GameObject playerGO;
    private int2 playerPosition;





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
        if (stepTimer < stepDuration)
        {
            return;
        }
        stepTimer = stepDuration - stepTimer; //Instead of setting to 0, which would cause minor beat-offsets over time.


        PlayerStep();


        foreach (StepUnit stepUnit in stepUnits)
        {
            stepUnit.OnStep();
        }

        //Play beat-sounds

        //P

        stepCount++;
    }


    private void PlayerStep()
    {
        int yMove = stepCount % 2 == 1 ? -1 : 1;
        playerGO.transform.Translate(0, yMove, 0);

        //TODO: Do something to playerPosition based on input and action. 
        
        
    }


    
}
