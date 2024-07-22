using DG.Tweening;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using Unity.Netcode;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.PackageManager;
using UnityEngine;


public class Criminal : Job, IStunDamageable
{
    public AllObjectStats.CriminalData criminalstats = new AllObjectStats.CriminalData();

    public NetworkVariable<ECriminalState> state = new(0);
    public PlayerController controller;
    public CriminalCondition condition;
    private CriminalInteractionMachine interactionMachine;

    public override void OnNetworkSpawn()
    {
        Init();
        if (IsServer)
        {
            controller.moveSpeed.Value = criminalstats.MoveSpeed;
            condition.hp.maxValue.Value = 5f;
            condition.hp.curValue.Value = 0f;
        }
    }
   
    private void Init()
    {
        controller = GetComponentInParent<PlayerController>(); // Todo : Fix. using GetComponentInParent is ok?

        interactionMachine = GetComponent<CriminalInteractionMachine>();
        interactionMachine.criminal = this;

        criminalstats = DataManager.Instance.criminalStats;
        jobindex = criminalstats.JobId;
        stats.faction = 0;
        DataManager.Instance.CashingData(this);

        state.OnValueChanged += ChangeState;

        if(IsOwner)
        {
            ItemSlot.Instance.button.onClick.AddListener(() => UseItem());
        }
    }
    public int totalCount = 0;
    public string itemName;
    public void UseItem()
    {
        if (!IsOwner) return;

        if (itemName == "Bomb(Clone)")
        {
            totalCount--;
        }
        else if (itemName == "Invincivility(Clone)")
        {
            SetInvincibleServerRpc();
            totalCount--;
        }

        ItemSlot.Instance.button.gameObject.SetActive(false);
    }

    [ServerRpc]
    private void SetInvincibleServerRpc()
    {
        state.Value |= ECriminalState.IS_INVINCIBLE;
    }

    private IEnumerator Stun()
    {
        controller.SetState(new StunState(controller));

        yield return new WaitForSeconds(1.0f);

        controller.SetState(new IdleState(controller));
        if(IsServer)
        {
            state.Value &= ~ECriminalState.IS_STUNNED;
        }
    }

    public IEnumerator Invincible()
    { 
        state.Value |= ECriminalState.IS_INVINCIBLE;

        yield return new WaitForSeconds(6.0f);

        state.Value &= ~ECriminalState.IS_INVINCIBLE;
    }

    private void ChangeState(ECriminalState pre, ECriminalState cur)
    {
        if((pre & ECriminalState.IS_STUNNED) == 0
            && (cur & ECriminalState.IS_STUNNED) != 0)
        {
            StartCoroutine(Stun());
            if(IsServer)
            {
                StartCoroutine(Invincible());
            }
        }

        if ((pre & ECriminalState.IS_INVINCIBLE) == 0
            && (cur & ECriminalState.IS_INVINCIBLE) != 0)
        {
            if (IsServer)
            {
                StartCoroutine(Invincible());
            }
        }
    }

    public void TakeHit(float damage)
    {
        condition.hp.curValue.Value += damage;
        if(condition.hp.curValue.Value == 5)
        {
            state.Value |= ECriminalState.IS_STUNNED;
        }
    }


}
