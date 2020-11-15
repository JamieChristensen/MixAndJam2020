using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


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

        bool isPlayerOnPos = false;
        int2 playerPos = GameManager.instance.PlayerPositionOnGrid();
        isPlayerOnPos = playerPos.Equals(gridPosition);

        Debug.Log("Is player on this enemy position?: " + isPlayerOnPos + "\n" + " playerPos / enemyPos: " + playerPos + " / " + gridPosition);
        if (isPlayerOnPos)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.KillPlayer());
        }
        //Debug.Log("step");

        //Do step-stuff.
        //if(grid.isPlayerOnPos) 
        //Needs grid-reference to kill player.
    }

    public override void IsKill()
    {
        base.IsKill();
        
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject, 0f);

    }
}
