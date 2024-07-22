using Unity.Netcode;
using UnityEngine;

public class FireCondition : NetworkBehaviour
{
    public Condition firePoint;

    public NetworkVariable<float> firePointChangeRate = new(0f);

    public override void OnNetworkSpawn()
    {
        
    }

    private void Update()
    {
        //if(IsServer)
        //{
        //    firePoint.SetCurValueWithChangeLate(firePointChangeRate.Value * Time.deltaTime);
        //}
    }
}
