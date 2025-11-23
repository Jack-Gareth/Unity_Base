using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float deathBoundaryY = -10f;
    [SerializeField] private bool useTransitionOnDeath = true;
    [SerializeField] private float blackScreenDuration = 0.3f;
    
    private Vector3 spawnPosition;
    private Rigidbody2D playerRigidbody;
    private PlayerScaler playerScaler;
    private PlayerGravityFlip playerGravityFlip;
    private bool isRespawning = false;
    
    private void Awake()
    {
        spawnPosition = transform.position;
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerScaler = GetComponent<PlayerScaler>();
        playerGravityFlip = GetComponent<PlayerGravityFlip>();
    }
    
    private void Update()
    {
        CheckOutOfBounds();
    }
    
    private void CheckOutOfBounds()
    {
        if (transform.position.y < deathBoundaryY && !isRespawning)
        {
            isRespawning = true;
            StartCoroutine(RespawnWithTransition());
        }
    }

    private IEnumerator RespawnWithTransition()
    {
        if (useTransitionOnDeath && SwipeTransition.Instance != null)
        {
            bool respawnComplete = false;
            SwipeTransition.Instance.PlayTransitionEffect(() =>
            {
                RespawnPlayer();
                respawnComplete = true;
            }, blackScreenDuration);

            while (!respawnComplete)
            {
                yield return null;
            }
        }
        else
        {
            RespawnPlayer();
        }

        isRespawning = false;
    }
    
    private void RespawnPlayer()
    {
        transform.position = spawnPosition;
        
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (CameraRespawnSnap.Instance != null)
        {
            CameraRespawnSnap.Instance.SnapToTarget();
        }

        PlayerEvents.TriggerPlayerRespawn();

        if (LevelColorManager.Instance != null)
        {
            LevelColorManager.Instance.ResetToWhite();
        }

        if (playerScaler != null)
        {
            playerScaler.ForceResetSize();
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
