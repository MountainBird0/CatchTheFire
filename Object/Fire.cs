using FireStatusTable;
using FlyingWormConsole3.LiteNetLib;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Fire : TileObject
{
    public AllObjectStats.ObjectData stats_Legacy = new AllObjectStats.ObjectData();
    public FireData stats;

    public NetworkVariable<BurnState> burnState = new(0);
    public NetworkVariable<EFireState> fireState = new(0);


    public FireInteractionMachine interactionMachine;

    public UnityAction<GameObject> OnExtinguish;

    public Tile tile;

    public override void OnNetworkSpawn()
    {
        Init();
    }

    private void Init()
    {
        stats_Legacy = DataManager.Instance.objectStats["Fire"];
        stats = DataManager.Instance.fireStats;
        DataManager.Instance.CashingFire(this);
        interactionMachine.fire = this;
    }

    public void Extinguish()
    {
        Destroy(this.gameObject);
    }
}
