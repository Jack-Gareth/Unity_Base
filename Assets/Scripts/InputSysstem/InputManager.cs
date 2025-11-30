using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    public Action<Vector2> OnMoveInput;
    public Action OnJumpInput;
    public Action<LevelColor> OnColorChangeInput;
    public Action OnCycleColorLeftInput;
    public Action OnCycleColorRightInput;
    public Action OnColorAbilityInput;
    public Action OnResetToWhiteInput;
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

        playerInputActions.Player.ColorChange.performed += OnColorChangePerformed;
        playerInputActions.Player.ColorAbility.performed += OnColorAbilityPerformed;

        playerInputActions.Player.CycleColorLeft.performed += OnCycleColorLeftPerformed;
        playerInputActions.Player.CycleColorRight.performed += OnCycleColorRightPerformed;

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

        playerInputActions.Player.ColorChange.performed -= OnColorChangePerformed;
        playerInputActions.Player.ColorAbility.performed -= OnColorAbilityPerformed;

        playerInputActions.Player.CycleColorLeft.performed -= OnCycleColorLeftPerformed;
        playerInputActions.Player.CycleColorRight.performed -= OnCycleColorRightPerformed;

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

    private void OnColorChangePerformed(InputAction.CallbackContext ctx)
    {
        LevelColor current = LevelColorManager.Instance.CurrentColor;
        int next = ((int)current + 1) % Enum.GetValues(typeof(LevelColor)).Length;
        LevelColor nextColor = (LevelColor)next;

        OnColorChangeInput?.Invoke(nextColor);
        PlayerEvents.TriggerColorChange(nextColor);
    }

    private void OnColorAbilityPerformed(InputAction.CallbackContext ctx)
    {
        OnColorAbilityInput?.Invoke();
        PlayerEvents.TriggerColorAbility();
    }

    private void OnCycleColorLeftPerformed(InputAction.CallbackContext ctx) =>
        OnCycleColorLeftInput?.Invoke();

    private void OnCycleColorRightPerformed(InputAction.CallbackContext ctx) =>
        OnCycleColorRightInput?.Invoke();

    private void OnResetToWhitePerformed(InputAction.CallbackContext ctx) =>
        OnResetToWhiteInput?.Invoke();

    private void OnContinueDialoguePerformed(InputAction.CallbackContext ctx) =>
        OnContinueDialogueInput?.Invoke();

    private void OnConfirmPerformed(InputAction.CallbackContext ctx) =>
        OnConfirmInput?.Invoke();
}
