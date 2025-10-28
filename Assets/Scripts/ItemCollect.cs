using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemCollect : MonoBehaviour
{
    [Header("Collection Settings")]
    [SerializeField] private bool usePlayerTag = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool resetOnPlayerDeath = true;

    [Header("Effects")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;

    private SpriteRenderer spriteRenderer;
    private Collider2D itemCollider;
    private bool isCollected = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GetComponent<Collider2D>();
        
        if (itemCollider != null)
        {
            itemCollider.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        if (resetOnPlayerDeath)
        {
            PlayerEvents.OnPlayerRespawn += HandlePlayerRespawn;
        }
    }

    private void OnDisable()
    {
        if (resetOnPlayerDeath)
        {
            PlayerEvents.OnPlayerRespawn -= HandlePlayerRespawn;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
            return;

        bool isPlayer = false;

        if (usePlayerTag)
        {
            isPlayer = other.CompareTag(playerTag);
        }
        else
        {
            isPlayer = ((1 << other.gameObject.layer) & playerLayer) != 0;
        }

        if (isPlayer)
        {
            OnCollect();
        }
    }

    private void OnCollect()
    {
        isCollected = true;

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (ItemCollectionManager.Instance != null)
        {
            ItemCollectionManager.Instance.CollectItem();
        }

        HideItem();
    }

    private void HideItem()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }
    }

    private void ShowItem()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        isCollected = false;
    }

    private void HandlePlayerRespawn()
    {
        ShowItem();
    }
}
