using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    //game control bools
    public bool startPlayingALevel = false;
    public bool endOfLevel = false, showFailed = false, ShowHighScore = false, ShowEnterName = false, Pause = false;
    public bool newHighScore = false, showResults = false, showLevelScore = false;
    public bool pauseOptionsClicked = false, showPauseOptions = false, continueClicked = false, exitClicked = false;
    public bool enterNameOKClicked = false, checkIfHighScore = true, HighScoresBackButtonClicked = false, clickToRestartLevel = false, clickToRestartLevelClicked = false;
    public bool canvasButtonClicked = false, HighScoresButtonClicked = false, SetUnlockNextLevel = false;
    public bool SetOneStarUnlocked = false, SetTwoStarsUnlocked = false, SetThreeStarsUnlocked = false;
    public int livesLeft = 4;
    public int[] LooseLifeAtCount = new int[4]; //set missed arrow count amounts to loose lives in ins[ector
    public int[] starTargetScores = new int[3]; //set in inspector for each level for bonus at end of level
    public int starcounter = 0, numtokens, starsPreviouslyAchieved;

    public AudioSource music;
    public beatScroller theBS;

    //Player animation
    public animStateController animStateControl;
    public Transform playerPOS;
    public Transform leftTarget;
    public Transform rightTarget;
    public Transform middleTarget;
    public float leftRightSpeed;
    private float elapsedTime;

    //private bool movingLeft = false;

    public GameObject player;
    public GameObject playerBody;
    public GameObject playerBoard;
    public GameObject ArrowsParent;
    public GameObject resultsScreen;
    public GameObject pauseScreen;
    public GameObject ScoresScreenInsert;
    public GameObject highScoresScreen;
    public GameObject failedLevelScreen;
    public GameObject EnterNameScreen;
    public RectTransform highScoreLocation;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThreshold;
    public int arrowMissedCounter;

    public bool movingLeft_ML = false;
    public bool movingLeft_RM = false;
    public bool movingRight_MR = false;
    public bool movingRight_LM = false;

    public int currentScore;
    public int scorePerNote = 100;
    public int scorePerGoodNote = 125;
    public int scorePerPerfectNote = 150;
    public float totalArrows, normalHits = 0, goodHits = 0, perfectHits = 0, missedHits = 0;
    private int[] getHighScores = new int[5];
    private string[] getHighScoreNames = new string[5];
    public string newHighScoreName = "";

    public static float perfectPercent = 20f; //hit accuracy
    public static float goodPercent = 50f; //hit accuracy

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;
    public TMP_InputField enterNameInputField;

    public TextMeshProUGUI percentHitText, normalsText, goodsText, perfectsText, missesText, finalScoreText;
    public GameObject noStarsText, oneOfThreeStars, twoOfThreeStarsText, levelClearedText, PlusFiveShopTokens;
    public GameObject[] starIcons = new GameObject[5]; //star icons - assign in inspector
    public GameObject[] livesIcons = new GameObject[4]; //lives icons - assign in inspector
    private GameObject[] GameObjectWithButton = new GameObject[9]; //for creating buttons using onClick rather than in inspector
    private Button[] b = new Button[9];

    public CanvasGroup fadeGroup; //for fade-in using fade alpha
    public float fadeInDuration = 1f; // fade-in time

    public AnimationCurve lerpMovingLeft_ML;
    public AnimationCurve lerpMovingLeft_RM;
    public AnimationCurve lerpMovingRight_MR;
    public AnimationCurve lerpMovingRight_LM;


    // Start is called before the first frame update
    void Start()
    {
        //////////IMPORTANT////////////

        //Scritp below is required to load preloader if not loaded
        //'Preloader' Scene can be set to bypass (load next 'Menu' scene fast) in 'Preloader' Scene inspector.
        //'Menu' Scene can be set to bypass (load the named scene) in inspector 'Menu' Scene.
        //You have to tick 'Bypass This Scene' in MenuScene and Preloader for those scenes to be bypassed and load quickly.
        //AFter Git push, you may need to renter your scene that you are working on in 'Menu' Scene. 
        if (!GameObject.Find("SaveManager")) SceneManager.LoadScene("Preloader");

        //init UI buttons
        //canvas click
        GameObjectWithButton[0] = GameObject.Find("ScreenCanvasButtonClick");
        b[0] = GameObjectWithButton[0].GetComponent<Button>();
        b[0].onClick.AddListener(() => CanvasButtonClick());
        //Debug.Log("Init CanvasButtonClick: " + GameObjectWithButton[0].name);

        //pause options click
        GameObjectWithButton[1] = GameObject.Find("PauseOptionsClick");
        b[1] = GameObjectWithButton[1].GetComponent<Button>();
        b[1].onClick.AddListener(() => PauseOptionsClick());
        //Debug.Log("Init PauseOptionsClick: " + GameObjectWithButton[1].name);

        //end level now TEMP buton
        GameObjectWithButton[2] = GameObject.Find("ParentendTheLevelNowButtonClick");
        b[2] = GameObjectWithButton[2].transform.GetChild(0).GetComponent<Button>();
        b[2].onClick.AddListener(() => endTheLevelNowButtonClick());
        //Debug.Log("Init endTheLevelNowButtonClick: " + GameObjectWithButton[2].transform.GetChild(0).name);

        //Unlock next level
        GameObjectWithButton[3] = GameObject.Find("ParentUnlockNextLevel");
        b[3] = GameObjectWithButton[3].transform.GetChild(0).GetComponent<Button>();
        b[3].onClick.AddListener(() => UnlockNextLevel());
        //Debug.Log("Init UnlockNextLevel: " + GameObjectWithButton[3].transform.GetChild(0).name);

        //Continue button click
        GameObjectWithButton[4] = GameObject.Find("ParentPauseOptionsMenu");
        b[4] = GameObjectWithButton[4].transform.GetChild(0).GetChild(2).GetComponent<Button>();
        b[4].onClick.AddListener(() => ContinueButtonClick());
        //Debug.Log("Init ContinueButtonClick: " + GameObjectWithButton[4].transform.GetChild(0).GetChild(2).name);

        //Pause option - Menu click
        GameObjectWithButton[5] = GameObject.Find("ParentPauseOptionsMenu");
        b[5] = GameObjectWithButton[5].transform.GetChild(0).GetChild(3).GetComponent<Button>();
        b[5].onClick.AddListener(() => PauseOptionsMenuClick());
        //Debug.Log("Init PauseOptionsMenuClick: " + GameObjectWithButton[5].transform.GetChild(0).GetChild(3).name);

        //High score button click
        GameObjectWithButton[6] = GameObject.Find("ParentPauseOptionsMenu");
        b[6] = GameObjectWithButton[6].transform.GetChild(0).GetChild(4).GetComponent<Button>();
        b[6].onClick.AddListener(() => HighScoresButtonClick());
        //Debug.Log("Init HighScoresButtonClick: " + GameObjectWithButton[6].transform.GetChild(0).GetChild(4).name);

        //High Score Back Button
        GameObjectWithButton[7] = GameObject.Find("ParentHighScoresInGame");
        b[7] = GameObjectWithButton[7].transform.GetChild(0).GetChild(2).GetComponent<Button>();
        b[7].onClick.AddListener(() => HighScoresBackButtonClick());
        //Debug.Log("Init HighScoresBackButtonClick: " + GameObjectWithButton[7].transform.GetChild(0).GetChild(2).name);

        //Enter Name button inside high scores
        GameObjectWithButton[8] = GameObject.Find("ParentHighScoresInGame");
        b[8] = GameObjectWithButton[8].transform.GetChild(0).GetChild(4).GetChild(4).GetComponent<Button>();
        b[8].onClick.AddListener(() => EnterNameOKButtonClick());
        //Debug.Log("Init EnterNameOKButtonClick: " + GameObjectWithButton[8].transform.GetChild(0).GetChild(4).GetChild(4).name);

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
        SetPlayerSkinBody();
        SetPlayerSkinBoard();
    }

    // Update is called once per frame
    void Update()

    {
        // Initial Fade-in
        if (Time.timeSinceLevelLoad <= fadeInDuration)

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

            if (HighScoresButtonClicked) //high score clicked
            {
                ShowHighScore = true;
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
                ShowHighScore = false;
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
            //showLevelScore = false;

            //Set stars active if any previously achieved. If not achieved, 'calc level' below will unlock them now.
            starsPreviouslyAchieved = SaveManager.Instance.state.starsPerLevel[Manager.Instance.currentLevel];
            Debug.Log("Stars Previously Achieved: " + starsPreviouslyAchieved);
            for (int i = 0; i < 4; i++) //debug saved stars per level (scene)
            {
                Debug.Log("Stars Saved Scene " + i + ": " + SaveManager.Instance.state.starsPerLevel[i] + " stars.");
            }

            //check first if three stars previously unlocked
            if (starsPreviouslyAchieved == 3)
            {
                starIcons[0].SetActive(true); //Three stars correctly positioned
                starIcons[3].SetActive(true); //
                starIcons[4].SetActive(true); //
                levelClearedText.SetActive(true);
                SetThreeStarsUnlocked = false; // level does not need unlocking
            }
            //else chec if 3 stars newly unlocked
            else if (currentScore >= starTargetScores[2])
            {
                starcounter = 3;
                starIcons[0].SetActive(true); //Three stars correctly positioned
                starIcons[3].SetActive(true); //
                starIcons[4].SetActive(true); //
                levelClearedText.SetActive(true);
                PlusFiveShopTokens.SetActive(true);
                SetUnlockNextLevel = true;
                SetThreeStarsUnlocked = true; //override above if level already unlocked
            }

            //check if two stars previously unloced
            else if (starsPreviouslyAchieved == 2)
            {
                starIcons[1].SetActive(true); //show 2 stars
                starIcons[2].SetActive(true); //
                twoOfThreeStarsText.SetActive(true);
                SetTwoStarsUnlocked = false; //level already unlocked
            }
            //else check if 2 stars newly unlocked
            else if (currentScore >= starTargetScores[1] && currentScore < starTargetScores[2])
            {
                starcounter = 2;
                starIcons[1].SetActive(true); //Two stars correctly positioned
                starIcons[2].SetActive(true); //
                twoOfThreeStarsText.SetActive(true);
                SetTwoStarsUnlocked = true;
            }

            //check lastly if one star previously unloced
            else if (starsPreviouslyAchieved == 1)
            {
                starIcons[0].SetActive(true); //show 1 star
                oneOfThreeStars.SetActive(true);
                SetOneStarUnlocked = false; //level already unlocked
            }
            //else check if 1 star newly unlocked
            else if (currentScore >= starTargetScores[0] && currentScore < starTargetScores[1])
            {
                starcounter = 1;
                starIcons[0].SetActive(true); //show 1 star
                oneOfThreeStars.SetActive(true);
                SetOneStarUnlocked = true;
            }
            else
            {
                noStarsText.SetActive(true);
            }

            if (SetOneStarUnlocked)
            {
                SetOneStarUnlocked = false;
                SaveManager.Instance.state.starsPerLevel[Manager.Instance.currentLevel] = 1;
                SaveManager.Instance.Save();
            }
            if (SetTwoStarsUnlocked)
            {
                SetTwoStarsUnlocked = false;
                SaveManager.Instance.state.starsPerLevel[Manager.Instance.currentLevel] = 2;
                SaveManager.Instance.Save();
            }
            if (SetThreeStarsUnlocked && SetUnlockNextLevel)
            {
                UnlockNextLevel();
                SetUnlockNextLevel = false;
                numtokens = SaveManager.Instance.state.token;
                SaveManager.Instance.state.token = numtokens + 5;
                SaveManager.Instance.state.starsPerLevel[Manager.Instance.currentLevel] = 3;
                SaveManager.Instance.Save();
                SetThreeStarsUnlocked = false;
            }
            //Check if a new high score
            for (int i = 0; i < 4; i++)
            {
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
            }
            //If therre is a high score
            if (newHighScore)
            {
                if (ShowHighScore == false)
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
                if (starcounter == 3)
                {
                    Debug.Log("Return to Menu. level Completed.");
                    SceneManager.LoadScene("Menu");//Goto Menu as Level completed - Works from Preloader only
                }
                else
                {
                    Debug.Log("Level Reloading...");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    canvasButtonClicked = false;
                }
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
            if (arrowMissedCounter == LooseLifeAtCount[0])
            {
                livesIcons[0].SetActive(false); //3 lives
                livesLeft = 3;
            }
            else if (arrowMissedCounter == LooseLifeAtCount[1])
            {
                livesIcons[1].SetActive(false); //2 lives
                livesLeft = 2;
            }
            else if (arrowMissedCounter == LooseLifeAtCount[2])
            {
                livesIcons[2].SetActive(false); //1 lives
                livesLeft = 1;
            }
            else if (arrowMissedCounter == LooseLifeAtCount[3])
            {
                //TDO RESET BELOW TO 0 AND TRUE
                livesIcons[3].SetActive(false); //0 lives
                showFailed = true;
                livesLeft = 0; //lives now 0
            }
        }





        //moving left from middle to left
        if (movingLeft_ML == true)
        {

            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / leftRightSpeed;

            playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, leftTarget.transform.position, lerpMovingLeft_ML.Evaluate(percentageComplete));
            //playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, leftTarget.transform.position, percentageComplete);
            animStateControl.leaningLeftAnim = true;

            if (elapsedTime >= 0.6f)
            {
                animStateControl.leaningLeftAnim = false;
                

            }

            if (playerPOS.transform.position == leftTarget.transform.position)
            {

                animStateControl.leaningLeftAnim = false;
                movingLeft_ML = false;
                
                elapsedTime = 0;
            }

        }


        //reset canvas clicked
        canvasButtonClicked = false;

        //moving left from right to middle
        if (movingLeft_RM == true)
        {

            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / leftRightSpeed;

            playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, middleTarget.transform.position, lerpMovingLeft_RM.Evaluate(percentageComplete));
            //playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, middleTarget.transform.position, percentageComplete);
            animStateControl.leaningLeftAnim = true;

            if (elapsedTime >= 0.6f)
            {
                animStateControl.leaningLeftAnim = false;
                

            }



            if (playerPOS.transform.position == middleTarget.transform.position)
            {

                movingLeft_RM = false;
                animStateControl.leaningLeftAnim = false;
                elapsedTime = 0;
            }

        }



        //moving right from middle to right
        if (movingRight_MR == true)
        {

            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / leftRightSpeed;

            playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, rightTarget.transform.position, lerpMovingRight_MR.Evaluate(percentageComplete));
            //playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, rightTarget.transform.position, percentageComplete);
            animStateControl.leaningRightAnim = true;

            if (elapsedTime >= 0.6f)
            {
                animStateControl.leaningRightAnim = false;
                

            }

            if (playerPOS.transform.position == rightTarget.transform.position)
            {

                movingRight_MR = false;
                animStateControl.leaningRightAnim = false;
                elapsedTime = 0;

            }

        }


        //moving right from left to middle
        if (movingRight_LM == true)
        {

            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / leftRightSpeed;

            playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, middleTarget.transform.position, lerpMovingRight_LM.Evaluate(percentageComplete));
            //playerPOS.transform.position = Vector3.Lerp(playerPOS.transform.position, middleTarget.transform.position, percentageComplete);
            animStateControl.leaningRightAnim = true;

            if (elapsedTime >= 0.6f)
            {
                animStateControl.leaningRightAnim = false;
                

            }

            if (playerPOS.transform.position == middleTarget.transform.position)
            {

                movingRight_LM = false;
                animStateControl.leaningRightAnim = false;
                elapsedTime = 0;

            }

        }




    }


    /////////////////////////////
    //Arrow Hits scoring system//


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

    public void moveRight_LM()
    {
        movingRight_LM = true;
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
    public void UnlockNextLevel()
    {
        Debug.Log("Completed level and saved progress: " + Manager.Instance.currentLevel); //Works from Preloader only (to access Manager)
        SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel); //Works from Preloader only (to access Manager)

        //Focus the level selection after game play
        Manager.Instance.menuFocus = 1;//Works from Preloader only (to access Manager)
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
    public void PauseOptionsMenuClick()
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
        newHighScoreName = enterNameInputField.text; //assigned in inspector
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
        Debug.Log("PRELOADER required");
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
        for (int m = 0; m < 5; m++)
        {
            SaveManager.Instance.state.highScoresSaved[m] = getHighScores[m];
            SaveManager.Instance.state.highScoreNameSaved[m] = getHighScoreNames[m];
            Debug.Log("Save High Scores[" + m + 1 + "] is: " + getHighScores[m] + ". Name: " + getHighScoreNames[m]);
        }
    }

    ///////////////////////
    //set skin for player//
    public void SetPlayerSkinBody()
    {
        //the array playerbodymaterials is set here and works with donotdestroyonload
        int index = SaveManager.Instance.state.activeSkinTShirt; //Get saved T-shirt colour index
        Debug.Log("T-Shirt ID: " + index);
        Manager.Instance.playerBodyMaterials = playerBody.GetComponent<SkinnedMeshRenderer>().materials; //get current player materials
        Manager.Instance.playerBodyMaterials[2].color = Manager.Instance.playerTshirtColorOptions[index]; //set colour of material 2 to colour index of the colours set up in inspector
    }
    
    ///////////////////////
    //set skin for board//
    public void SetPlayerSkinBoard()
    {
        //the array playerboardmaterials is set here and works with donotdestroyonload
        int index = SaveManager.Instance.state.activeSkinBoard; //Get saved board colour index
        Debug.Log("Board ID: " + index);
        Manager.Instance.playerBoardMaterials = playerBoard.GetComponent<MeshRenderer>().materials; //get current board materials
        Manager.Instance.playerBoardMaterials[0].color = Manager.Instance.playerBoardColorOptions[index]; //set colour of material 1 to colour index of the colours set up in inspector
    }
}
