using System.Globalization;
using UnityEngine;

public class StunState : BaseState
{
    public StunState(PlayerController playerController) : base(playerController)
    {
    }

    public override void OnStateEnter()
    {
        playerController.animation.PlayAnimationServerRpc("Stun", playerController.lastInputDir);
    }
}
