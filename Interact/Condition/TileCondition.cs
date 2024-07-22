using Unity.Netcode;
using UnityEngine;

public class TileCondition : NetworkBehaviour
{
    public Condition hp;
    public NetworkVariable<float> hpChangeRate = new(0f);

    private void Update()
    {
        if (IsServer)
        {
            hp.SetCurValueWithChangeLate(hpChangeRate.Value * Time.deltaTime);
        }
    }
}
