using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<StepUnit> stepUnits;

    [SerializeField]
    private float stepDuration;
    private float stepTimer;

    [Tooltip("The current step in this round. For score, eventually.")]
    [SerializeField]
    private int stepCount;

    [Header("Player related")]
    [SerializeField]
    private GameObject playerGO;



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
        stepTimer = 0;



        foreach (StepUnit stepUnit in stepUnits)
        {
            stepUnit.OnStep();
        }


    }
}
