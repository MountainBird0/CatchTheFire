using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
public enum FireFighterState
{
    NORMAL = 0,
    IN_FIRE = 1,
    IN_CAPSULE = 2,
    IS_SHOOT = 4,
    IS_DIE = 8,
    IS_STUNNED = 16,
    IS_INVINCIBLE = 32,
}

public class FireFighter : Job
{
    public AllObjectStats.FireFighterData fireFighterstats = new AllObjectStats.FireFighterData();
    public PlayerController controller;
    public FireFighterInteractionMachine interactionMachice;
    [SerializeField] private SprayParticle particle;

    public NetworkVariable<FireFighterState> state = new(0);

    public int currentCapsule { get; set; } = 0;

    public NetworkVariable<float> sprayTimer = new NetworkVariable<float>(5.0f);
    public NetworkVariable<float> cooldownTimer = new NetworkVariable<float>(2.0f);

    public NetworkVariable<bool> isSpraying = new(false); 
    public NetworkVariable<bool> isCharging = new(false);
    public NetworkVariable<double> lastSprayTime = new(0.0);
    public NetworkVariable<double> sprayEndTime = new(0.0);

    [SerializeField] private GameObject collider;
    public Transform models;

    //private float prePos;

    public NetworkVariable<float> prePos = new(0.0f);

    public WaterLasers waterLasers;



    private void Awake()
    {
        Init();
    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += ChangeState;

        if (IsServer)
        {
            controller.moveSpeed.Value = fireFighterstats.MoveSpeed;
            //sprayTimer.Value = fireFighterstats.ExtinguisherTime; // Cheat Fix
            sprayTimer.Value = 5f;
            isSpraying.Value = false;        
        }

        if (IsOwner)
        {
           

            ItemSlot.Instance.button.onClick.AddListener(() => UseItem());
        }
    }
    private void Init()
    {
        fireFighterstats = DataManager.Instance.fireFighterStats;
        jobindex = fireFighterstats.JobId;
        DataManager.Instance.CashingData(this);

        controller = this.gameObject.GetComponentInParent<PlayerController>();

        stats.faction = 1;

        interactionMachice = GetComponent<FireFighterInteractionMachine>();
        interactionMachice.fireFight = this;

        DeactiveCollider();
    }

    public int totalCount = 0;
    public string itemName;
    public void UseItem()
    {
        Debug.Log($"아이템 사용 이름은{itemName}");
        if (!IsOwner) return;

        if (itemName == "Bomb(Clone)")
        {
            Debug.Log("실제 아이템 사용");

            //var genericMethod = method.MakeGenericMethod(typeof(int));

            
            totalCount--;
        }
        else if (itemName == "Invincivility(Clone)")
        {
            Debug.Log("실제 아이템 사용");

            SetInvincibleServerRpc();
            totalCount--;
        }

        ItemSlot.Instance.button.gameObject.SetActive(false);
    }

    [ServerRpc]
    private void SetInvincibleServerRpc()
    {
        state.Value |= FireFighterState.IS_INVINCIBLE;
    }

    private void Update()
    {
        //if (IsOwner)
        //{
        //    InputKey();
        //}

        //if (IsServer)
        //{
        //    HandleSparying();
        //}
    }

