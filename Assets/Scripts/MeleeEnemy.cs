using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using GameJam.Events;

public class MeleeEnemy : StepUnit
{
    [SerializeField]
    private GameObject deathParticles;

    [SerializeField]
    private GameObject playerDeathParticles;

    [SerializeField]
    private VoidEvent meleeDeath;

    private void Start()
    {
        shouldStep = true;
    }

    public override void OnStep()
    {
        base.OnStep();

        if (!shouldStep) return;

        bool isPlayerOnPos = false;
        int2 playerPos = GameManager.instance.PlayerPositionOnGrid();
        isPlayerOnPos = playerPos.Equals(gridPosition);

        Debug.Log("Is player on this enemy position?: " + isPlayerOnPos + "\n" + " playerPos / enemyPos: " + playerPos + " / " + gridPosition);
        if (isPlayerOnPos)
        {
            StartCoroutine(WaitThenKILL(GameManager.instance.stepDuration*0.5f));
        }
        //Debug.Log("step");

        //Do step-stuff.
        //if(grid.isPlayerOnPos) 
        //Needs grid-reference to kill player.
    }

    IEnumerator WaitThenKILL(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(GameManager.instance.KillPlayer());
        Instantiate(playerDeathParticles, GameManager.instance.playerGO.transform.position, Quaternion.identity);
        Time.timeScale = 0.3f;
        yield return null;
    }

    public override void IsKill()
    {
        base.IsKill();

        meleeDeath.Raise();
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject, 0f);

    }
}
