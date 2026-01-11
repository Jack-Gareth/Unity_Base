using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    public Action<Vector2> OnMoveInput;
    public Action OnJumpInput;
    public Action OnContinueDialogueInput;
    public Action OnConfirmInput;

    private InputSystem_Actions playerInputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SetupInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupInputActions()
    {
        playerInputActions = new InputSystem_Actions();

        playerInputActions.Player.Move.performed += OnMovePerformed;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;

        playerInputActions.Player.Jump.started += OnJumpPerformed;

        playerInputActions.Player.ContinueDialogue.performed += OnContinueDialoguePerformed;
        playerInputActions.Player.Confirm.performed += OnConfirmPerformed;
    }

    private void OnEnable() => playerInputActions?.Enable();
    private void OnDisable() => playerInputActions?.Disable();

    private void OnDestroy()
    {
        if (playerInputActions == null) return;

        playerInputActions.Player.Move.performed -= OnMovePerformed;
        playerInputActions.Player.Move.canceled -= OnMoveCanceled;

        playerInputActions.Player.Jump.started -= OnJumpPerformed;

        playerInputActions.Player.ContinueDialogue.performed -= OnContinueDialoguePerformed;
        playerInputActions.Player.Confirm.performed -= OnConfirmPerformed;

        playerInputActions.Dispose();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx) =>
        OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());

    private void OnMoveCanceled(InputAction.CallbackContext ctx) =>
        OnMoveInput?.Invoke(Vector2.zero);

    private void OnJumpPerformed(InputAction.CallbackContext ctx) =>
        OnJumpInput?.Invoke();


    private void OnContinueDialoguePerformed(InputAction.CallbackContext ctx) =>
        OnContinueDialogueInput?.Invoke();

    private void OnConfirmPerformed(InputAction.CallbackContext ctx) =>
        OnConfirmInput?.Invoke();
}