    private void InputKey()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartSprayingServerRpc();
        }

        if (Input.GetMouseButton(0) && isSpraying.Value)
        {
            UpdateSprayingServerRpc();
        }

        if (Input.GetMouseButtonUp(0))
        {
             StopSprayingServerRpc();
        }
    }

    private void HandleSparying()
    {
        if (isSpraying.Value && NetworkManager.ServerTime.Time - sprayEndTime.Value <= sprayTimer.Value) 
        {
            ShootExting();
        }
        else if (!isSpraying.Value && NetworkManager.ServerTime.Time - sprayEndTime.Value < cooldownTimer.Value)
        {
            CancleShoot();
        } 
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartSprayingServerRpc()
    {
        if (NetworkManager.ServerTime.Time - sprayEndTime.Value < cooldownTimer.Value) return;

        isSpraying.Value = true;
        lastSprayTime.Value = NetworkManager.ServerTime.Time;
        sprayEndTime.Value = lastSprayTime.Value + sprayTimer.Value;

        prePos.Value = models.transform.localRotation.y;


        WaterSpayClientRpc();
        //particle.PlayParticleClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateSprayingServerRpc()
    {
        if(NetworkManager.ServerTime.Time - sprayEndTime.Value > sprayTimer.Value)
        {
            sprayEndTime.Value = NetworkManager.ServerTime.Time;
            isSpraying.Value = false;
            WaterStopSpayClientRpc();
            DeactiveCollider();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopSprayingServerRpc()
    {
        if (!isSpraying.Value) return;

        isSpraying.Value = false;
        sprayEndTime.Value = NetworkManager.ServerTime.Time;
        sprayTimer.Value = 5.0f;
        waterLasers.maxLength.Value = 1;

        //particle.StopParticleClientRpc();
        WaterStopSpayClientRpc();
    }

    [ClientRpc]
    private void WaterSpayClientRpc()
    {
        waterLasers.startDissovle = false;
        waterLasers.UpdateSaver = false;
        waterLasers.FireLaser();
    }
    [ClientRpc]
    private void WaterSpayUpdateClientRpc()
    {
        waterLasers.FireLaser();
    }

    [ClientRpc]
    private void WaterStopSpayClientRpc()
    {
        waterLasers.DisablePrepare();
    }



    [ServerRpc(RequireOwnership = false)]
    private void ChargeSprayGaugeServerRpc()                                              
    {
        isCharging.Value = true;
    }


    private void ShootExting()
    {
        if ((state.Value & FireFighterState.IS_DIE) != 0) return;

        ActivateCollider();
        Debug.Log("ShootExing!");
       
        //if (sprayTimer.Value <= 3)
        //{
        //    //extinguisher.Value = sprayTimer.Value; //* 2

        //    extinguisher.Value += fireFighterstats.Extinguisher;

        //    condition.UseExtinguising(-extinguisher.Value);
        //    waterLasers.maxLength.Value = sprayTimer.Value;


        //}

        //if (sprayTimer.Value < 3)
        //{
        //    sprayTimer.Value++;
        //}
    }

    private void CancleShoot()
    {
        Debug.Log("CancelShoot!");
        DeactiveCollider();
    }

    private void ActivateCollider() //float index
    {
        collider.SetActive(true);
    }

    private void DeactiveCollider()
    {
        collider.SetActive(false);
    }

    private void LoadCapsule()
    {
        Debug.Log("Capsule 사용!");

        UseCapsuleClientRpc();
        isCharging.Value = false;
    }

    [ClientRpc]
    private void UseCapsuleClientRpc()
    {
        currentCapsule = 0;
    }


    public void EscapeEvent(GameObject other)
    {
        Escape();
    }

    public void Escape()
    {
        state.Value &= ~FireFighterState.IS_DIE;
        state.Value |= FireFighterState.NORMAL;

        controller.moveSpeed.Value = 1.5f;
    }

    private IEnumerator Stun()
    {
        controller.SetState(new StunState(controller));

        yield return new WaitForSeconds(1.0f);

        controller.SetState(new IdleState(controller));
        if (IsServer)
        {
            state.Value &= ~FireFighterState.IS_STUNNED;
        }
    }

    private IEnumerator Invincible()
    {
        state.Value |= FireFighterState.IS_INVINCIBLE;
        SetLayerClientRpc(0);

        yield return new WaitForSeconds(6.0f);

        state.Value &= ~FireFighterState.IS_INVINCIBLE;
        SetLayerClientRpc(10);
    }

    private void ChangeState(FireFighterState pre, FireFighterState cur)
    {
        if ((pre & FireFighterState.IS_STUNNED) == 0
            && (cur & FireFighterState.IS_STUNNED) != 0)
        {
            StartCoroutine(Stun());
            if (IsServer)
            {
                StartCoroutine(Invincible());
            }
        }

        if ((pre & FireFighterState.IS_INVINCIBLE) == 0
    && (cur & FireFighterState.IS_INVINCIBLE) != 0)
        {
            if (IsServer)
            {
                StartCoroutine(Invincible());
            }
        }
    }

    [ClientRpc]
    public void SetLayerClientRpc(int num)
    {
        controller.gameObject.layer = num;
    }


}
