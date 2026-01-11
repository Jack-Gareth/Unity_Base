using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;
<<<<<<< HEAD
=======
    private PlayerWallClimb wallClimb;
>>>>>>> origin/main

    private bool isSubscribed = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
<<<<<<< HEAD
=======
        wallClimb = GetComponent<PlayerWallClimb>();
>>>>>>> origin/main
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
<<<<<<< HEAD
        movement.SetMoveInput(input);
=======

        wallClimb ??= GetComponent<PlayerWallClimb>();

        movement.SetMoveInput(input);
        wallClimb?.SetMoveInput(input);
>>>>>>> origin/main
    }

    private void OnJumpInput()
    {
        if (!GameManager.Instance.Settings.CanJump)
            return;

        if (jump == null) return;
    }
}
