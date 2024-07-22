using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using static UnityEngine.CullingGroup;
using System;
using System.Collections.Generic;

public class GameStateController : NetworkBehaviour
{
    public static GameStateController instance { get; private set; }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);


    [HideInInspector] public NetworkVariable<float> countdownToStartTimer = new(5f);
    [HideInInspector] public NetworkVariable<double> gamePlayerToStartTimer = new(0f);
    [HideInInspector] public NetworkVariable<int> remainCriminalCount = new(0);
    [HideInInspector] public NetworkVariable<float> needBurnRate = new(0f);
    [HideInInspector] public NetworkVariable<float> BurningTileCount = new(0f);

    private NetworkVariable<int> allTileCount = new(0);
    private Dictionary<ulong, bool> playerReadyDictionary;

    public event EventHandler OnStageChanged;


    public TextMeshProUGUI timeLimitText_Temp;
    public TextMeshProUGUI burnRateText_Temp;
    public TextMeshProUGUI remainCriminalText_Temp;
    public TextMeshProUGUI resultText_Temp;

    private bool isPlaying = false;
    private bool isLocalPlayerReady;


    private void Awake()
    {
        instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += ChangeStage;

        gamePlayerToStartTimer.OnValueChanged += SetTimerText;
        BurningTileCount.OnValueChanged += SetBurnRateText;
        remainCriminalCount.OnValueChanged += SetRemainCriminalText;

    }

    private void Start()
    {
        if (IsServer) SetInfo();
        InBattleGame();
    }

    public void Update()
    {
        if (!IsServer) return;

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0)
                {
                    state.Value = State.GamePlaying;
                }
                break;
            case State.GamePlaying:
                gamePlayerToStartTimer.Value += Time.deltaTime;

                //if (gamePlayerToStartTimer.Value < 0f)
                //{
                //    //isPlaying = false;
                //    TimeOver();
                //}
                break;
            case State.GameOver:
                break;
        }
    }

    public void SetInfo()
    {
        countdownToStartTimer.Value = 5f;
        gamePlayerToStartTimer.Value = 0; // changevalue
        remainCriminalCount.Value = 2;
        needBurnRate.Value = 60;
        allTileCount.Value = 236;

        isPlaying = true;
    }

    private void InBattleGame()
    {
        Debug.Log("InBattleGame Load!");

        if (state.Value == State.WaitingToStart)
        {
            Debug.Log("isLocalReady >> True");
            isLocalPlayerReady = true;
            SetPlayerReadyServerRpc();
        }
    }



    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allClientsReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }

    }

    private void ChangeStage(State preValue, State newValue)
    {
        OnStageChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetTimerText(double previousValue, double newValue)
    {
        // Todo : move UIcontroller
        timeLimitText_Temp.text = newValue.ToString("F2");
    }

    private void SetBurnRateText(float previousValue, float newValue)
    {
        // Todo : move UIcontroller
        var value = (BurningTileCount.Value / allTileCount.Value) * 100;
        burnRateText_Temp.text = value.ToString("F2");
    }


    private void TimeOver()
    {
        Debug.Log($"{GetType()} - TimeOver");

        // state.Value = State.GameOver;
        if (float.TryParse(burnRateText_Temp.text, out var rate))
        {
            if (rate >= needBurnRate.Value) CriminalVictoryClientRPC();
            else CriminalDefeatClientRPC();
        }
        else
        {
            Debug.Log($"{GetType()} - Burn Check Fail");
        }
    }

    public void CriminalCountDown()
    {
        remainCriminalCount.Value -= 1;
    }


    public void CriminalCountUp()
    {
        remainCriminalCount.Value += 1;
    }


    private void SetRemainCriminalText(int previousValue, int newValue)
    {
        remainCriminalText_Temp.text = newValue.ToString();

        if (newValue == 0)
        {
            CriminalDefeatClientRPC();
        }
    }

    [ClientRpc]
    public void CriminalVictoryClientRPC()
    {
        // if burnPercent Higher Then info
        // show ui
        resultText_Temp.text = "CriminalVictory!";
        Debug.Log($"{GetType()} - CriminalVictory!");
    }

    [ClientRpc]
    public void CriminalDefeatClientRPC()
    {
        // when all Crimial Caught
        // show ui
        resultText_Temp.text = "CriminalDefeat!";
        Debug.Log($"{GetType()} - CriminalDefeat!");
    }

    public bool IsCountDownState()
    {
        return state.Value == State.CountdownToStart;
    }

    public bool IsGamePlayingState()
    {
        return state.Value == State.GamePlaying;
    }

    public float GetcountDownValue()
    {
        return countdownToStartTimer.Value;
    }
}
