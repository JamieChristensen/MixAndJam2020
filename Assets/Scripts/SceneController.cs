﻿using System.Collections;
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

    private bool goingToNextLevel = false;
    public void GoToNextLevel()
    {
        if (goingToNextLevel)
        {
            return;
        }
        LevelManager.CurrentLevel++;
        if (LevelManager.CurrentLevel > LevelManager.GetNumberOfLevels())
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }
        else
        {
            SceneManager.LoadScene(LevelManager.GetCurrentLevel().AssociatedLevel);
        }
        goingToNextLevel = true;
    }

}
