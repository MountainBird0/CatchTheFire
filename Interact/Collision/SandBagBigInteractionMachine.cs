using UnityEngine;

public class SandBagBigInteractionMachine : InteractionMachine
{
    public SandBag sandBag;
    void Start()
    {
        interactionMap.Add(Tag.Criminal, InteractWithCriminal);
    }

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
