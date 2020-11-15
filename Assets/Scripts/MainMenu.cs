using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public LevelManager LevelManager;

    public void SetStartLevel(int levelIndex)
    {
        LevelManager.CurrentLevel = levelIndex;
    }

    public void SwapScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
