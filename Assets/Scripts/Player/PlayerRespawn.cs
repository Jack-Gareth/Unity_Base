using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float deathBoundaryY = -10f;
    
    private Vector3 spawnPosition;
    private Rigidbody2D playerRigidbody;
    private PlayerScaler playerScaler;
    
    private void Awake()
    {
        spawnPosition = transform.position;
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerScaler = GetComponent<PlayerScaler>();
    }
    
    private void Update()
    {
        CheckOutOfBounds();
    }
    
    private void CheckOutOfBounds()
    {
        if (transform.position.y < deathBoundaryY)
        {
            RespawnPlayer();
        }
    }
    
    private void RespawnPlayer()
    {
        transform.position = spawnPosition;
        
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (LevelColorManager.Instance != null)
        {
            LevelColorManager.Instance.ResetToWhite();
        }

        if (playerScaler != null)
        {
            playerScaler.ResetSize();
        }

        if (DeathCounter.Instance != null)
        {
            DeathCounter.Instance.IncrementDeathCount();
        }
    }
    
    public void SetSpawnPosition(Vector3 newSpawnPosition)
    {
        spawnPosition = newSpawnPosition;
    }
}
