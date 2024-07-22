using UnityEngine;

public abstract class BaseState
{
    protected PlayerController playerController;

    protected BaseState(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public virtual void OnStateEnter() { }
    public virtual void OnStateUpdate() { }
    public virtual void OnStateFixedUpdate() { }
    public virtual void OnStateExit() { }
}
