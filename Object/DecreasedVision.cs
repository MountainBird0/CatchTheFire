using System;
using UnityEngine;

public class DecreasedVision : TileObject
{
    private DecreasedVisionMachine decreasedVisionMachine;
    private void Awake()
    {
        decreasedVisionMachine = GetComponent<DecreasedVisionMachine>();
        decreasedVisionMachine.decreasedVision = this;
    }
    public override void OnDestroy()
    {
        if (BattleMapManager.Instance != null)
            BattleMapManager.Instance.OnDestoryDecreasedVision();
    }
}
