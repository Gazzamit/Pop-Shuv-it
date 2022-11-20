using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class gameManager : MonoBehaviour
{   
    public bool startPlayingALevel = false;
    public bool endOfLevel = false, showFailed = false, ShowHighScore = false, ShowEnterName = false, Pause = false;
    public bool newHighScore = false, showResults = false, showLevelScore = false;
    public bool pauseOptionsClicked = false, showPauseOptions = false, continueClicked = false, exitClicked = false;
    public bool enterNameOKClicked = false, checkIfHighScore = true, HighScoresBackButtonClicked = false, clickToRestartLevel = false, clickToRestartLevelClicked = false;
    public bool canvasButtonClicked = false, HighScoresButtonClicked = false;
    
    public AudioSource music; 
    public beatScroller theBS;
    public animStateController animStateControl;


    public static gameManager instance;

    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;
    public float totalArrows, normalHits = 0, goodHits = 0, perfectHits = 0, missedHits = 0;
    public int[] getHighScores = new int[5];
    public string[] getHighScoreNames = new string[5];
    public string newHighScoreName = "";
    public RectTransform highScoreLocation;

    public TextMeshProUGUI percentHitText, normalsText, goodsText, perfectsText, missesText, finalScoreText;

    public GameObject ArrowsParent;

    public int arrowMissedCounter;
    public GameObject resultsScreen;
    public GameObject pauseScreen;
    public GameObject ScoresScreenInsert;
    public GameObject highScoresScreen;
    public GameObject failedLevelScreen;
    public GameObject EnterNameScreen;

    public static float perfectPercent = 20f; //hit accuracy
    public static float goodPercent = 50f; //hit accuracy

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public TMP_InputField enterNameInputField;

    public GameObject[] livesIcons = new GameObject [4];
    public int[] LooseLifeAtCount = new int[4];
    public int livesLeft = 4;

    public GameObject player;
    public GameObject playerBody;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThreshold; 

    public CanvasGroup fadeGroup; //for fade-in using fade alpha
    public float fadeInDuration = 1f; // fade-in time

    // Start is called before the first frame update
    void Start()
    {
        Selection.activeGameObject = gameObject;
       
        instance = this;

        scoreText.text = "Score 0";

        currentMultiplier = 1;

        totalArrows = ArrowsParent.transform.childCount; //count notes
        //Debug.Log("Total Arrows: " + totalArrows);

        fadeGroup = FindObjectOfType<CanvasGroup>(); // find canvas group for alpha fade

        // set fade to 1
        fadeGroup.alpha = 1;

        //This usess in skin set in the Menuscene - only works if loaded from the preloader
        //SetPlayerSkin();
    }

    // Update is called once per frame
    void Update()
    {       
        // Initial Fade-in
        if( Time.timeSinceLevelLoad <= fadeInDuration )
        {
            // Initial Fade-in
            fadeGroup.alpha = 1 - (Time.timeSinceLevelLoad / fadeInDuration);

        }        
        else if (!startPlayingALevel) // When loaded Start Game Play and key press
        {
            fadeGroup.alpha = 0; //ensure alpha is zero on game start
            
            if (canvasButtonClicked && !showPauseOptions && !ShowEnterName) //key pressed and not paused
            {
                Debug.Log("Start Playing Level");
                StartLevelPlay();
                startPlayingALevel = true;
                animStateControl.startPlayAnim = true;
                canvasButtonClicked = false;
            }
        }

        //pause during gameplay
        if (pauseOptionsClicked) 
        {
            showPauseOptions = true;
            clickToRestartLevel = false;
            ShowPauseOptionsScreen(); //In game Pause screen

            if(HighScoresButtonClicked) //high score clicked
            {
                SortHighScores(); //Retrieve and Sort so that can calc if high score
                PrepareHighScoresForDisplay(); 
                ShowHighScoreScreen();
                HighScoresButtonClicked = false;
            }
            else if (continueClicked) //pause menu
            {
                showPauseOptions = false;
                HidePauseOptionsScreen();
                ContinuePlayingGame(); 
                continueClicked = false;
                pauseOptionsClicked = false; 
            }
            else if (exitClicked) //pause menu
            {   
                showPauseOptions = false;
                HidePauseOptionsScreen();
                ExitToMenu();
                exitClicked = false;
                pauseOptionsClicked = false; 
            }
            else if (HighScoresBackButtonClicked)
            {
                HideHighScoreScreen();
                HideEnterNameScreen();
                HighScoresBackButtonClicked = false;
            }
        }
       
       if (showFailed)
        {
            CalcEndOfLevelScore();
            ShowFailedScreen();
            ShowLevelScoresInsert();
            StopLevelPlay();
            if (!pauseOptionsClicked && canvasButtonClicked) // can reload scene
            {
                    Debug.Log("Level Reloading...");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);    
                    canvasButtonClicked = false;            
            }
        }
        
        //end of level show results screen
        if (endOfLevel && !showFailed) 
        {
            StopLevelPlay(); // end beat scroller etc
            CalcEndOfLevelScore(); //calc percent hits etc

            //show results (if end of level)
            ShowResultsScreen();
            //showResults = false;
            ShowLevelScoresInsert();
            //sshowLevelScore = false;
            
            //Check if a new high score
            if (checkIfHighScore)
            {
                SortHighScores(); //Sort so that can calc if high score
                PrepareHighScoresForDisplay();
                if (currentScore > getHighScores[4])
                {
                    Debug.Log("New High Score " + currentScore);
                    newHighScore = true;
                }
                checkIfHighScore = false;
            }
            
            //If therre is a high score
            if (newHighScore)
            {
                if(ShowHighScore == false)
                {
                    ShowHighScore = true;
                    ShowHighScoreScreen();
                } 
                if (ShowEnterName == false)
                {
                    ShowEnterName = true;
                    ShowEnterNameScreen();
                }

                if (enterNameOKClicked)
                {
                    GetUserName(); //new high score name
                    InsertNameInHighScoreArray(); // array is stored sorted so add to 4th place and sort
                    HideEnterNameScreen(); // hide enter name dialogue
                    PrepareHighScoresForDisplay(); // write the high score values to TMP elements
                    SaveCorrectHighScoresToPlayerPrefs(); // save the sorted array to player prefs 
                    enterNameOKClicked = false;
                    ShowEnterName = false;
                    newHighScore = false;
                }

            }
            else if (ShowHighScore && HighScoresBackButtonClicked)
            {
                HideHighScoreScreen();
                HideEnterNameScreen();
                HighScoresBackButtonClicked = false;
                ShowHighScore = false;
            }
            else if (!pauseOptionsClicked && !ShowHighScore && canvasButtonClicked) // can reload scene
            {
                Debug.Log("Level Reloading...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);    
                canvasButtonClicked = false;            
            }
        }

        //End of level Triggered in Game
        if (startPlayingALevel && !music.isPlaying && !endOfLevel && !showPauseOptions && !ShowHighScore)
        {
            Debug.Log("endOfLevel=true");
            //Display results
            endOfLevel = true;
        }
        
        //Reduce lives in normal game play
        if (livesLeft != 0 && !endOfLevel || !showFailed || !showPauseOptions) 
        {
            //4 lives to start
            if (arrowMissedCounter == LooseLifeAtCount[0]) livesIcons[0].SetActive(false); //3 lives
            if (arrowMissedCounter == LooseLifeAtCount[1]) livesIcons[1].SetActive(false); //2 lives
            if (arrowMissedCounter == LooseLifeAtCount[2]) livesIcons[2].SetActive(false); //1 lives
            if (arrowMissedCounter == LooseLifeAtCount[3]) 
            {
                livesIcons[3].SetActive(false); //0 lives
                showFailed = true;
                livesLeft = 0; //lives now 0
            }
        }

        //reset canvas clicked
        canvasButtonClicked = false;

    }

    /////////////////////////////
    //Arrow Hits scoring system//
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
        arrowMissedCounter++;
    }

    public void WrongDirection()
    {
        Debug.Log("wrong direction");

        currentMultiplier = 1;
        multiplierTracker = 0;
        scoreText.text = "Score: " + currentScore;
        arrowMissedCounter++;
    }

    ///////////////////////////
    //////////BUTTONS//////////

    //For testing completion of level
    public void CompleteLevelButton()
    {
        Debug.Log("Completed level and saved progress: " + Manager.Instance.currentLevel); //Works from Preloader only (to access Manager)
        SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel); //Works from Preloader only (to access Manager)

        //Focus the level selection after game play
        Manager.Instance.menuFocus = 1;//Works from Preloader only (to access Manager)

        SceneManager.LoadScene("Menu");//Works from Preloader only
    }

    //For testing End Of Level script actions
    public void endTheLevelNowButtonClick()
    {
        endOfLevel = true;
    }

    //Click Pause
    public void PauseOptionsClick()
    {
        Debug.Log("Pause Clicked");
        pauseOptionsClicked = true; //call ShowPauseOptionsScreen
    }

    //Click Exit (inside pause screen)
    public void ExitClick()
    {
        Debug.Log("Exit Clicked");
        exitClicked = true;
    }

    //Click continue (inside pause screen)
    public void ContinueButtonClick()
    {   
        Debug.Log("Continue Button Clicked");
        continueClicked = true;
    }
    
    //Enter name on high score
    public void EnterNameOKButtonClick()
    {
        Debug.Log("Enter Name Button Clicked");
        enterNameOKClicked = true;
    }
    
    //Click on canvas anywhere
    public void CanvasButtonClick()
    {   
        Debug.Log("Canvas Clicked");
        canvasButtonClicked = true;
    }
    
    //hig scores back button
    public void HighScoresBackButtonClick()
    {
        Debug.Log("High Scores Back Button");
        HighScoresBackButtonClicked = true;
    }
    
    //high scores button in pause / options menu
    public void HighScoresButtonClick()
    {
        Debug.Log("High Scores Button Click");
        HighScoresButtonClicked = true;
    }


    /////////////////////////
    //Process button clicks//
    public void ExitToMenu()
    {   
        AudioListener.pause = false;
        Time.timeScale = 1;
        Manager.Instance.menuFocus = 0; //Works from Preloader only (to access Manager)
        SceneManager.LoadScene("Menu");//Works from Preloader only 
    }

    public void ContinuePlayingGame()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
    }

    public void GetUserName()
    {
        newHighScoreName =  enterNameInputField.text; //assigned in inspector
        Debug.Log("GetUserName: " + newHighScoreName);
    }

    public void StopLevelPlay()
    {
        //startPlayingALevel = false;
        theBS.hasStarted = false;
        music.Stop();  
    }

    public void StartLevelPlay()
    {
        theBS.hasStarted = true;
        music.Play();  
    }

    /////////////////////////////
    //Show/Hide in game screens//
    public void ShowPauseOptionsScreen()
    {
        Debug.Log("Show pause Screen");
        pauseScreen.SetActive(true);
        AudioListener.pause = true;
        Time.timeScale = 0;
    }

    public void HidePauseOptionsScreen()
    {
        pauseScreen.SetActive(false);
    }
    
    public void ShowResultsScreen()
    {
        Debug.Log("Show Results Screen");
        resultsScreen.SetActive(true);
    }

    public void HideResultsScreen()
    {
        Debug.Log("Hide Results Screen");
        resultsScreen.SetActive(false);
    }

    public void ShowFailedScreen()
    {
        Debug.Log("Show Failed Screen");
        failedLevelScreen.SetActive(true);
    }

    public void HideFailedScreen()
    {
        Debug.Log("Hide Failed Screen");
        failedLevelScreen.SetActive(false);
    }
    
    public void ShowLevelScoresInsert()
    {
        Debug.Log("Show Level Scores Insert");
        ScoresScreenInsert.SetActive(true);
    }

    public void HideLevelScoresInsert()
    {
        Debug.Log("Hide Level Scores Insert");
        ScoresScreenInsert.SetActive(false);
    }

    public void ShowHighScoreScreen()
    {
        Debug.Log("Show High Scores Screen");
        highScoresScreen.SetActive(true);
    }
    
    public void HideHighScoreScreen()
    {
        Debug.Log("Hide High Scores Screen");
        highScoresScreen.SetActive(false);
    }

    public void ShowEnterNameScreen()
    {
        Debug.Log("Show Enter Names Screen");
        EnterNameScreen.SetActive(true);
    }
    
    public void HideEnterNameScreen()
    {
        Debug.Log("Hide Enter Name Screen");
        EnterNameScreen.SetActive(false);
    }

    ////////////////////
    //Calculate Scores//
    public void CalcEndOfLevelScore()
    {
        //on results / failed level
        normalsText.text = normalHits.ToString();
        goodsText.text = goodHits.ToString();
        perfectsText.text = perfectHits.ToString();
        missesText.text = missedHits.ToString();

        float percentHit = ((normalHits + goodHits + perfectHits) / totalArrows) * 100f;

        percentHitText.text = percentHit.ToString("F1") + "%";         

        finalScoreText.text = currentScore.ToString();
    }
    
    public void SortHighScores()
    {
        Debug.Log("RUN FROM PRELOADER - Sort High Scores from player prefs");
        //Get and sort data
        for (int i = 0; i < 5; i++)
        {
            for (int j = i + 1; j < 5; j++)
            {
                getHighScores[i] = SaveManager.Instance.state.highScoresSaved[i];
                getHighScores[j] = SaveManager.Instance.state.highScoresSaved[j];
                getHighScoreNames[i] = SaveManager.Instance.state.highScoreNameSaved[i];
                getHighScoreNames[j] = SaveManager.Instance.state.highScoreNameSaved[j];
                if (getHighScoreNames[i] == null) getHighScoreNames[i] = "---"; //if empty
                if (getHighScoreNames[j] == null) getHighScoreNames[j] = "---"; //if empty
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

    }

    public void InsertNameInHighScoreArray()
    {
        Debug.Log("Insert name into array at highest position");
        //add to forth place (as already sorted adn is a new score)
        if (currentScore > getHighScores[4])
        {
            getHighScores[4] = currentScore;
            getHighScoreNames[4] = newHighScoreName;
        }
        //sort array
        for (int i = 0; i < 5; i++)
        {
            for (int j = i + 1; j < 5; j++)
            {
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
    }

    public void PrepareHighScoresForDisplay()
    {
        //push to highScores Screen
        Debug.Log("Push High scores to screen");
        for (int k = 0; k < 5; k++)
        {
            highScoreLocation.GetChild(k).GetChild(1).GetComponent<TextMeshProUGUI>().text = getHighScores[k].ToString();
            highScoreLocation.GetChild(k).GetChild(2).GetComponent<TextMeshProUGUI>().text = getHighScoreNames[k];
            //Debug.Log("High Score:" + k + ": " + getHighScores[k] + " - " + getHighScoreNames[k]);
        }    
            
    }

    public void SaveCorrectHighScoresToPlayerPrefs()
    {
        Debug.Log("Save sorted high scores to player prefs");
         for (int m=0; m < 5; m++)           
        {
            SaveManager.Instance.state.highScoresSaved[m] = getHighScores[m];
            SaveManager.Instance.state.highScoreNameSaved[m] = getHighScoreNames[m];
            Debug.Log("Save High Scores[" + m + 1 + "] is: " + getHighScores[m] + ". Name: " + getHighScoreNames[m]);
        }
    }

    ///////////////////////
    //set skin for player//
    public void SetPlayerSkin()
    {
        int index =  SaveManager.Instance.state.activeSkinTShirt; //Get saved T-shirt colour index
        Manager.Instance.playerMaterials = playerBody.GetComponent<SkinnedMeshRenderer>().materials; //get current player materials
        Manager.Instance.playerMaterials[2].color = Manager.Instance.playerTshirtColorOptions[index]; //set colour of material 2 to colour index of the colours set up in inspector
    }
}
