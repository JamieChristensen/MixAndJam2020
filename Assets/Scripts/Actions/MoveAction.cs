using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Actions/Move")]
public class MoveAction : PlayerAction
{
    public override void ResolvePlayerAction(GameManager gameManager)
    {
        gameManager.MovePlayerToNextPointOnPath();
    }
}
