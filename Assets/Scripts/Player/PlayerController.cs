using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;

    private bool isSubscribed = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
    }

    private void Start()
    {
        SubscribeToInputManager();
    }

    private void OnEnable()
    {
        SubscribeToInputManager();
    }

    private void OnDisable()
    {
        if (GameInputManager.Instance != null && isSubscribed)
        {
            GameInputManager.Instance.OnMoveInput -= OnMoveInput;
            GameInputManager.Instance.OnJumpInput -= OnJumpInput;
        }

        isSubscribed = false;
    }

    private void SubscribeToInputManager()
    {
        if (isSubscribed || GameInputManager.Instance == null)
            return;

        GameInputManager.Instance.OnMoveInput += OnMoveInput;
        GameInputManager.Instance.OnJumpInput += OnJumpInput;

        isSubscribed = true;
    }

    private void OnMoveInput(Vector2 input)
    {
        if (!GameManager.Instance.Settings.CanMove)
            return;

        if (movement == null) return;
        movement.SetMoveInput(input);
    }

    private void OnJumpInput()
    {
        if (!GameManager.Instance.Settings.CanJump)
            return;

        if (jump == null) return;
    }
}
