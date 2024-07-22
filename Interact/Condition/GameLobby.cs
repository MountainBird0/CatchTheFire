using System;
using System.Collections.Generic;

using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Netcode;
using Random = UnityEngine.Random;
using Unity.Netcode.Transports.UTP;


#if DEDICATED_SERVER
using Unity.Services.Multiplay;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Matchmaker;
# endif

public class GameLobby : NetworkBehaviour
{
    public static GameLobby Instance { get; private set; }

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float listLobbiesTimer;

#if DEDICATED_SERVER
    private float autoAllocateTimer = 9999999f;
    private bool alreadyAutoAllocated;
    private static IServerQueryHandler serverQueryHandler; // static so it doesn't get destroyed when this object is destroyed
    private string backfillTicketId;
    private float acceptBackfillTicketsTimer;
    private float acceptBackfillTicketsTimerMax = 1.1f;
    private PayloadAllocation payloadAllocation;
#endif

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
        InitializeUnityAuthentication();
    }

    private void Start()
    {
        GameMultiPlayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.OnInstanceCreated += CharacterSelectReady_OnInstanceCreated;
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();

#if DEDICATED_SERVER
        autoAllocateTimer -= Time.deltaTime;
        if (autoAllocateTimer <= 0f)
        {
            autoAllocateTimer = 999f;
            MultiplayEventCallbacks_Allocate(null);
        }

        if (serverQueryHandler != null)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
            }
            serverQueryHandler.UpdateServerCheck();
        }

        if (backfillTicketId != null)
        {
            acceptBackfillTicketsTimer -= Time.deltaTime;
            if (acceptBackfillTicketsTimer <= 0f)
            {
                acceptBackfillTicketsTimer = acceptBackfillTicketsTimerMax;
                HandleBackfillTickets();
            }
        }
#endif
    }

    private async void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
#if DEDICATED_SERVER
        HandleUpdateBackfillTickets();

        if (GameMultiPlayer.Instance.HasAvailablePlayerSlots())
        {
            await MultiplayService.Instance.ReadyServerForPlayersAsync();
        }
        else
        {
            await MultiplayService.Instance.UnreadyServerAsync();
        }
#endif
    }

    private void CharacterSelectReady_OnInstanceCreated(object sender, EventArgs e)
    {
        CharacterSelectReady.Instance.OnGameStarting += CharacterSelectReady_OnGameStarting;
    }

    private async void CharacterSelectReady_OnGameStarting(object sender, EventArgs e)
    {
#if DEDICATED_SERVER
        if (backfillTicketId != null)
        {
            await MatchmakerService.Instance.DeleteBackfillTicketAsync(backfillTicketId);
        }
#endif
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();

            // only test -> Use to run multiple builds on the same pc -> need check and delete
#if !DEDICATED_SERVER
            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());
#endif

            await UnityServices.InitializeAsync(initializationOptions);

#if !DEDICATED_SERVER
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
#endif

#if DEDICATED_SERVER
            UtilManager.InActiveCamera();

            Debug.Log("DEDICATED_SERVER LOBBY");
            // OnDedicatedServer("Dedicated Server", false);            


            MultiplayEventCallbacks multiplayEventCallbacks = new();
            multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
            multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
            multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
            multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
            IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

            serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(4, "C1", "Direct file upload", "86595", "Default");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "")
            {
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
            }
#endif
        }
        else
        {
            // Already Initialized

#if DEDICATED_SERVER
            Debug.Log("DEDICATED_SERVER LOBBY - ALREADY INIT");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            if (serverConfig.AllocationId != "") {
                // Already Allocated
                MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
            }
#endif
        }
    }

