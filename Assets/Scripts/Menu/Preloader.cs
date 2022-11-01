using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preloader : MonoBehaviour
{
    public CanvasGroup fadeGroup; //assign in inspector
    public float minLogoTime = 3.0f; // min time for preload scence dispolay of logo
    public float gameLoadTime;
    public float fadeInTime = 1f;
    public float fadeOutTime = 2f;

    private void Start() 
    {

        fadeGroup.alpha = 1; // white screen to start
        
        //preload the game here

        //display logo while preload
        if (Time.time < minLogoTime) //if preload is fast, allow 3s to see logo and assign loadtime
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
            fadeGroup.alpha = fadeInTime - Time.time; //1s fade in
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
