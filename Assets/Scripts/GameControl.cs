using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    //GAZ - ADAPT THIS AND ADD TO GAME MANAGER 
    
    public CanvasGroup fadeGroup; //for fade-in using fade alpha
    public float fadeInDuration = 1f; // fade-in time
    public bool gameStarted; // ready to play
    
private void Start()
{
    fadeGroup = FindObjectOfType<CanvasGroup>();

    // set fade to 1
    fadeGroup.alpha = 1;
}

private void Update()
{
    if( Time.timeSinceLevelLoad <= fadeInDuration )
    {
        // Initial Fade-in
        fadeGroup.alpha = 1 - (Time.timeSinceLevelLoad / fadeInDuration);

    }
    else if (!gameStarted) //fade-in complete, but game not started
    {
        fadeGroup.alpha = 0; //ensure alpha is zero on game start
        gameStarted = true;
    }
    
}

    public void CompleteLevel()
    {
        Debug.Log("//GC - Complete the level and save the progress: " + LevelManager.Instance.currentLevel);
        SaveManager.Instance.CompleteLevel(LevelManager.Instance.currentLevel);

        //Focus the level selection after game play
        LevelManager.Instance.menuFocus = 1;

        ExitScene();
    }

    public void ExitScene()
    {   
        SceneManager.LoadScene("Menu");
    }
}