#if DEDICATED_SERVER
    private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
        Debug.Log(obj);
    }

    private void MultiplayEventCallbacks_Error(MultiplayError obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
        Debug.Log(obj.Reason);
    }

    private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
    }

    private void MultiplayEventCallbacks_Allocate(MultiplayAllocation obj)
    {
        Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Allocate");

        if (alreadyAutoAllocated)
        {
            Debug.Log("Already auto allocated!");
            return;
        }

        SetupBackfillTickets();

        alreadyAutoAllocated = true;

        var serverConfig = MultiplayService.Instance.ServerConfig;
        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        Debug.Log($"Port[{serverConfig.Port}]");
        Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

        string ipv4Address = "0.0.0.0";
        ushort port = serverConfig.Port;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");

        GameMultiPlayer.Instance.StartServer();
        // Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        // Loader.LoadNetwork("ReadyRoomScene");
    }

    private async void SetupBackfillTickets()
    {
        Debug.Log("SetupBackfillTickets");
        payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<PayloadAllocation>();

        backfillTicketId = payloadAllocation.BackfillTicketId;
        Debug.Log("backfillTicketId: " + backfillTicketId);

        acceptBackfillTicketsTimer = acceptBackfillTicketsTimerMax;
    }

    private async void HandleUpdateBackfillTickets()
    {
        if (backfillTicketId != null && payloadAllocation != null && GameMultiPlayer.Instance.HasAvailablePlayerSlots())
        {
            Debug.Log("HandleUpdateBackfillTickets");

            List<Unity.Services.Matchmaker.Models.Player> playerList = new List<Unity.Services.Matchmaker.Models.Player>();

            foreach (PlayerData playerData in GameMultiPlayer.Instance.GetPlayerDataNetworkList())
            {
                playerList.Add(new Unity.Services.Matchmaker.Models.Player(playerData.clientId.ToString()));
            }

            MatchProperties matchProperties = new MatchProperties(
                payloadAllocation.MatchProperties.Teams,
                playerList,
                payloadAllocation.MatchProperties.Region,
                payloadAllocation.MatchProperties.BackfillTicketId
            );

            try
            {
                await MatchmakerService.Instance.UpdateBackfillTicketAsync(payloadAllocation.BackfillTicketId,
                    new BackfillTicket(backfillTicketId, properties: new BackfillTicketProperties(matchProperties))
                );
            }
            catch (MatchmakerServiceException e)
            {
                Debug.Log("Error: " + e);
            }
        }
    }

    private async void HandleBackfillTickets()
    {
        if (GameMultiPlayer.Instance.HasAvailablePlayerSlots())
        {
            BackfillTicket backfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(backfillTicketId);
            backfillTicketId = backfillTicket.Id;
        }
    }

    [Serializable]
    public class PayloadAllocation
    {
        public Unity.Services.Matchmaker.Models.MatchProperties MatchProperties;
        public string GeneratorName;
        public string QueueName;
        public string PoolName;
        public string EnvironmentId;
        public string BackfillTicketId;
        public string MatchId;
        public string PoolId;
    }
#endif

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter> {
                  new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
             }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void OnDedicatedServer(string lobbyName, bool isPrivate)
    {
        try{
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, new CreateLobbyOptions { IsPrivate = isPrivate, });
            Debug.Log($"GameLobby >> OnDedicatedServer : {joinedLobby.Id}");
            GameMultiPlayer.Instance.StartServer();
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try{
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, new CreateLobbyOptions { IsPrivate = isPrivate, });

            Debug.Log($"GameLobby >> CtreateLobby LobbyCode : {joinedLobby.Id}");

            GameMultiPlayer.Instance.StartHost();
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log($"GameLobby >> QuickJjoin LobbyCode : {joinedLobby.Id}");
            GameMultiPlayer.Instance.StartClient();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void OnLocalPlay()
    {
        GameMultiPlayer.Instance.StartHost();
    }

    public async void JoinWithId(string lobbyId)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            GameMultiPlayer.Instance.StartClient();
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                joinedLobby = null;
            }catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
           
            
    }
    public async void LeaveLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
        
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    private void HandleHeartbeat()
    {
        if(IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime;

            if(heartbeatTimer <= 0f)
            {
                float heartbeatTimeMax = 15f;
                heartbeatTimer = heartbeatTimeMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private void HandlePeriodicListLobbies()
    {
        if (joinedLobby == null &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn
            )
        {

            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }

}
