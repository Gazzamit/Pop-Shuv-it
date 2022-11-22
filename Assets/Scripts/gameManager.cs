using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    public AudioSource music; 
    public bool startPlaying;
    public beatScroller theBS;
    public animStateController animStateControl;
    public Transform playerPOS;
    public Transform leftTarget;
    public Transform rightTarget;
    public Transform middleTarget;
    public float leftRightSpeed;
    private float elapsedTime;
    public bool movingLeft_ML = false;
    public bool movingLeft_RM = false;
    public bool movingRight_MR = false;
    public static gameManager instance;
    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;
    public float totalArrows, normalHits, goodHits, perfectHits, missedHits;
    public int[] getHighScores = new int[5];
    public string[] getHighScoreNames = new string[5];
    public RectTransform highScoreLocation;

    public TextMeshProUGUI percentHitText, normalsText, goodsText, perfectsText, missesText, finalScoreText;

    public GameObject ArrowsParent;
    public GameObject resultsScreen;
    public GameObject pauseScreen;
    public GameObject highScoresScreen;

    public static float perfectPercent = 20f; //hit accuracy
    public static float goodPercent = 50f; //hit accuracy

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThreshold; 


    public CanvasGroup fadeGroup; //for fade-in using fade alpha
    public float fadeInDuration = 1f; // fade-in time

    public AnimationCurve lerpMovingLeft_ML;
    public AnimationCurve lerpMovingLeft_RM;
    public AnimationCurve lerpMovingRight_MR;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        scoreText.text = "Score 0";

        currentMultiplier = 1;

        totalArrows = ArrowsParent.transform.childCount; //count notes
        //Debug.Log("Total Arrows: " + totalArrows);

        fadeGroup = FindObjectOfType<CanvasGroup>(); // find canvas group for alpha fade

        // set fade to 1
        fadeGroup.alpha = 1;

      

    }

    // Update is called once per frame
    void Update()
    {

        

        if ( Time.timeSinceLevelLoad <= fadeInDuration )
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
                animStateControl.startPlayAnim = true;
                music.Play();
            }
        }
        else
        {
            //Call results screen - DELETE EXIT AFTER 20S IN IF STATEMENT
            if((!music.isPlaying && !resultsScreen.activeInHierarchy && !pauseScreen.activeInHierarchy) || Time.timeSinceLevelLoad >= 50f)
            {
                Debug.Log("Result Screen called");
                resultsScreen.SetActive(true);
                normalsText.text = normalHits.ToString();
                goodsText.text = goodHits.ToString();
                perfectsText.text = perfectHits.ToString();
                missesText.text = missedHits.ToString();

                float percentHit = ((normalHits + goodHits + perfectHits) / totalArrows) * 100f;

                percentHitText.text = percentHit.ToString("F1") + "%";         

                finalScoreText.text = currentScore.ToString();

                startPlaying = false;
                theBS.hasStarted = false;

                music.Stop();    
            }
        }
        if (resultsScreen.activeInHierarchy && Input.anyKeyDown) // can reload scene
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }





        //move from middle to left
        if (movingLeft_ML == true)
        {

            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / leftRightSpeed;

            playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, leftTarget.transform.position, lerpMovingLeft_ML.Evaluate(percentageComplete));
            //playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, leftTarget.transform.position, percentageComplete);
            animStateControl.leaningLeftAnim = true;

            if (playerPOS.transform.position == leftTarget.transform.position)
            {

                animStateControl.leaningLeftAnim = false;
                movingLeft_ML = false;
                
                elapsedTime = 0;
            }

        }


        //move from right to middle
        if (movingLeft_RM == true)
        {

            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / leftRightSpeed;

            playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, middleTarget.transform.position, lerpMovingLeft_RM.Evaluate(percentageComplete));
            //playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, middleTarget.transform.position, percentageComplete);
            animStateControl.leaningLeftAnim = true;

            if (playerPOS.transform.position == middleTarget.transform.position)
            {

                movingLeft_RM = false;
                animStateControl.leaningLeftAnim = false;
                elapsedTime = 0;
            }

        }



        //move from middle to right
        if (movingRight_MR == true)
        {

            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / leftRightSpeed;

            playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, rightTarget.transform.position, lerpMovingRight_MR.Evaluate(percentageComplete));
            //playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, rightTarget.transform.position, percentageComplete);



            if (playerPOS.transform.position == rightTarget.transform.position)
            {

                movingRight_MR = false;
                elapsedTime = 0;

            }

        }



    }




    public void moveLeft_ML()

    {
        movingLeft_ML = true;
    }

    public void moveLeft_RM()

    {
        movingLeft_RM = true;
    }

    public void moveRight_MR()
    {
        movingRight_MR = true;
    }

  




    public void NoteHit()
    {
        //Debug.Log("Hit on Time");

        if (currentMultiplier - 1 < multiplierThreshold.Length)
        {
            multiplierTracker++;

            if (multiplierThreshold[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }

            multiplierText.text = "Multiplier: x" + currentMultiplier;

            //currentScore += scorePerNote * currentMultiplier; //see below for variety of hits
            scoreText.text = "Score: " + currentScore;        
        }
    }

    public void NormalHit()
    {
        Debug.Log("Normal Hit");
        currentScore += scorePerNote * currentMultiplier; 
        NoteHit();
        normalHits++;
    }

    public void GoodHit()
    {
        Debug.Log("Good Hit");
        currentScore += scorePerGoodNote * currentMultiplier; 
        NoteHit();
        goodHits++;
    }

    public void PerfectHit()
    {
        Debug.Log("Perfect Hit");        
        currentScore += scorePerPerfectNote * currentMultiplier; 
        NoteHit();
        perfectHits++;
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

    //For testing completion of level
    public void CompleteLevelButton()
    {
        Debug.Log("Completed level and saved progress: " + Manager.Instance.currentLevel); //Works from Preloader only (to access Manager)
        SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel); //Works from Preloader only (to access Manager)

        //Focus the level selection after game play
        Manager.Instance.menuFocus = 1;//Works from Preloader only (to access Manager)

        SceneManager.LoadScene("Menu");//Works from Preloader only
    }

    //For exiting the scene
    public void ExitSceneButton()
    {   
        Debug.Log("Exit Scene");
        pauseScreen.SetActive(false);
        AudioListener.pause = false;
        Time.timeScale = 1;
        Manager.Instance.menuFocus = 0; //Works from Preloader only (to access Manager)
        SceneManager.LoadScene("Menu");//Works from Preloader only 

    }

    public void PauseOptionsButton()
    {
        Debug.Log("Pause Screen");
        pauseScreen.SetActive(true);
        AudioListener.pause = true;
        Time.timeScale = 0;
    }

    public void RestartButton()
    {   
        Debug.Log("Restart");
        pauseScreen.SetActive(false);
        AudioListener.pause = false;
        Time.timeScale = 1;
    }
    
    public void HighScoresButtonInGame()
    {
        //selected in pause screen
        Debug.Log("High Scores Screen");

        //Get and sort data
        for (int i = 0; i < 5; i++)
        {
            for (int j = i + 1; j < 5; j++)
            {
                getHighScores[i] = SaveManager.Instance.state.highScoresSaved[i];
                getHighScores[j] = SaveManager.Instance.state.highScoresSaved[j];
                getHighScoreNames[i] = SaveManager.Instance.state.highScoreNameSaved[i];
                getHighScoreNames[j] = SaveManager.Instance.state.highScoreNameSaved[j];
                if (getHighScoreNames[i] == null) getHighScoreNames[i] = "---";
                if (getHighScoreNames[j] == null) getHighScoreNames[j] = "---";
                if (getHighScores[j] > getHighScores[i])
                {
                    //swap
                    int tmpScore = getHighScores[i];
                    getHighScores[i] = getHighScores[j];
                    getHighScores[j] = tmpScore;
                    string tmpName = getHighScoreNames[i];
                    getHighScoreNames[i] = getHighScoreNames[j];
                    getHighScoreNames[j] = tmpName;
                }
            }
        }

        for (int k = 0; k < 5; k++)
        {
            highScoreLocation.GetChild(k).GetChild(1).GetComponent<TextMeshProUGUI>().text = getHighScores[k].ToString();
            highScoreLocation.GetChild(k).GetChild(2).GetComponent<TextMeshProUGUI>().text = getHighScoreNames[k];
            Debug.Log("High Score:" + k + ": " + getHighScores[k] + " - " + getHighScoreNames[k]);
        }    
            
        
        highScoresScreen.SetActive(true);
        //AudioListener.pause = true;
        //Time.timeScale = 0;
    }
    public void HighScoresBackButtonInGame()
    {
        Debug.Log("High Scores Back Button");
        highScoresScreen.SetActive(false);
        //AudioListener.pause = true;
        //Time.timeScale = 0;
    }

}
