using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    [Header("Input Events")]
    // --- Movement ---
    public Action<Vector2> OnMoveInput;
    public Action OnJumpInput;

    // --- Unified Color Change ---
    public Action<LevelColor> OnColorChangeInput;

    // --- Abilities ---
    public Action OnRedPhaseAbilityInput;
    public Action OnRedPhaseAbilityReleased;
    public Action OnActivateAbilityInput;

    // --- Color Cycling ---
    public Action OnCycleColorLeftInput;
    public Action OnCycleColorRightInput;

    // --- Utility ---
    public Action OnResetToWhiteInput;
    public Action OnContinueDialogueInput;
    public Action OnConfirmInput;

    private InputSystem_Actions playerInputActions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        // --- Movement ---
        playerInputActions.Player.Move.performed += OnMovePerformed;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Jump.performed += OnJumpPerformed;

        // --- Unified Color Change ---
        playerInputActions.Player.ColorChange.performed += OnColorChangePerformed;

        // --- Abilities ---
        playerInputActions.Player.RedPhaseAbility.performed += OnRedPhaseAbilityPerformed;
        playerInputActions.Player.RedPhaseAbility.canceled += OnRedPhaseAbilityCanceled;
        playerInputActions.Player.ActivateAbility.performed += OnActivateAbilityPerformed;

        // --- Color Cycling ---
        playerInputActions.Player.CycleColorLeft.performed += OnCycleColorLeftPerformed;
        playerInputActions.Player.CycleColorRight.performed += OnCycleColorRightPerformed;

        // --- Utility ---
        playerInputActions.Player.ResetToWhite.performed += OnResetToWhitePerformed;
        playerInputActions.Player.ContinueDialogue.performed += OnContinueDialoguePerformed;
        playerInputActions.Player.Confirm.performed += OnConfirmPerformed;
    }

    private void OnEnable() => playerInputActions?.Enable();
    private void OnDisable() => playerInputActions?.Disable();

    private void OnDestroy()
    {
        if (playerInputActions == null) return;

        // --- Movement ---
        playerInputActions.Player.Move.performed -= OnMovePerformed;
        playerInputActions.Player.Move.canceled -= OnMoveCanceled;
        playerInputActions.Player.Jump.performed -= OnJumpPerformed;

        // --- Unified Color Change ---
        playerInputActions.Player.ColorChange.performed -= OnColorChangePerformed;

        // --- Abilities ---
        playerInputActions.Player.RedPhaseAbility.performed -= OnRedPhaseAbilityPerformed;
        playerInputActions.Player.RedPhaseAbility.canceled -= OnRedPhaseAbilityCanceled;
        playerInputActions.Player.ActivateAbility.performed -= OnActivateAbilityPerformed;

        // --- Color Cycling ---
        playerInputActions.Player.CycleColorLeft.performed -= OnCycleColorLeftPerformed;
        playerInputActions.Player.CycleColorRight.performed -= OnCycleColorRightPerformed;

        // --- Utility ---
        playerInputActions.Player.ResetToWhite.performed -= OnResetToWhitePerformed;
        playerInputActions.Player.ContinueDialogue.performed -= OnContinueDialoguePerformed;
        playerInputActions.Player.Confirm.performed -= OnConfirmPerformed;

        playerInputActions.Dispose();
    }

    // --- Movement ---
    private void OnMovePerformed(InputAction.CallbackContext ctx) => OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => OnMoveInput?.Invoke(Vector2.zero);
    private void OnJumpPerformed(InputAction.CallbackContext ctx) => OnJumpInput?.Invoke();

    // --- Unified Color Change ---
    private void OnColorChangePerformed(InputAction.CallbackContext ctx)
    {
        // The binding name (as defined in Input Actions, e.g., "Red", "Blue", etc.)
        string bindingName = ctx.action.activeControl?.name ?? string.Empty;

        if (Enum.TryParse(bindingName, true, out LevelColor color))
        {
            OnColorChangeInput?.Invoke(color);
            PlayerEvents.TriggerColorChange(color);
        }
        else
        {
            Debug.LogWarning($"GameInputManager: Unknown color binding '{bindingName}'.");
        }
    }

    // --- Abilities ---
    private void OnRedPhaseAbilityPerformed(InputAction.CallbackContext ctx)
    {
        OnRedPhaseAbilityInput?.Invoke();
        PlayerEvents.TriggerGravityFlip();
    }

    private void OnRedPhaseAbilityCanceled(InputAction.CallbackContext ctx) => OnRedPhaseAbilityReleased?.Invoke();

    private void OnActivateAbilityPerformed(InputAction.CallbackContext ctx)
    {
        OnActivateAbilityInput?.Invoke();
        OnRedPhaseAbilityInput?.Invoke();
        PlayerEvents.TriggerGravityFlip();
    }

    // --- Color Cycling ---
    private void OnCycleColorLeftPerformed(InputAction.CallbackContext ctx) => OnCycleColorLeftInput?.Invoke();
    private void OnCycleColorRightPerformed(InputAction.CallbackContext ctx) => OnCycleColorRightInput?.Invoke();

    // --- Utility ---
    private void OnResetToWhitePerformed(InputAction.CallbackContext ctx) => OnResetToWhiteInput?.Invoke();
    private void OnContinueDialoguePerformed(InputAction.CallbackContext ctx) => OnContinueDialogueInput?.Invoke();
    private void OnConfirmPerformed(InputAction.CallbackContext ctx) => OnConfirmInput?.Invoke();
}
