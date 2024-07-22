using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleMapManager : NetworkBehaviour
{
    public static BattleMapManager Instance { get; private set; }

    public CinemachineCamera cinemachineCamera;
    public VariableJoystick joyStick;

    public List<List<GameObject>> tiles;

    public float spawnDelay = 60f;

    //public NetworkVariable<Dictionary<EJob, ulong>> teamDataDic = new();

    //private MapMaker mapMaker;
    [HideInInspector] public CharacterSpawner characterSpawner;
    [HideInInspector] public GameStateController gameStateController;


    //test
    //public Transform playerPrefab;

    public bool isInitialized { get; private set; } = false;

    private void Awake()
    {
        Instance = this;


        //mapMaker = GetComponent<MapMaker>();
        characterSpawner = GetComponent<CharacterSpawner>();
        gameStateController = GetComponent<GameStateController>();

    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            Debug.Log("BattleMapManager >> OnNetworkSpawn >> Server initialized");
            //DivideTeam();
        }
    }

    private void Start()
    {
        // StartCoroutine(CoItemDrop(20f, "Bomb"));
        // StartCoroutine(CoItemDrop(30f, "Invincivility"));
        
    }

    public void Init()
    {

        //mapMaker.MapSetting(mapInfo);
        //MapInfo mapInfo = GameManager.Instance.SetSelectedMap();
        //gameStateController.SetInfoServerRPC(mapInfo);
        //isInitialized = true;
    }

    private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("OnLoadEventCompleted called");
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"Spawning player for client ID: {clientId}");

            var playerData = GameMultiPlayer.Instance.GetPlayerDataFromClientId  (clientId);

            var jobOb = characterSpawner.SelectJob(playerData.jobId);
            if (jobOb == null)
            {
                Debug.LogError("Failed to instantiate playerPrefab.");
                continue;
            }

            Debug.Log($"Instantiated playerPrefab: {jobOb}");

            var networkObject = jobOb.GetComponent<NetworkObject>();


            networkObject.SpawnAsPlayerObject(clientId, true);
            Debug.Log($"Spawned player object for client ID: {clientId}");

            Debug.Log($"NetworkObject NetworkId: {networkObject.NetworkObjectId}");
            Debug.Log($"NetworkObject IsSpawned: {networkObject.IsSpawned}");
            Debug.Log($"NetworkObject OwnerClientId: {networkObject.OwnerClientId}");

        }
    }

    //private void DivideTeam()
    //{
    //    var playerDatas = GameMultiPlayer.Instance.GetPlayerDataNetworkList();

    //    foreach (var playerData in playerDatas)
    //    {
    //        if (playerData.jobId == 0)
    //            teamDataDic.Value.Add(EJob.CIRIMINAL, playerData.clientId);
    //        else if (playerData.jobId == 1)
    //            teamDataDic.Value.Add(EJob.FIREFIGHTER, playerData.clientId);

    //        Debug.Log($" playerDatas {playerDatas}");
    //    }
    //}


    /// <summary>
    /// Drop the Item
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoItemDrop(float time, string itemName)
    {
        while(true)
        {
            yield return new WaitForSeconds(time);

            Vector2Int randomPos = GetRandomTilePosition();

            while(!IsTile(randomPos))
            {
                randomPos = GetRandomTilePosition();
            }

            GameObject tile = tiles[randomPos.x][randomPos.y];

            Vector3 spawnPos = tile.transform.position;
            spawnPos.y = 1.5f;

            var netOb = NetworkObjectPoolLegacy.Singleton.Spawn(itemName, spawnPos, Quaternion.identity);
            netOb.Spawn();
        }
    }
    private Vector2Int GetRandomTilePosition()
    {
        int x = Random.Range(0, tiles.Count); 
        int y = Random.Range(0, tiles[0].Count);

        return new Vector2Int(x, y);
    }

    private bool IsTile(Vector2Int pos)
    {
        return tiles[pos.x][pos.y] != null;
    }

    public void SpawnDecreasedVision()
    {
        Debug.Log("BattleMapManager >> DecreasedVision Spawn");
        Vector3 spawnPos = GetTileCenter();
        var netOb = NetworkObjectPoolLegacy.Singleton.Spawn("DecreasedVision", spawnPos, Quaternion.identity);
        netOb.Spawn();
    }

    public void OnDestoryDecreasedVision()
    {
        StartCoroutine(CoDecreasedSpawn());
    }

    private IEnumerator CoDecreasedSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnDecreasedVision();
    }

    private Vector3 GetTileCenter()
    {
        int rowCount = tiles.Count;
        int colCount = tiles[0].Count;

        float centerX = (colCount - 1) / 2f;
        float centerZ = (rowCount - 1) / 2f;

        Vector3 centorPos = new Vector3(centerX, 1, centerZ);

        return centorPos;
    }

}
