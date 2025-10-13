using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Input Events")]
    public System.Action<Vector2> OnMoveInput;
    public System.Action OnJumpInput;
    public System.Action OnRedColorInput;
    public System.Action OnBlueColorInput;
    public System.Action OnGreenColorInput;
    
    private PlayerInputActions playerInputActions;
    
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
        playerInputActions = new PlayerInputActions();
        
        playerInputActions.Player.Move.performed += OnMovePerformed;
        playerInputActions.Player.Move.canceled += OnMoveCanceled;
        playerInputActions.Player.Jump.performed += OnJumpPerformed;
        playerInputActions.Player.RedColor.performed += OnRedColorPerformed;
        playerInputActions.Player.BlueColor.performed += OnBlueColorPerformed;
        playerInputActions.Player.GreenColor.performed += OnGreenColorPerformed;
    }
    
    private void OnEnable()
    {
        playerInputActions?.Enable();
        Debug.Log("InputManager: Input actions enabled");
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
        Debug.Log("InputManager: Red color input detected (E key)");
        OnRedColorInput?.Invoke();
    }
    
    private void OnBlueColorPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("InputManager: Blue color input detected (R key)");
        OnBlueColorInput?.Invoke();
    }
    
    private void OnGreenColorPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("InputManager: Green color input detected (T key)");
        OnGreenColorInput?.Invoke();
    }
}