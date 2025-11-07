using UnityEngine;

public class PlayerController : MonoBehaviour
{
     private PlayerMovement movement;
     private PlayerJump jump;
     private PlayerWallClimb wallClimb;
     private bool isSubscribed = false;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        wallClimb = GetComponent<PlayerWallClimb>();
    }

    private void Start()
    {
        SubscribeToInputManager();
    }

    private void OnEnable()
    {
        SubscribeToInputManager();
    }

    private void SubscribeToInputManager()
    {
        if (isSubscribed)
            return;

        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnMoveInput += OnMoveInput;
            GameInputManager.Instance.OnJumpInput += OnJumpInput;
            isSubscribed = true;
        }
    }

    private void OnDisable()
    {
        if (GameInputManager.Instance != null && isSubscribed)
        {
            GameInputManager.Instance.OnMoveInput -= OnMoveInput;
            GameInputManager.Instance.OnJumpInput -= OnJumpInput;
            isSubscribed = false;
        }
    }


    private void OnMoveInput(Vector2 input)
    {
        movement.SetMoveInput(input);
        wallClimb?.SetMoveInput(input);
    }

    private void OnJumpInput()
    {
        jump.TryJump();
        wallClimb?.TryWallJump();
    }
}
