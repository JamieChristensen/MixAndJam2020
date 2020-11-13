using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : StepUnit
{
    
    private void Update()
    {

    }


    public override void OnStep()
    {
        base.OnStep();

        Debug.Log("step");
        
        //Do step-stuff.
        //if(grid.isPlayerOnPos) 
            //Needs grid-reference to kill player.


    }

}
