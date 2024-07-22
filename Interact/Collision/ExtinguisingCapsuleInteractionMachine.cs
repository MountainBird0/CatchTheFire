using Unity.Netcode;
using UnityEngine;

public class ExtinguisingCapsuleInteractionMachine : InteractionMachine
{
    public ExtinguishingCapsule extinguishingCapsule;

    private void Start()
    {
        interactionMap.Add(Tag.FireFighter, InteractWithCapsule);
        interactionMap.Add(Tag.Fire, InteractWithFire);
    }

    public void InteractWithFire(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if(isEnter) EnterFire(other);
    }

    public void InteractWithCapsule(GameObject other, bool isEnter)
    {
        if (!IsServer) return;

        if (isEnter) EnterFireFighter(other);
    }


    private void EnterFireFighter(GameObject other)
    {
        extinguishingCapsule.AddCapsule();
    }

    private void EnterFire(GameObject other)
    {
        //Fix
        extinguishingCapsule.ExplosionCapsule();
    }
    
}
