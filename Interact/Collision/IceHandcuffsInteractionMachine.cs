using Unity.Netcode;
using UnityEngine;

public class IceHandcuffsInteractionMachine : InteractionMachine
{
    public IceHandcuffs iceHandcuffs;
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
        if(other.TryGetComponent<Criminal>(out var criminal))
        {
            if ((criminal.state.Value & ECriminalState.IS_STUNNED) != 0) return;
            if ((criminal.state.Value & ECriminalState.IS_CAUGHTED) != 0) return;

            iceHandcuffs.condition.hpChangeRate.Value = -1f;
        }
    }
    private void ExitCriminal(GameObject other)
    {
        if (other.TryGetComponent<Criminal>(out var criminal))
        {
            if ((criminal.state.Value & ECriminalState.IS_STUNNED) == 0 ||
                (criminal.state.Value & ECriminalState.IS_CAUGHTED) == 0)
            {
                iceHandcuffs.condition.hpChangeRate.Value = 0f;
            }
        }
    }
    #endregion
}