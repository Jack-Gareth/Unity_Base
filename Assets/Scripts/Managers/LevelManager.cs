using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManager", menuName = "Managers/LevelManager", order = 0)]
public class LevelManager : ScriptableObject
{
    public Action OnScreenShow; //This event is called whenever the Uncover Screen finishes, indicating the player can now see the game
    public Action OnScreenHide; //This event is called whenever the Cover Screen finishes, indicating the player can no longer see the game
    public Action<Action> StartScreenHide; //This event is called whenever the Cover Screen starts

    public void LoadSceneFade(int levelNum)
    {
        StartScreenHide?.Invoke(() => Load(levelNum));
    }

    public void LoadSceneInstant(int levelNum)
    {
        Load(levelNum);
    }

    private void Load(int num)
    {
        OnScreenHide?.Invoke();
        Time.timeScale = 1;
        AudioListener.pause = false;
        SceneManager.LoadScene(num);
    }

    public void InvokeScreenVisible()
    {
        OnScreenShow?.Invoke();
    }

}
