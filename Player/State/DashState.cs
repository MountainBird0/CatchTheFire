using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DashState : BaseState
{
    private float dashTimer = 0f;
    private float dashDuration = 0.2f;
    private float dashSpeed = 8f;
    public DashState(PlayerController playerController) : base(playerController)
    {
    }

    public override void OnStateEnter()
    {
        playerController.isDash = true;
        playerController.lastDashTime = Time.time;
        dashTimer = 0f;
    }
    public override void OnStateUpdate()
    {
        if (!playerController.isDash)
            playerController.SetState(new IdleState(playerController));
    }

    public override void OnStateFixedUpdate()
    {
        Dash();
    }

    public override void OnStateExit()
    {

    }

    private void Dash()
    {
        if (playerController.isDash)
        {
            dashTimer += Time.deltaTime;
            if(dashTimer < dashDuration)
            {
                Vector3 inputDir = playerController.lastInputDir != Vector3.zero ? playerController.lastInputDir : playerController.transform.GetChild(0).forward;
                playerController.moveVec = inputDir * dashSpeed * Time.deltaTime;
                playerController.rigidbody.MovePosition(playerController.rigidbody.position + playerController.moveVec);
            }
            else
            {
                playerController.isDash = false;
                dashTimer = 0f;
            }
        }
    }
}
