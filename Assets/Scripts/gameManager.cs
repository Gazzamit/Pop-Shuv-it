using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public AudioSource music; 
    public bool startPlaying;
    public beatScroller theBS;

    public static gameManager instance;

    public int currentScore;
    public int scorePerNote = 100;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThreshold; 

    public CanvasGroup fadeGroup; //for fade-in using fade alpha
    public float fadeInDuration = 1f; // fade-in time

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        scoreText.text = "Score 0";

        currentMultiplier = 1;

        fadeGroup = FindObjectOfType<CanvasGroup>();

        // set fade to 1
        fadeGroup.alpha = 1;
    }

    // Update is called once per frame
    void Update()
    {       
        if( Time.timeSinceLevelLoad <= fadeInDuration )
        {
            // Initial Fade-in
            fadeGroup.alpha = 1 - (Time.timeSinceLevelLoad / fadeInDuration);

        }
        
        else if (!startPlaying)
        {
            fadeGroup.alpha = 0; //ensure alpha is zero on game start
            
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                theBS.hasStarted = true;

                music.Play();
            }
        }
    }


    public void NoteHit()
    {
        Debug.Log("Hit on Time");

        if (currentMultiplier - 1 < multiplierThreshold.Length)
        {
            multiplierTracker++;

            if (multiplierThreshold[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }

            multiplierText.text = "Multiplier: x" + currentMultiplier;

            currentScore += scorePerNote * currentMultiplier;
            scoreText.text = "Score: " + currentScore;        
        }
    }

    public void NoteMissed()
    {
        Debug.Log("Missed note");

        currentMultiplier = 1;
        multiplierTracker = 0;
        scoreText.text = "Score: " + currentScore;
    }

    public void WrongDirection()
    {
        Debug.Log("wrong direction");

        currentMultiplier = 1;
        multiplierTracker = 0;
        scoreText.text = "Score: " + currentScore;
    }

    public void CompleteLevel()
    {
        Debug.Log("Completed level and saved progress: " + LevelManager.Instance.currentLevel);
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
