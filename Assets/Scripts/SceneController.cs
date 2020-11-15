using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public LevelManager LevelManager;

    public string MainMenuSceneName;

    public void StartWinSequence()
    {
        StartCoroutine(WinSequence());
    }

    public IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(3.0f);
        GoToNextLevel();
    }


    public void GoToNextLevel()
    {
        LevelManager.CurrentLevel++;
        if (LevelManager.CurrentLevel > LevelManager.GetNumberOfLevels())
        {
            SceneManager.LoadScene(MainMenuSceneName);
        } else
        {
            SceneManager.LoadScene(LevelManager.GetCurrentLevel().AssociatedLevel);
        }
    }

}
