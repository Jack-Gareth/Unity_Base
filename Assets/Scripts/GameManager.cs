using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
#if UNITY_EDITOR
            if (_instance == null)
                Instantiate(Resources.Load<GameManager>("GameManager"));
#endif
            return _instance;
        }
    }

    [SerializeField] private GameManager_SO gameSettings;
    public GameManager_SO Settings => gameSettings;

    // --- Global Tracked Game Stats ---
    public int ItemsCollected { get; private set; }
    public int TotalItems { get; private set; }
    public int DeathCount { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSettingsObject();
    }

    private void OnEnable()
    {
        // Subscribe to UI-independent global events

    }

    private void OnDisable()
    {

    }

    private void EnsureSettingsObject()
    {
        if (gameSettings == null)
        {
            gameSettings = Resources.Load<GameManager_SO>("GameManagerSettings");

            if (gameSettings == null)
            {
                gameSettings = ScriptableObject.CreateInstance<GameManager_SO>();
            }
        }
    }


    public void SetMovementEnabled(bool enabled) => gameSettings.CanMove = enabled;
    public void SetJumpEnabled(bool enabled) => gameSettings.CanJump = enabled;

    public void DisablePlayerControl()
    {
        gameSettings.CanMove = false;
        gameSettings.CanJump = false;
    }

    public void EnablePlayerControl()
    {
        gameSettings.CanMove = true;
        gameSettings.CanJump = true;
    }
}
