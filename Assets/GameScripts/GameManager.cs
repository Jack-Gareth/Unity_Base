using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this as an entry point. Keep a reference to core game systems here.
/// </summary>
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
                //in editor, we can start any scene to test, so we are not sure the game manager will have been
                //created by the first scene starting the game. So we load it manually. This check is useless in
                //player build as the 1st scene will have created the GameManager so it will always exists.
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
#endif
            return _instance;
        }
    }

    //Have references to all the core systems here for example
    //public PlayerController Player { get; set; }  unless we use eventhubs they we can reference them from there

    /*

    [SerializeField] private GameOptions _gameOptions;
    public GameOptions GameOptions
    {
        get
        {
            if (_gameOptions == null)
            {
                _gameOptions = Resources.Load<GameOptions>("Options/GameOptions");
                if (_gameOptions == null) Debug.LogError("GameOptions not found. Make sure it is in a Resources folder.");
            }
            return _gameOptions;
        }
    }   
            Will add this in later 
    */

    void Awake()
    {
        _instance = this;
    }

}
