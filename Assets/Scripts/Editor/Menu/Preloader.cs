using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    public CanvasGroup fadeGroup; //assign in inspector
    public bool BypassThisScene;
    public float minLogoTime = 3.0f; // min time for preload scene dispolay of logo
    public float gameLoadTime;
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;

    private void Start() 
    {
        if (BypassThisScene) SceneManager.LoadScene("Menu");
        
        fadeGroup.alpha = 1; // white screen to start
        
        //preload the game here

        //display logo while preload
        if (Time.time < minLogoTime) //if preload is fast, allow 2s to see logo and assign loadtime
        {
            //Debug.Log("Fast");
            gameLoadTime = minLogoTime; 
        }
        else //slow preload - if load time is slow, display logo until complete and assign loadtime
        {
            //Debug.Log("Slow");
            gameLoadTime = Time.time; 
        }
    }

    private void Update()
    {

        // Fade-in phase
        if (Time.time < minLogoTime)
        {
            fadeGroup.alpha = fadeInTime - Time.time; //2s fade in
        }

        //Fade-out phase
        if (Time.time > minLogoTime && gameLoadTime != 0) //loadtime not equal to zero (as its been assigned due to game loaded)
        {
            //Debug.Log(" Fadeout");
            fadeGroup.alpha = (Time.time - gameLoadTime) / fadeOutTime;// 2s fadeout
            if (fadeGroup.alpha >= 1)
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

}
