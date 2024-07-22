using UnityEngine;

public class MoveState : BaseState
{
    private Rigidbody rigid;
    public MoveState(PlayerController playerController) : base(playerController)
    {
        rigid = playerController.GetComponent<Rigidbody>();
    }

    public override void OnStateEnter()
    {

    }
    public override void OnStateUpdate()
    {
        Vector3 inputDir = new Vector3(InputManager.instance.moveX, 0, InputManager.instance.moveZ);

        if (inputDir == Vector3.zero)
            playerController.SetState(new IdleState(playerController));

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= playerController.lastDashTime + playerController.dashCoolTime)
            playerController.SetState(new DashState(playerController));
    }

    public override void OnStateFixedUpdate()
    {
        Move();
    }

    public override void OnStateExit()
    {

    }

    private void Move()
    {
        Vector3 inputDir = new Vector3(InputManager.instance.moveX, 0, InputManager.instance.moveZ);

        string animationName = (inputDir.z > 0) ? "BackWalk" : "FrontWalk";

        playerController.animation.SwitchFlipServerRpc(inputDir);

        if (playerController.animation != null && (!playerController.animation.IsPlaying(animationName)))
            playerController.animation.PlayAnimationServerRpc(animationName, inputDir);


        playerController.transform.GetChild(0).transform.rotation = Quaternion.LookRotation(inputDir);
        playerController.moveVec = inputDir * playerController.moveSpeed.Value * Time.deltaTime;
        rigid.MovePosition(rigid.position + playerController.moveVec);
    }
}
