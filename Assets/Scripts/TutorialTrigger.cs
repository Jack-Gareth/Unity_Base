using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")]
    [SerializeField] private PageCollection tutorialContent;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool pausePlayerMovement = false;

    [Header("Optional: Visual Settings")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);

    private bool hasTriggered = false;
    private Collider2D triggerCollider;
    private PlayerController pausedPlayer;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce)
            return;

        if (other.CompareTag("Player"))
        {
            TriggerTutorial(other.gameObject);
        }
    }

    private void TriggerTutorial(GameObject player)
    {
        if (tutorialContent == null)
        {
            Debug.LogWarning($"TutorialTrigger on {gameObject.name} has no tutorial content assigned!");
            return;
        }

        if (PopupSystem.Instance == null)
        {
            Debug.LogWarning("PopupSystem.Instance not found!");
            return;
        }

        if (pausePlayerMovement)
        {
            pausedPlayer = player.GetComponent<PlayerController>();
            if (pausedPlayer != null)
            {
                pausedPlayer.enabled = false;
                StartCoroutine(WaitForPopupToClose());
            }
        }

        PopupSystem.Instance.Show(tutorialContent);

        hasTriggered = true;

        if (triggerOnce)
        {
            triggerCollider.enabled = false;
        }
    }

    private IEnumerator WaitForPopupToClose()
    {
        yield return new WaitForSeconds(0.1f);

        while (PopupSystem.Instance != null && PopupSystem.Instance.IsPopupActive)
        {
            yield return null;
        }

        if (pausedPlayer != null)
        {
            pausedPlayer.enabled = true;
            pausedPlayer = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo)
            return;

        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
            return;

        Gizmos.color = gizmoColor;

        if (col is BoxCollider2D boxCollider)
        {
            Vector3 center = transform.position + (Vector3)boxCollider.offset;
            Vector3 size = boxCollider.size;
            size.x *= transform.localScale.x;
            size.y *= transform.localScale.y;
            Gizmos.DrawCube(center, size);
        }
        else if (col is CircleCollider2D circleCollider)
        {
            Vector3 center = transform.position + (Vector3)circleCollider.offset;
            float radius = circleCollider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);
            Gizmos.DrawSphere(center, radius);
        }
    }
}
