using UnityEngine;

public class FireInteractionMachine : InteractionMachine
{
    public Fire fire;

    private void Start()
    {
        interactionMap.Add(Tag.Criminal, InteractWithCriminal);
        interactionMap.Add(Tag.Sand, InteractWithSand);
        interactionMap.Add(Tag.Capsule, InteractWithCapsule);
        interactionMap.Add(Tag.Water, InteractWithWater);
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
        if (other.TryGetComponent<Criminal>(out var criminal))
        {
            fire.fireState.Value |= EFireState.ON_CRIMINAL;
            //fire.condition.firePointChangeRate.Value = fire.stats.hpChangeRate * 2f;
        }
    }
    private void ExitCriminal(GameObject other)
    {
        if (other.TryGetComponent<Criminal>(out var criminal))
        {
            fire.fireState.Value &= ~EFireState.ON_CRIMINAL;
            //fire.condition.firePointChangeRate.Value = fire.stats.hpChangeRate;
        }
    }
    #endregion

    public void InteractWithSand(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<SandBag>(out var sand))
        {
            if(sand.condition.hp.curValue.Value >= (int)fire.burnState.Value)
            {
                Destroy(this.gameObject);
            }
        }    
    }

    public void InteractWithCapsule(GameObject other, bool isEnter)
    {
        if (!IsServer) return;
        Destroy(gameObject);
    }

    #region Water
    public void InteractWithWater(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterWater(other);
        else ExitWater(other);
    }

    private void EnterWater(GameObject other)
    {
        if (other.TryGetComponent<Water>(out var water))
        {
            fire.Extinguish();
        }
    }
    private void ExitWater(GameObject other)
    {
        if (other.TryGetComponent<Water>(out var water))
        {
            water.OnShootCancle -= ExitWater;
            fire.fireState.Value &= ~EFireState.IN_WATER;
            //fire.condition.firePointChangeRate.Value =
            //(fire.fireState.Value & FireState.ON_CRIMINAL) != 0 ? 2f : fire.stats.hpChangeRate;
        }
    }
    #endregion
}
