using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;


public enum PoliceState
{
    NORAML = 0,
    IS_CANCATCH = 1,
}

public class Police : Job
{
    #region boolState
    public NetworkVariable<bool> isMakeSand = new(false);
    private NetworkVariable<bool> isExtinguishingCapsule = new(false);
    public NetworkVariable<bool> isIceHandcuffs = new(false);
    public bool isMakeSandClient = true;
    public bool isExtinguishingCapsuleClient = true;
    #endregion

    public AllObjectStats.PoliceData policeStats = new AllObjectStats.PoliceData();
    public NetworkVariable<PoliceState> state = new(0);

    public NetworkVariable<int> iExtinguishingCapsuleCount = new NetworkVariable<int>(0);
    public Job target;
    public Coroutine tryMakeIceHandcuffs;

    public Transform modelTransform;

    public PlayerController controller;
    private PoliceInteractionMachine interactionMachine;

    public Skill makeCapSule;
    public Skill makeSand;

    public int iPoliceCount;

    private void Awake()
    {
        Init();
        isMakeSand = new(false);
        isExtinguishingCapsule = new(false);
        isIceHandcuffs = new(false);
        isMakeSandClient = true;
        isExtinguishingCapsuleClient = true;
    }

    //temp
    private void Update()
    {
        if (IsOwner)
            InputKey();
        if (IsServer)
            Skill();
    }
    private void Skill()
    {
        if (isMakeSand.Value)
            OnSandMake();
        if (isExtinguishingCapsule.Value)
            OnExtinguishingCapsule();
    }
    [ServerRpc(RequireOwnership = false)]
    public void IsMakeSandServerRpc()
    {
        if (isMakeSandClient == false)
            return;
        if (isMakeSand.Value == false)
            isMakeSand.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void IsMakeExtinguishingCapsuleServerRpc()
    {
        if (isExtinguishingCapsuleClient == false)
            return;
        if (isExtinguishingCapsule.Value == false)
            isExtinguishingCapsule.Value = true;
    }
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            controller.moveSpeed.Value = policeStats.MoveSpeed;
        }
    }
    private void Init()
    {
        controller = this.gameObject.GetComponentInParent<PlayerController>();
        interactionMachine = GetComponent<PoliceInteractionMachine>();
        interactionMachine.police = this;
        iPoliceCount = 1;
        policeStats = DataManager.Instance.policeStats;
        jobindex = policeStats.JobId;
        DataManager.Instance.CashingData(this);
    }
    public void OnExtinguishingCapsule()
    {
        TileManager tileanager = TileManager.Instance;
        GameObject tileOb = tileanager.GetTilesForwardDirection((int)modelTransform.position.x, (int)modelTransform.position.z, modelTransform.forward * -1);

        if (isExtinguishingCapsuleClient)
        {
            //if (iExtinguishingCapsuleCount == iPoliceCount) return;

            var tile = tileOb.GetComponent<Tile>();

            if (tile.onTileObject != null) return;

            var pos = tile.transform.position;
            pos.y = 1;

            var netOb = NetworkObjectPoolLegacy.Singleton.Spawn("FireExtinguishingFluid", pos, Quaternion.identity);
            netOb.Spawn();
            ExtinguishingCapsule obj = netOb.GetComponent<ExtinguishingCapsule>();
            obj.SetPolice(this);

            StartCoroutine("TryExtinguishingCapsuleMaking");
            iExtinguishingCapsuleCount.Value++;
            isExtinguishingCapsuleClient = false;
            isExtinguishingCapsule.Value = false;
        }
    }

    public void OnSandMake()
    {
        TileManager tileanager = TileManager.Instance;
        Transform sandspawn = modelTransform.transform;
        GameObject tileOb = tileanager.GetTilesForwardDirection((int)sandspawn.position.x, (int)sandspawn.position.z, sandspawn.forward);
        var tile = tileOb.GetComponent<Tile>();

        if (isMakeSandClient)
        {
            if (tile.onTileObject != null) return;

            var pos = tile.transform.position;
            pos.y = 1.5f;
            var netOb = NetworkObjectPoolLegacy.Singleton.Spawn("Sand", pos, Quaternion.identity);
            tile.onTileObject = netOb.GetComponent<SandBag>();
            netOb.Spawn();

            StartCoroutine("TrySandMaking");

            isMakeSandClient = false;
            isMakeSand.Value = false;
        }

    }

    private IEnumerator TryIceHandcuffsMaking()
    {
        yield return new WaitForSeconds(1f);
        OnIceHandcuffsServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnIceHandcuffsServerRpc()
    {
        Debug.Log($"{GetType()} - make handcuffs");
        if ((state.Value & PoliceState.IS_CANCATCH) != 0 && (target != null))
        {
            var netOb = NetworkObjectPoolLegacy.Singleton.Spawn("IceHandcuffs", target.transform.position, Quaternion.identity);
            netOb.Spawn();

            var iceHandcuffs = netOb.GetComponent<IceHandcuffs>();
            iceHandcuffs.target = target;
            // SpawnManager.instance.SpawnObject(IceHandcuffsprefab, target.transform);
        }
    }

    private IEnumerator TrySandMaking()
    {
        yield return new WaitForSeconds(10f);
        isMakeSandClient = true;
        if (isMakeSandClient)
        {
            Debug.Log($"{GetType()} - make EMBER");
        }
    }

    private IEnumerator TryExtinguishingCapsuleMaking()
    {
        yield return new WaitForSeconds(10f);
        isExtinguishingCapsuleClient = true;
        if (isExtinguishingCapsuleClient)
        {
            Debug.Log($"{GetType()} - make EMBER");
        }
    }
    private void InputKey()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            // makeSand.ApplySkill();

            IsMakeSandServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            IsMakeExtinguishingCapsuleServerRpc();
            Debug.Log(policeStats.Index);
            Debug.Log(policeStats.JobId);
            Debug.Log(policeStats.MoveSpeed);
            Debug.Log(policeStats.SandbagMoveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("키 누름");
                if ((state.Value & PoliceState.IS_CANCATCH) != 0 && (target != null))
                {
                    Debug.Log("만들기 시작");
                    tryMakeIceHandcuffs = StartCoroutine(TryIceHandcuffsMaking());
                }
            }
        }

    }
}
