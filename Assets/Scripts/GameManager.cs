using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    private NetworkVariable<float> gameTime = new NetworkVariable<float>(120f);
    private bool gameActive = false;
    [SerializeField] TextMeshProUGUI gameInfoText;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;
    [SerializeField] private float spawnInterval = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameInfoText.text = "Press Enter To Start!";
        gameTime.OnValueChanged += UpdateGameInfoText;
    }

    void Update()
    {
        if (IsHost && Input.GetKeyDown(KeyCode.Return) && !gameActive)
        {
            StartGame();
        }

        if (!IsServer || !gameActive) return;

        gameTime.Value -= Time.deltaTime;
        if (gameTime.Value <= 0)
        {
            EndGame();
        }
    }

    void StartGame()
    {
        if (!IsServer) return;
        Debug.Log("Game Started");
        gameTime.Value = 120f;
        SetGameActiveRpc(true);
        InvokeRepeating(nameof(SpawnCoin), 0f, spawnInterval);
    }

    [Rpc(SendTo.ClientsAndHost)]
    void SetGameActiveRpc(bool active)
    {
        gameActive = active;
        gameInfoText.text = active ? $"{gameTime.Value:F1}" : "Game Over!";
    }

    private void SpawnCoin()
    {
        if (!gameActive) return;

        Vector2 randomPosition = new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );

        GameObject coinInstance = Instantiate(coinPrefab, randomPosition, Quaternion.identity);

        var networkObject = coinInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }

    private void EndGame()
    {
        gameTime.Value = 0;
        SetGameActiveRpc(false);
        CancelInvoke(nameof(SpawnCoin));
        Coin[] coins = FindObjectsByType<Coin>(FindObjectsSortMode.None);
        foreach (Coin coin in coins)
        {
            coin.DestroyCoinRpc();
        }
        UpdateWinnerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void UpdateWinnerRpc()
    {
        string winnerText = ScoreboardManager.Instance.GetWinnerName();
        gameInfoText.text = winnerText;
    }

    void UpdateGameInfoText(float previousTime, float newTime)
    {
        if (gameActive)
        {
            gameInfoText.text = $"{newTime:F1}";
        }
    }
}
