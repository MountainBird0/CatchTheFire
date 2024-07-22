using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SandBagInteractionMachine : InteractionMachine
{
    public SandBag sandBag;
    void Start()
    {
        interactionMap.Add(Tag.Fire, InteractWithFire);
        interactionMap.Add(Tag.Criminal, InteractWithCriminal);

    }

    #region Fire
    public void InteractWithFire(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterFire(other);
    }
    private void EnterFire(GameObject other)
    {
        Fire fire = other.gameObject.GetComponent<Fire>();
        if (sandBag.condition.hp.curValue.Value > (int)fire.burnState.Value)
        {
            sandBag.condition.hp.curValue.Value -= (int)fire.burnState.Value;
            //if (sandBag.condition.hp.curValue.Value <= 0)
            //    sandBag.GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            // sandBag.GetComponent<NetworkObject>().Despawn();
            sandBag.DestroySand();
        }
    }
    #endregion

    #region Criminal
    public void InteractWithCriminal(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterCriminal(other);
        else ExitCriminal(other);
    }
    private void EnterCriminal(GameObject other)
    {
        sandBag.condition.hpChangeRate.Value = -1;
        sandBag.SetIsInSandBag(true);
    }
    private void ExitCriminal(GameObject other)
    {
        sandBag.condition.hpChangeRate.Value = 0;
        sandBag.SetIsInSandBag(false);
    }
    #endregion


}
