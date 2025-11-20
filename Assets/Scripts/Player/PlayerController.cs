using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
    private PlayerWallClimb wallClimb;
    private PlayerWallFriction wallFriction;

    private bool isSubscribed = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        wallClimb = GetComponent<PlayerWallClimb>();
        wallFriction = GetComponent<PlayerWallFriction>();
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

        wallClimb ??= GetComponent<PlayerWallClimb>();
        wallFriction ??= GetComponent<PlayerWallFriction>();

        movement.SetMoveInput(input);
        wallClimb?.SetMoveInput(input);
        wallFriction?.SetMoveInput(input);
    }

    private void OnJumpInput()
    {
        if (!GameManager.Instance.Settings.CanJump)
            return;

        if (jump == null) return;

        wallClimb ??= GetComponent<PlayerWallClimb>();

        bool didWallJump = wallClimb != null && wallClimb.TryWallJump();
        if (!didWallJump)
            jump.TryJump();
    }
}
