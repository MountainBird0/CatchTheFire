using UnityEngine;

public class IdleState : BaseState
{


    public IdleState(PlayerController playerController) : base(playerController)
    {
    }

    public override void OnStateEnter()
    {
        string idelAnimation = playerController.lastInputDir.z > 0 ? "BackIdle" : "FrontIdle";
        if ((!playerController.animation.IsPlaying(idelAnimation)))
        {
            playerController.animation.PlayAnimationServerRpc(idelAnimation, playerController.lastInputDir);
        }
    }

    public override void OnStateExit()
    {
    }

    public override void OnStateUpdate()
    {
        Vector3 inputDir = new Vector3(InputManager.instance.moveX, 0, InputManager.instance.moveZ);

        if (inputDir != Vector3.zero)
        {
          playerController.SetState(new MoveState(playerController));
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= playerController.lastDashTime + playerController.dashCoolTime)
            playerController.SetState(new DashState(playerController));
    }
}
