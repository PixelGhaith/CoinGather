using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    [SerializeField] private int coinValue = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ulong playerId = other.GetComponent<NetworkObject>().OwnerClientId;
            ScoreboardManager.Instance.IncreasePlayerScoreRpc(playerId, 1);
            DestroyCoinRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void DestroyCoinRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
        Destroy(gameObject);
    }
}
