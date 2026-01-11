using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerJump jump;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();

    }

    private void OnDisable()
    {
            GameInputManager.Instance.OnMoveInput -= OnMoveInput;
            GameInputManager.Instance.OnJumpInput -= OnJumpInput;
    }

    private void OnMoveInput(Vector2 input)
    {
        if (!GameManager.Instance.Settings.CanMove)
            return;

        if (movement == null) return;
    }

    private void OnJumpInput()
    {
        if (!GameManager.Instance.Settings.CanJump)
            return;

        if (jump == null) return;
    }
}
