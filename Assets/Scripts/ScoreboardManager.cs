using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreboardManager : NetworkBehaviour
{
    public static ScoreboardManager Instance { get; private set; }
    private NetworkList<PlayerStats> networkPlayerList = new NetworkList<PlayerStats>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(gameObject);

        networkPlayerList.OnListChanged += OnPlayerListChanged;
    }

    private void OnPlayerListChanged(NetworkListEvent<PlayerStats> changeEvent)
    {
        List<PlayerStats> playerList = new List<PlayerStats>();

        foreach (var player in networkPlayerList)
            playerList.Add(player);
        ScoreboardUI.Instance.UpdateScoreboard(playerList);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandlePlayerConnected;
        }
    }

    private void HandlePlayerConnected(ulong playerId)
    {
        PlayerStats newPlayer = new PlayerStats
        {
            playerId = playerId,
            score = 0
        };

        networkPlayerList.Add(newPlayer);
    }

    [Rpc(SendTo.Server)]
    public void IncreasePlayerScoreRpc(ulong playerId, int scoreIncrease)
    {
        for (int i = 0; i < networkPlayerList.Count; i++)
        {
            if (networkPlayerList[i].playerId == playerId)
            {
                var player = networkPlayerList[i];
                player.score += scoreIncrease;
                networkPlayerList[i] = player;
                break;
            }
        }
    }

    public string GetWinnerName()
    {
        if (networkPlayerList.Count == 0)
            return "No Players";

        PlayerStats topPlayer = networkPlayerList[0];

        foreach (var player in networkPlayerList)
        {
            if (player.score > topPlayer.score)
            {
                topPlayer = player;
            }
        }

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(topPlayer.playerId, out var client) &&
        client.PlayerObject != null && client.PlayerObject.TryGetComponent(out PlayerName playerNameComponent))
        {
            return $"{playerNameComponent.GetPlayerName()} won!";
        }

        return "Unknown player won!";
    }
}