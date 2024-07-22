using NUnit.Framework.Interfaces;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player LocalInstance { get; private set; }

    public PlayerController controller;


    private void Awake()
    {
        if (IsOwner)
            LocalInstance = this;

            //CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("spawn");
        InitializePlayer();
        //StartCoroutine(WaitForBattleMapManagerInitialization());
    }


    private void Start()
    {
        if (IsOwner)
        {   
            Debug.Log("Player object spawned for owner client.");
        }
        else
        {
            Debug.Log("Player object spawned for non-owner client.");
        }

        //PlayerData playerData = GameMultiPlayer.Instance.GetPlayerDataFromClientId(OwnerClientId);

    }

    private IEnumerator WaitForBattleMapManagerInitialization()
    {
        while(BattleMapManager.Instance == null)
        {
            yield return null;
        }

        while (!BattleMapManager.Instance.isInitialized)
        {
            yield return null;
        }

        InitializePlayer();
    }

    private void InitializePlayer()
    {
        Debug.Log("Player Initialized");
        
        //GameManager.Instance.SetBattle(this);
        controller.joystick = BattleMapManager.Instance.joyStick;

        ulong id = NetworkManager.Singleton.LocalClientId;

        controller.Init();

        //Future Fix
        // transform.position = new Vector3(11 + (id * 4), 2 , 4 + (id * 4));


        int jobId = GameMultiPlayer.Instance.GetPlayerDataFromClientId(id).jobId;

        if(IsOwner)
        {
            if(jobId == 0)
            {
                transform.position = new Vector3(6, 1, 4);
            }
            else if(jobId == 1)
            {
                transform.position = new Vector3(11, 1, 11);
            }
            else
            {
                transform.position = new Vector3(22, 1, 34);
            }
        }

        // testClientRpc(id);
    }

    [ClientRpc]
    private void testClientRpc(ulong id)
    {
        transform.position = new Vector3(11 + (id * 4), 1, 4 + (id * 4));
    }
}
