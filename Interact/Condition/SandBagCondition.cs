using Unity.Netcode;
using UnityEngine;

public class SandBagCondition : NetworkBehaviour
{
    public Condition hp;
    // public Condition sendboxtimePoint;
    public NetworkVariable<float> hpChangeRate = new(0f);


    public override void OnNetworkSpawn()
    {
        hpChangeRate.Value = 0f;
    }

    private void Update()
    {
        if(IsServer)
        {
            hp.SetCurValueWithChangeLate(hpChangeRate.Value * Time.deltaTime);
        }
    }
}
