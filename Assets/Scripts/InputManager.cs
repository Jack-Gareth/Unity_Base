using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance { get; private set; }

    [Header("Input Events")]
    public System.Action<Vector2> OnMoveInput;
    public System.Action OnJumpInput;
    public System.Action OnRedColorInput;
    public System.Action OnBlueColorInput;
    public System.Action OnGreenColorInput;
    public System.Action OnYellowColorInput;
    public System.Action OnPinkColorInput;
    public System.Action OnBrownColorInput;
    public System.Action OnGravityFlipInput;
    public System.Action OnRedPhaseAbilityInput;

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

        playerInputActions.Player.Move.performed += OnMovePerformed;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Jump.performed += OnJumpPerformed;
        playerInputActions.Player.RedColor.performed += OnRedColorPerformed;
        playerInputActions.Player.BlueColor.performed += OnBlueColorPerformed;
        playerInputActions.Player.GreenColor.performed += OnGreenColorPerformed;
        playerInputActions.Player.YellowColor.performed += OnYellowColorPerformed;
        playerInputActions.Player.PinkColor.performed += OnPinkColorPerformed;
        playerInputActions.Player.BrownColor.performed += OnBrownColorPerformed;
        playerInputActions.Player.GravityFlip.performed += OnGravityFlipPerformed;
        playerInputActions.Player.RedPhaseAbility.performed += OnRedPhaseAbilityPerformed;
    }

    private void OnEnable()
    {
        playerInputActions?.Enable();
    }

    private void OnDisable()
    {
        playerInputActions?.Disable();
    }

    private void OnDestroy()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Move.performed -= OnMovePerformed;
            playerInputActions.Player.Move.canceled -= OnMoveCanceled;
            playerInputActions.Player.Jump.performed -= OnJumpPerformed;
            playerInputActions.Player.RedColor.performed -= OnRedColorPerformed;
            playerInputActions.Player.BlueColor.performed -= OnBlueColorPerformed;
            playerInputActions.Player.GreenColor.performed -= OnGreenColorPerformed;
            playerInputActions.Player.YellowColor.performed -= OnYellowColorPerformed;
            playerInputActions.Player.PinkColor.performed -= OnPinkColorPerformed;
            playerInputActions.Player.BrownColor.performed -= OnBrownColorPerformed;
            playerInputActions.Player.GravityFlip.performed -= OnGravityFlipPerformed;
            playerInputActions.Player.RedPhaseAbility.performed -= OnRedPhaseAbilityPerformed;
            playerInputActions.Dispose();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        OnMoveInput?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        OnMoveInput?.Invoke(Vector2.zero);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        OnJumpInput?.Invoke();
    }

    private void OnRedColorPerformed(InputAction.CallbackContext context)
    {
        OnRedColorInput?.Invoke();
    }

    private void OnBlueColorPerformed(InputAction.CallbackContext context)
    {
        OnBlueColorInput?.Invoke();
    }

    private void OnGreenColorPerformed(InputAction.CallbackContext context)
    {
        OnGreenColorInput?.Invoke();
    }

    private void OnYellowColorPerformed(InputAction.CallbackContext context)
    {
        OnYellowColorInput?.Invoke();
    }

    private void OnPinkColorPerformed(InputAction.CallbackContext context)
    {
        OnPinkColorInput?.Invoke();
    }

    private void OnBrownColorPerformed(InputAction.CallbackContext context)
    {
        OnBrownColorInput?.Invoke();
    }

    private void OnGravityFlipPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("GameInputManager: OnGravityFlipPerformed called - H key pressed!");
        OnGravityFlipInput?.Invoke();
        PlayerEvents.TriggerGravityFlip();
    }

    private void OnRedPhaseAbilityPerformed(InputAction.CallbackContext context)
    {
        OnRedPhaseAbilityInput?.Invoke();
    }
}
