using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour // use this to to avoid manual singleton setup
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    Debug.LogError($"[Singleton] No instance of {typeof(T)} found in scene.");
                }
            }
            return _instance;
        }
    }

    [SerializeField] private bool dontDestroyOnLoad = false;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }
}
