using UnityEngine;

public class PlayerController : MonoBehaviour
{
     private PlayerMovement movement;
     private PlayerJump jump;
     private PlayerWallClimb wallClimb;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
        wallClimb = GetComponent<PlayerWallClimb>();
    }

    private void OnEnable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnMoveInput += OnMoveInput;
            GameInputManager.Instance.OnJumpInput += OnJumpInput;
        }
    }

    private void OnDisable()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.OnMoveInput -= OnMoveInput;
            GameInputManager.Instance.OnJumpInput -= OnJumpInput;
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
