using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameJam.Events;

[CreateAssetMenu(menuName = "Player Actions/Attack")]
public class AttackAction : PlayerAction
{
    public override void ResolvePlayerAction(GameManager gameManager)
    {
        gameManager.PlayerAttackAction();
    }
}
