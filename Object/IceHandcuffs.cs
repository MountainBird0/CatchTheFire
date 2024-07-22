using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class IceHandcuffs : NetworkBehaviour
{
    public AllObjectStats.ObjectData iceHandcuffsStats = new AllObjectStats.ObjectData();

    public bool isStartfireTimer;

    public IceHandcuffsInteractionMachine interactionMachine;
    public IceHandcuffsCondition condition;

    public Job target;

    public void Awake()
    {
        DataManager.Instance.CashingIceHandCuffs(this);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        iceHandcuffsStats = DataManager.Instance.objectStats["IceHandcuffs"];
        interactionMachine = GetComponent<IceHandcuffsInteractionMachine>();
        condition = GetComponent<IceHandcuffsCondition>();

        interactionMachine.iceHandcuffs = this;

        if(IsServer)
        {
            // condition.hp.maxValue.Value = iceHandcuffsStats.Hp[0];
        }
    }

    public void Melt()
    {
        var criminal = target.GetComponent<Criminal>();
        //criminal.Escape();
        Destroy(this.gameObject);
    }
}
