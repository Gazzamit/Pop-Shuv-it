using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuScene : MonoBehaviour
{

    public bool BypassThisScene;
    public string SkipToThisScene;
    public CanvasGroup fadeGroup; //assign in inspector

    private float fadeInSpeed = 1f; //think 1/fadeInSpeed to get time
    public float menuLerp = 0.05f;

    public RectTransform menuContainer;

    public Transform skinShirtPanel; //assign in inspector - part of Shop Panel (R)
    public Transform skinBoardPanel; //assign in inspector - part of Shop Panel (R)
    public Transform routePanel; //assign in inspector - Part of Shiop Panel (R)
    public Transform levelPanel; //assign in inspector - Part of Level Panel (L)

    //Text for shop
    public TextMeshProUGUI skinShirtBuySetText;
    public TextMeshProUGUI routeBuySetText; 
    public TextMeshProUGUI skinBoardBuySetText; 
    public TextMeshProUGUI tokenText;

    private MenuCamera menuCam;

    //Shop cost items
    public int[] skinShirtCost = new int[] { 0, 5, 5, 5, 5, 10, 10, 10, 10, 10};
    public int[] skinBoardCost = new int[] { 0, 5, 5, 5, 5, 10, 10, 10, 10, 10}; 
    public int[] routeCost = new int[] { 0, 5, 5, 5, 5, 10, 10, 10, 10, 10};
    // item selected in shop 
    public int selectedSkinShirtIndex; //after selct
    public int selectedSkinBoardIndex; //after selct
    private int selectedRouteIndex; //after select
    private int activeSkinShirtIndex; //after set
    private int activeSkinBoardIndex; //after set
    private int activeRouteIndex; //after set

    public static int selectedShirt;


    private Vector3 desiredMenuPosition;
    public int referenceHorizontalResolution;
    //public String SceneToLoad;


    //Game fade in
    public AnimationCurve enteringLevelZoomCurve;
    private bool isEnteringLevel = false;
    public float zoomDuration = .8f;
    private float zoomTransition = 0;

    //grab player to zoom it in
    public GameObject playerInMenu;
    public GameObject playerBodyInMenu;
    public GameObject playerBoardInMenu;
    private Material[] signMaterials;

    //Shop Shirt select system
    public static Vector3 newPosShirt;
    public static bool selectMoveShirt;
    public Transform shirtContentTransform;
    public Transform centreShopShirt;
    public float shirtScrollLerp;
    public ScrollRect scrollRectShirt;

    //Shop Board select system
    public static Vector3 newPosBoard;
    public static bool selectMoveBoard;
    public Transform boardContentTransform;
    public Transform centreShopBoard;
    public float boardScrollLerp;
    public ScrollRect scrollRectBoard;

    //hide shop window if on oither menus
    public GameObject hideShopWindow;

    //Show specific windows in shop
    public GameObject shirtsWindow;
    public GameObject gripsWindow;

    public GameObject skinShirtBuySetButton;
    public GameObject skinBoardBuySetButton;
     

    private void Awake()
    {
        if (!GameObject.Find("Manager")) 
        {
            StartCoroutine(WaitForSceneLoad());
        }
    }

    private void Start()
    {   
        // $$ TEMP $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        SaveManager.Instance.state.token = 20; 

        //Find menu cam
        menuCam = FindObjectOfType<MenuCamera>();

        //hide shop menu to start
        hideShopWindow.SetActive(true);

        //Position Cam on the Focus Menu    
        //SetFocusTo(Manager.Instance.menuFocus);
        Debug.Log("menuFocus (via Manager): " + Manager.Instance.menuFocus);
        NavigateTo(Manager.Instance.menuFocus);

        //Update the gold text at the start
        UpdateTokenText();

        fadeGroup.alpha = 1; //fade-in

        // add button on-clock events to skin Panel buttons
        InitShop();

        // add button on-click events to levels Panel
        InitLevel();

        //set player prefs for skin and route and board
        OnSkinShirtSelect(SaveManager.Instance.state.activeSkinTShirt);
        SetSkinShirt(SaveManager.Instance.state.activeSkinTShirt);

        OnRouteSelect(SaveManager.Instance.state.activeRoute);
        SetRoute(SaveManager.Instance.state.activeRoute);

        OnSkinBoardSelect(SaveManager.Instance.state.activeSkinBoard);//board
        SetSkinBoard(SaveManager.Instance.state.activeSkinBoard);//board

        // Make buttons bigger for the selected items above - no longer an image. change RectTransform to Transform
        skinShirtPanel.GetChild(SaveManager.Instance.state.activeSkinTShirt).GetComponent<Transform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
        routePanel.GetChild(SaveManager.Instance.state.activeRoute).GetComponent<Transform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
        skinBoardPanel.GetChild(SaveManager.Instance.state.activeSkinBoard).GetComponent<Transform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
        

        //Shirt / Board start position in shop to stored prefs value
        StartCoroutine(SetShirtLoadScrollPosition());
        StartCoroutine(SetBoardLoadScrollPosition());

        //Bypass if set in inspector
        if(BypassThisScene) SceneManager.LoadScene(SkipToThisScene);
    }

    private void Update() 
    {

        // Fade-in menu
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * fadeInSpeed;

        // smooth menu lerp between menus
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D, desiredMenuPosition, menuLerp);

        // Entering level zoom
        if (isEnteringLevel)
        {
            //add to zoom float
            zoomTransition += (1 / zoomDuration) * Time.deltaTime;

            //change the menuContainer to scale using animationCurve
            menuContainer.localScale = Vector3.Lerp(Vector3.one, new Vector3(5f,5f,5f), enteringLevelZoomCurve.Evaluate(zoomTransition));

            //change the desired position of canvas to zoom in centre of level as it zooms
            Vector3 newDesiredPosition = desiredMenuPosition * 5;
            RectTransform rt = levelPanel.GetChild(Manager.Instance.currentLevel).GetComponent<RectTransform>();
            newDesiredPosition -= rt.anchoredPosition3D * 5;

            playerInMenu.transform.localScale = Vector3.Lerp(playerInMenu.transform.localScale, new Vector3(.1f,.1f,.1f), enteringLevelZoomCurve.Evaluate(zoomTransition));
            
            //protect zoom going off left of screen
            if (newDesiredPosition.x > referenceHorizontalResolution * 7f) newDesiredPosition.x = referenceHorizontalResolution * 7f;


            //overide smooth menu lerp above (not switching menus here)
            menuContainer.anchoredPosition3D = Vector3.Lerp(desiredMenuPosition, newDesiredPosition, enteringLevelZoomCurve.Evaluate(zoomTransition));

            //fade to white (overide Fade-in menu above)
            fadeGroup.alpha = zoomTransition;

            // after animation 
            if (zoomTransition >= 1)
            {
                //enter level
                //SceneManager.LoadScene(SceneToLoad); //scene selected in Inspector
                int currentIndex = Manager.Instance.currentLevel;
                Debug.Log("Loading Scene: " + currentIndex);
                SceneManager.LoadScene(currentIndex);
            }
        }
        
        //scroll the Shirt menu
        if (shirtContentTransform.position != newPosShirt && selectMoveShirt)
        {
            //Debug.Log("Lerp Shop To: " + newPosShirt);
            shirtContentTransform.position = Vector3.Lerp(shirtContentTransform.position, newPosShirt, shirtScrollLerp * Time.deltaTime);
        }
        if (Vector3.Distance(shirtContentTransform.position, newPosShirt) < 0.01f)
        {
            shirtContentTransform.position = newPosShirt;
            selectMoveShirt = false;
        }

        //scroll the Board menu
        if (boardContentTransform.position != newPosBoard && selectMoveBoard)
        {
            //Debug.Log("Lerp Shop To: " + newPosBoard);
            boardContentTransform.position = Vector3.Lerp(boardContentTransform.position, newPosBoard, boardScrollLerp * Time.deltaTime);
        }
        if (Vector3.Distance(boardContentTransform.position, newPosBoard) < 0.01f)
        {
            boardContentTransform.position = newPosBoard;
            selectMoveBoard = false;
        }


    }
    
    //-------------------------------Init-----------------------------
    private void InitShop()
    {
        Debug.Log("InitShop Running");
        //SHIRT add click events e.g. 0 to 10 to skin items
        int i = 0;
        foreach (Transform t in skinShirtPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(()=>OnSkinShirtSelect(currentIndex));
            //Debug.Log("Shop listener added: " + b);
            
            // set colour of image is owned or not using tern operator
            //Image img = t.GetComponent<Image>(); //no longer using img

            //img.color = Manager.Instance.playerTshirtColorOptions[currentIndex]; //player color from manager - no longer using img
            Material[] shirtMaterials = t.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().materials;
            //Debug.Log("materials: " + shirtMaterials[2]);
            shirtMaterials[2].color = Manager.Instance.playerTshirtColorOptions[currentIndex];
            
            //Not using dimming - looked weird as selected colours changed when bought
            //img.color = SaveManager.Instance.IsSkinOwned(i) 
            //    ? Manager.Instance.playerTshirtColorOptions[currentIndex] //player color from manager
            //    : Color.Lerp(Manager.Instance.playerTshirtColorOptions[currentIndex], new Color(0,0,0,1),0.25f); // dim the colour

            i++;
        }
        
        //BOARD add click events e.g. 0 to 10 to skin items
        i = 0; //reset
        foreach (Transform t in skinBoardPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(()=>OnSkinBoardSelect(currentIndex));
            //Debug.Log("Shop listener added: " + b);
            
            // set colour of image is owned or not using tern operator
            //Image img = t.GetComponent<Image>(); //no longer using img

            //img.color = Manager.Instance.playerTshirtColorOptions[currentIndex]; //player color from manager - no longer using img
            Material[] boardMaterials = t.GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>().materials;
            //Debug.Log("materials: " + boardMaterials[2]);
            boardMaterials[0].color = Manager.Instance.playerBoardColorOptions[currentIndex];
            
            //Not using dimming - looked weird as selected colours changed when bought
            //img.color = SaveManager.Instance.IsSkinOwned(i) 
            //    ? Manager.Instance.playerBoardColorOptions[currentIndex] //board color from manager
            //    : Color.Lerp(Manager.Instance.playerBoardColorOptions[currentIndex], new Color(0,0,0,1),0.25f); // dim the colour

            i++;
        }

       //add click events e.g. 0 to 10 to route items
        i = 0;
        foreach (Transform t in routePanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(()=>OnRouteSelect(currentIndex));

            // set colour of image is owned or not using tert operator
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsRouteOwned(i) ? Color.white : new Color(0.7f,0.7f,0.7f);

            i++;
        }

    }
    private void InitLevel()
    {
         //add click events e.g. 0 to 5 to levels
        Debug.Log("InitLevel Running");
        int i = 0;
        foreach (Transform t in levelPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(()=>OnLevelSelect(currentIndex));

            //Image img = t.GetComponent<Image>(); // no longer using as was for sprite

            Debug.Log("Level Button Found Index: " + i);
           
           //get the materials of the 2nd gameobjectobject of child of sign
           signMaterials = t.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().materials;

            //Is it unlocked?
            if( i <= SaveManager.Instance.state.completedLevel)
            {
                //Debug.Log("//It is unlocked");
                if ( i == SaveManager.Instance.state.completedLevel)
                {
                    //Debug.Log("//Its not completed");
                    //img.color = Color.white; // no longer using as was for sprite
                    signMaterials[0].color = Color.white;
                }
                else
                {
                    //Debug.Log("//Level is already completed");
                    //img.color = Color.green; // no longer using as was for sprite
                    signMaterials[0].color = new Color(0,1,0,0.5f); //green  
                }
            }
            else
            {
                //Debug.Log("// level is locked, disable button and grey");
                b.interactable = false;
                //img.color = Color.grey; // no longer using as was for sprite
                signMaterials[0].color = new Color(0.3f,0.3f,0.3f,1); //Grey
            }


            i++;
        }
    }


    //--------------------------Menu Control---------------------
    private void NavigateTo (int menuIndex)
    {
        //switch between menus
        switch (menuIndex)
        {
            default: //main menu
            case 0: //main menu
                desiredMenuPosition = Vector3.zero;
                menuCam.BackToMainMenu();
                Debug.Log("NavigateTo: " + menuIndex);
                hideShopWindow.SetActive(true);
                break;
            case 1: //level
                desiredMenuPosition = Vector3.right * referenceHorizontalResolution;
                menuCam.MoveToLevel();
                Debug.Log("NavigateTo: " + menuIndex);
                hideShopWindow.SetActive(true);
                break;
            case 2: //shop
                desiredMenuPosition = Vector3.left * referenceHorizontalResolution;
                menuCam.MoveToShop();
                Debug.Log("NavigateTo: " + menuIndex);
                hideShopWindow.SetActive(true);
                break;
        }
    }

    //----------------------------Shop---------------------------
    private void SetSkinShirt (int index)
    {
        // set the active index on the model
        activeSkinShirtIndex = index;
        SaveManager.Instance.state.activeSkinTShirt = index;  //Set prefs

        // Get the T-shirt and change skin on the model
        Manager.Instance.playerBodyMaterials = playerBodyInMenu.GetComponent<SkinnedMeshRenderer>().materials;
        
        //debugMaterials();

        //Set color of player T Shirt (3rd material)
        Manager.Instance.playerBodyMaterials[2].color = Manager.Instance.playerTshirtColorOptions[index];

        // change buy/set button
        skinShirtBuySetText.text = "Current";

        //Remember Prefs
        SaveManager.Instance.Save();

    }
    private void SetSkinBoard (int index)
    {
        // set the active index on the model
        activeSkinBoardIndex = index;
        SaveManager.Instance.state.activeSkinBoard = index;  //Set prefs

        // Get the Board and change skin on the board
        Manager.Instance.playerBoardMaterials = playerBoardInMenu.GetComponent<MeshRenderer>().materials;
        
        //debugMaterials();

        //Set color of player Board (2nd material)
        Manager.Instance.playerBoardMaterials[0].color = Manager.Instance.playerBoardColorOptions[index];

        // change buy/set button
        skinBoardBuySetText.text = "Current";

        //Remember Prefs
        SaveManager.Instance.Save();

    }
    private void SetRoute (int index)
    {
        // set the active index on the model
        activeRouteIndex = index;
        SaveManager.Instance.state.activeRoute = index;  //Set prefs
        
        // TODO change route

        // change buy/set button
        routeBuySetText.text = "Current";

        //Remember Prefs
        SaveManager.Instance.Save();
    }

    private void UpdateTokenText()
    {
        tokenText.text = SaveManager.Instance.state.token.ToString();
    }

    //-------------------------Menu Nav--------------------------
    public void OnBackClick()
    {
    
        NavigateTo(0);
        Debug.Log("Back button clicked");
    }
    public void OnPlayClick()
    {
        NavigateTo(1);
        Debug.Log("Play button clicked");
    }
    public void OnShopClick()
    {
        NavigateTo(2);
        Debug.Log("Shop button clicked");
    }

    //--------------------------Shirts, Grips buttons etc---------

    public void ButtonShowShirts()
    {
        hideShopWindow.SetActive(false);
        skinShirtBuySetButton.SetActive(true);
        skinBoardBuySetButton.SetActive(false);  

        shirtsWindow.SetActive(true);
        gripsWindow.SetActive(false);
    }

    public void ButtonShowGrips()
    {
        hideShopWindow.SetActive(false);
        skinBoardBuySetButton.SetActive(true); 
        skinShirtBuySetButton.SetActive(false); 

        shirtsWindow.SetActive(false);
        gripsWindow.SetActive(true);
    }

    //--------------------------Shop Buttons----------------------
    private void OnSkinShirtSelect(int currentIndex)
    {
        Debug.Log("Selecting Skin Shirt Button: " + currentIndex);

        //if button clicked is already selected, do nothing
        if (selectedSkinShirtIndex == currentIndex)
        {
            return;
        }
        else 
        {   
            //make icon bigger// added GetChild(0). as now 3D object not img
            skinShirtPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
            //skinShirtPanel.GetChild(currentIndex).GetComponent<Image>().color = Manager.Instance.playerTshirtColorOptions[currentIndex];

            //make last selected icon normal size
            skinShirtPanel.GetChild(selectedSkinShirtIndex).GetComponent<RectTransform>().localScale = Vector3.one;
            //skinShirtPanel.GetChild(selectedSkinIndex).GetComponent<Image>().color 
            //    = Color.Lerp(skinShirtPanel.GetChild(currentIndex).GetComponent<Image>().color, new Color(0,0,0,1),0.25f); // dim the colour
        
            //temporary change colour of Shirt
            Material[] shirtMaterials = playerBodyInMenu.GetComponent<SkinnedMeshRenderer>().materials;
            shirtMaterials[2].color = Manager.Instance.playerTshirtColorOptions[currentIndex];

        }

        //set the selected skin
        selectedSkinShirtIndex = currentIndex;

        //change content of buy/set button (selection can be owned/not owned etc)
        if (SaveManager.Instance.IsSkinShirtOwned(currentIndex))
        {
            //skin owned
            //Already current skin?
            if (activeSkinShirtIndex == currentIndex)
            {
                skinShirtBuySetText.text = "Current";               
            }
            else
            {
                skinShirtBuySetText.text = "Select";
            }

        }
        else
        {
            //skin isn't owned - get skinCost from index and display
            skinShirtBuySetText.text = "Buy: " + skinShirtCost[currentIndex].ToString();
        }

        /*
        //Sort of working code to only show some itmes either side of selected item
        int index = 0;
        foreach (Transform t in skinShirtPanel)
        {
            if((index > (selectedSkinIndex - 3)) && (index < (selectedSkinIndex + 3)))
            {
                Debug.Log("Show Shirt: " + index);
                t.gameObject.SetActive(true);
            }
            // else if ((index < (newShirt - 2)) && (index > (newShirt + 2)))
            //{
            else
            {
                t.gameObject.SetActive(false);
                Debug.Log("Hide Shirt: " + index);
            }

            Debug.Log("selectedSkinIndex: " + selectedSkinIndex + ". Index: " + index);
            index++;
            
        }
        */
    }

    private void OnSkinBoardSelect(int currentIndex)
    {
        Debug.Log("Selecting Skin Board Button: " + currentIndex);

        //if button clicked is already selected, do nothing
        if (selectedSkinBoardIndex == currentIndex)
        {
            return;
        }
        else 
        {   
            //make icon bigger// added GetChild(0). as now 3D object not img
            skinBoardPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
            //skinBoardPanel.GetChild(currentIndex).GetComponent<Image>().color = Manager.Instance.playerBoardColorOptions[currentIndex];

            //make last selected icon normal size
            skinBoardPanel.GetChild(selectedSkinBoardIndex).GetComponent<RectTransform>().localScale = Vector3.one;
            //skinBoardPanel.GetChild(selectedSkinBoardIndex).GetComponent<Image>().color 
            //    = Color.Lerp(skinBoardPanel.GetChild(currentIndex).GetComponent<Image>().color, new Color(0,0,0,1),0.25f); // dim the colour
        
            //temporary change colour of Shirt
            Material[] boardMaterials = playerBoardInMenu.GetComponent<MeshRenderer>().materials;
            boardMaterials[0].color = Manager.Instance.playerBoardColorOptions[currentIndex];

        }

        //set the selected skin
        selectedSkinBoardIndex = currentIndex;

        //change content of buy/set button (selection can be owned/not owned etc)
        if (SaveManager.Instance.IsSkinBoardOwned(currentIndex))
        {
            //skin owned
            //Already current skin?
            if (activeSkinBoardIndex == currentIndex)
            {
                skinBoardBuySetText.text = "Current";               
            }
            else
            {
                skinBoardBuySetText.text = "Select";
            }

        }
        else
        {
            //skin isn't owned - get skinCost from index and display
            skinBoardBuySetText.text = "Buy: " + skinBoardCost[currentIndex].ToString();
        }

        /*
        //Sort of working code to only show some itmes either side of selected item
        int index = 0;
        foreach (Transform t in skinBoardPanel)
        {
            if((index > (selectedSkinBoardIndex - 3)) && (index < (selectedSkinBoardIndex + 3)))
            {
                Debug.Log("Show Board: " + index);
                t.gameObject.SetActive(true);
            }
            // else if ((index < (newBoard - 2)) && (index > (newBoard + 2)))
            //{
            else
            {
                t.gameObject.SetActive(false);
                Debug.Log("Hide Board: " + index);
            }

            Debug.Log("selectedSkinBoardIndex: " + selectedSkinBoardIndex + ". Index: " + index);
            index++;
            
        }
        */
    }

    private void OnRouteSelect(int currentIndex)
    {
        Debug.Log("Selecting Route Button: " + currentIndex);

        //if button clicked is already selected, do nothing
        if (selectedRouteIndex == currentIndex)
        {
            return;
        } 
        else 
        {   
            //make icon bigger
            routePanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);

            //make last selected icon normal size
            routePanel.GetChild(selectedRouteIndex).GetComponent<RectTransform>().localScale = Vector3.one;
        }

        //set the selected route
        selectedRouteIndex = currentIndex;

        //change content of buy/set button (selection can be owned/not owned etc)
        if (SaveManager.Instance.IsRouteOwned(currentIndex))
        {
            //route owned
            //Already current route?
            if (activeRouteIndex == currentIndex)
            {
                routeBuySetText.text = "Current";               
            }
            else
            {
                routeBuySetText.text = "Select";
            }
        }
        else
        {
            //route isn't owned - get routeCost from index and display
            routeBuySetText.text = "Buy: " + routeCost[currentIndex].ToString();
        }
    }

    public void OnSkinShirtBuySet()
    {
        Debug.Log("Buy/set Skin Shirt Clicked");

        // Is the skin owned?
        if (SaveManager.Instance.IsSkinShirtOwned(selectedSkinShirtIndex))
        {
            // set the skin
            SetSkinShirt(selectedSkinShirtIndex);
        }
        else
        {
            // attempt to buy the skin
            if(SaveManager.Instance.BuySkinShirt(selectedSkinShirtIndex, skinShirtCost[selectedSkinShirtIndex])) //true / false return
            {
                //success
                SetSkinShirt(selectedSkinShirtIndex);

                //change the colour of the button if bought in realtime
                skinShirtPanel.GetChild(selectedSkinShirtIndex).GetComponent<Image>().color = Manager.Instance.playerTshirtColorOptions[selectedSkinShirtIndex];

                //Update token text with new value
                UpdateTokenText();
            }
            else
            {
                //not enought tokens
                // Play sound
                Debug.Log("Not Enought Tokens");
            }
        }
    }

    public void OnSkinBoardBuySet()
    {
        Debug.Log("Buy/set Skin Board Clicked");

        // Is the skin owned?
        if (SaveManager.Instance.IsSkinBoardOwned(selectedSkinBoardIndex))
        {
            // set the skin
            SetSkinBoard(selectedSkinBoardIndex);
        }
        else
        {
            // attempt to buy the skin
            if(SaveManager.Instance.BuySkinBoard(selectedSkinBoardIndex, skinBoardCost[selectedSkinBoardIndex])) //true / false return
            {
                //success
                SetSkinBoard(selectedSkinBoardIndex);

                //change the colour of the button if bought in realtime
                skinBoardPanel.GetChild(selectedSkinBoardIndex).GetComponent<Image>().color = Manager.Instance.playerBoardColorOptions[selectedSkinBoardIndex];

                //Update token text with new value
                UpdateTokenText();
            }
            else
            {
                //not enought tokens
                // Play sound
                Debug.Log("Not Enought Tokens");
            }
        }
    }

    public void OnRouteBuySet()
    {
        Debug.Log("Buy/set Course Clicked");

         // Is the route owned?
        if (SaveManager.Instance.IsRouteOwned(selectedRouteIndex))
        {
            // set the route
            SetRoute(selectedRouteIndex);
        }
        else
        {
            // attempt to buy the skin
            if(SaveManager.Instance.BuyRoute(selectedRouteIndex, routeCost[selectedRouteIndex])) //true / false return
            {
                //success
                SetRoute(selectedRouteIndex);

                //change the colour of the button if bought in realtime
                routePanel.GetChild(selectedRouteIndex).GetComponent<Image>().color = Color.white;

                //Update token text with new value
                UpdateTokenText();
            }
            else
            {
                //not enought tokens
                // Play sound
                Debug.Log("Not Enought Tokens");
            }
        }
    }

    //----------------------------------Level Selection----------------------
    private void OnLevelSelect(int currentIndex)
    {
        Manager.Instance.currentLevel = currentIndex;
        isEnteringLevel = true;
        Debug.Log("Selecting Level Button: " + currentIndex);
    }

    private void debugMaterials()
    {
        int i = 0;
        //Debug Materials
        foreach (Material material in signMaterials) 
        {
            Debug.Log("Index: " + i + "Material: " + material);
        }
    }
    //------------------------------------IEnumerators-------------------------
    
    //load preloader if run from menu
   private IEnumerator WaitForSceneLoad()
    {
        Debug.Log("IE running");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Preloader");

        while (!asyncLoad.isDone)
        {
            yield return null;//new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        Debug.Log("2: menuFocus (via Manager): " + Manager.Instance.menuFocus);
        }

    private IEnumerator SetShirtLoadScrollPosition()
    {
        
        yield return null;
        /*
        //did not snap into position accurately
        yield return null;
        scrollRectShirt.horizontalNormalizedPosition =  (float)SaveManager.Instance.state.activeSkinTShirt / 9f;
        Debug.Log("SM Shirt Selected: " + SaveManager.Instance.state.activeSkinTShirt);
        Debug.Log("Shirt Scroll Pos: " + (float)SaveManager.Instance.state.activeSkinTShirt / 9f);
        */
        Transform moveToShirtTransform = skinShirtPanel.GetChild(SaveManager.Instance.state.activeSkinTShirt).GetComponent<RectTransform>();
        float move = centreShopShirt.position.x - moveToShirtTransform.position.x;//distance to this game object
        //Debug.Log("Have to Move: " + move);
        newPosShirt = new Vector3(shirtContentTransform.position.x + move, shirtContentTransform.position.y, shirtContentTransform.position.z);
        selectMoveShirt = true;
    }

    private IEnumerator SetBoardLoadScrollPosition()
    {
        yield return null;
        /*
        //did not snap into position accurately
        yield return null;
        scrollRectBoard.horizontalNormalizedPosition =  (float)SaveManager.Instance.state.activeSkinBoard / 9f;
        Debug.Log("SM Board Selected: " + SaveManager.Instance.state.activeSkinBoard);
        Debug.Log("Board Scroll Pos: " + (float)SaveManager.Instance.state.activeSkinBoard / 9f);
        */
        Transform moveToBoardTransform = skinBoardPanel.GetChild(SaveManager.Instance.state.activeSkinBoard).GetComponent<RectTransform>();
        float move = centreShopBoard.position.x - moveToBoardTransform.position.x;//distance to this game object
        //Debug.Log("Have to Move: " + move);
        newPosBoard = new Vector3(boardContentTransform.position.x + move, boardContentTransform.position.y, boardContentTransform.position.z);
        selectMoveBoard = true;
        
    }

}
