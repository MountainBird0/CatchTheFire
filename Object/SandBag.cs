using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;


public class SandBag : TileObject
{
    public AllObjectStats.ObjectData sandBagStats = new AllObjectStats.ObjectData();

    public int health = -1;
    public bool isfire;
    public bool isStartfireTimer;
    public bool isPushSand;

    // private Coroutine setFireTimer;
    private Coroutine setPushSand;
    Rigidbody rigidbody;

    private SandBagInteractionMachine interactionMachine;
    public SandBagCondition condition;

    public override void OnNetworkSpawn()
    {
        interactionMachine = GetComponent<SandBagInteractionMachine>();
        condition = GetComponent<SandBagCondition>();
    }

    public void Awake()
    {
        DataManager.Instance.CashingSandBad(this);
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        interactionMachine.sandBag = this;
        // rigidbody.isKinematic = false;

        if (IsServer)
        {
            sandBagStats = DataManager.Instance.objectStats["Sandbox"];
            // condition.hp.maxValue.Value = sandBagStats.Hp[0];
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator TryFireTimer()
    {
        yield return new WaitForSeconds(5f);
        //    if (isfire)
        //    {
        //        HealthSubtract();
        //    }
    }

    private IEnumerator TrySandPush(GameObject other)
    {
        Police police = other.gameObject.GetComponentInChildren<Police>();
        Transform sandspawn = this.transform;
        TileManager tilemanager = TileManager.Instance;
        Vector3 pos = police.modelTransform.forward;
        GameObject tile = tilemanager.GetTilesForwardDirection((int)sandspawn.position.x, (int)sandspawn.position.z, pos);
        Vector3 tilepos = tile.transform.position;
        tilepos.y = 2;
        Tile tileCheck = tile.GetComponent<Tile>();
        yield return new WaitForSeconds(2f);
        //if (!tileCheck.onTileObject || tileCheck.onTileObject is Fire)
        //{   
        //    tileCheck.onTileObject = this;
        //    tile = tilemanager.GetCurrentTilesPos((int)sandspawn.position.x, (int)sandspawn.position.z);
        //    tileCheck = tile.GetComponent<Tile>();
        //    tileCheck.onTileObject = null;
        //    TrySandPushServerRpc(tilepos);
        //}
    }
    [ServerRpc(RequireOwnership = false)]
    private void TrySandPushServerRpc(Vector3 pos)
    {
        transform.DOMove(pos, 1f);
    }

    public void SetIsInPushSandBag(bool isColliding, GameObject other)
    {
        isPushSand = isColliding;
        //if (isPushSand)
        //{
        //    PlayerController police = other.gameObject.GetComponent<PlayerController>();
        //    police.moveSpeed.Value = other.gameObject.GetComponentInChildren<Police>().policestats.SandbagMoveSpeed;
        //}
        //else
        //{
        //    PlayerController police = other.gameObject.GetComponent<PlayerController>();
        //    police.moveSpeed.Value = other.gameObject.GetComponentInChildren<Police>().policestats.MoveSpeed;
        //}
    }

    public void SetIsInSandBag(bool isColliding)
    {
        isfire = isColliding;
        isStartfireTimer = isColliding;
        //if (isfire)
        //{
        //    Debug.Log($"{GetType()} - in tile");
        //    setFireTimer = StartCoroutine(TryFireTimer());
        //}
        //else
        //{
        //    Debug.Log($"{GetType()} - escape tile");
        //    StopCoroutine(setFireTimer);
        //}
    }

    public void DestroySand()
    {
        Destroy(gameObject);
    }
}
