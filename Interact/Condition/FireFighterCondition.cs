using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

public class FireFighterCondition : NetworkBehaviour
{
    public Condition resistance;
    public Condition extinguishing;

    public NetworkVariable<float> resistancePointChangeRate = new(0f);
    public NetworkVariable<float> extinguisingPointChangeRate = new(0f);

    //public NetworkVariable<float> extingusingChargeRate = new(0);

    public override void OnNetworkSpawn()
    {
        //resistance.curValue.Value = 100;
        //extinguishing.curValue.Value = 50;
        //extinguishing.maxValue.Value = 200;

        //extinguisingPointChangeRate.OnValueChanged += UseExtinguising;
    }

    public void Update()
    {
        //if (!IsServer) return;

        //if (resistancePointChangeRate.Value == 0) return;

        if (resistancePointChangeRate.Value == 0) return;

        if (IsServer)
        {
            resistance.SetCurValueWithChangeLate(resistancePointChangeRate.Value * Time.deltaTime);
        }


        //if (extingusingChargeRate.Value != 0)
        //    extinguishing.SetCurValueWithChangeLate(extingusingChargeRate.Value * Time.deltaTime);
    }

    public void UseExtinguising(float value) 
    {
        if (!IsServer) return;

        extinguishing.SetCurValueWithChangeLate(value);
    }

    
}
