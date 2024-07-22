using Unity.Netcode;
using UnityEngine;

public class IceHandcuffsCondition : NetworkBehaviour
{
    public Condition hp;

    public NetworkVariable<float> hpChangeRate = new(0f);

    private void Update()
    {
        if (hpChangeRate.Value == 0) return;

        if (IsServer)
        {
            hp.SetCurValueWithChangeLate(hpChangeRate.Value * Time.deltaTime);
        }
    }
}
