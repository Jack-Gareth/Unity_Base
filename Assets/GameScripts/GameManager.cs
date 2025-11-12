using System.Collections;
using System.Collections.Generic;
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
            {
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
#endif
            return _instance;
        }
    }

    [Header("Managers")]
    [SerializeField] private LevelManager _levelManager;
    public LevelManager GameLevelManager => _levelManager;

    public SaveData SaveData => SaveManager.CurrentSave;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        SaveManager.Load();
    }

    void OnApplicationQuit()
    {
        SaveManager.Save();
    }
}
