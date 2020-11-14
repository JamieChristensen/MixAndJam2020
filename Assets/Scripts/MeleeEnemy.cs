using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeEnemy : StepUnit
{
    [SerializeField]
    private GameObject deathParticles;
    
    private void Start()
    {
        shouldStep = true;
    }

    public override void OnStep()
    {
        base.OnStep();

        Debug.Log("step");

        //Do step-stuff.
        //if(grid.isPlayerOnPos) 
        //Needs grid-reference to kill player.
    }

    public override void IsKill()
    {
        base.IsKill();
        
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(this, 0.1f);

    }
}
