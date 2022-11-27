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

    public Transform skinPanel; //assign in inspector - part of Shop Panel (R)
    public Transform routePanel; //assign in inspector - Part of Shiop Panel (R)
    public Transform levelPanel; //assign in inspector - Part of Level Panel (L)

    //Text for shop
    public TextMeshProUGUI skinBuySetText;
    public TextMeshProUGUI routeBuySetText; 
    public TextMeshProUGUI tokenText;

    private MenuCamera menuCam;

    //Shop cost items
    public int[] skinCost = new int[] { 0, 5, 5, 5, 5, 10, 10, 10, 10, 10};
    public int[] routeCost = new int[] { 0, 5, 5, 5, 5, 10, 10, 10, 10, 10};
    // item selected in shop 
    private int selectedSkinIndex; //after selct
    private int selectedRouteIndex; //after select
    private int activeSkinIndex; //after set
    private int activeRouteIndex; //after set


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
    private Material[] signMaterials;

    private void Awake()
    {

    }

    private void Start()
    {   
        // $$ TEMP $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
        //SaveManager.Instance.state.token = 100; 

        //Find menu cam
        menuCam = FindObjectOfType<MenuCamera>();

        //Position Cam on the Focus Menu    
        //SetFocusTo(Manager.Instance.menuFocus);
        NavigateTo(Manager.Instance.menuFocus);

        //Update the gold text at the start
        UpdateTokenText();

        fadeGroup.alpha = 1; //fade-in

        // add button on-clock events to skin Panel buttons
        InitShop();

        // add button on-click events to levels Panel
        InitLevel();

        //set player prefs for skin and route
        OnSkinSelect(SaveManager.Instance.state.activeSkinTShirt);
        SetSkin(SaveManager.Instance.state.activeSkinTShirt);

        OnRouteSelect(SaveManager.Instance.state.activeRoute);
        SetRoute(SaveManager.Instance.state.activeRoute);

        // Make buttons bigger for the selected items above
        skinPanel.GetChild(SaveManager.Instance.state.activeSkinTShirt).GetComponent<RectTransform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
        routePanel.GetChild(SaveManager.Instance.state.activeRoute).GetComponent<RectTransform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
        
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
    }
    
    //-------------------------------Init-----------------------------
    private void InitShop()
    {
        Debug.Log("InitLevel Running");
        //add click events e.g. 0 to 10 to skin items
        int i = 0;
        foreach (Transform t in skinPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(()=>OnSkinSelect(currentIndex));

            // set colour of image is owned or not using tert operator
            Image img = t.GetComponent<Image>();
            
            img.color = Manager.Instance.playerTshirtColorOptions[currentIndex]; //player color from manager
            
            //Not using dimming - looked weird as selected colours changed when bought
            //img.color = SaveManager.Instance.IsSkinOwned(i) 
            //    ? Manager.Instance.playerTshirtColorOptions[currentIndex] //player color from manager
            //    : Color.Lerp(Manager.Instance.playerTshirtColorOptions[currentIndex], new Color(0,0,0,1),0.25f); // dim the colour

            i++;
        }

        i = 0; //reset

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
        
            //Debug Materials
            //foreach (Material material in signMaterials) 
            //{
            //    Debug.Log("Index: " + i + "Material: " + material);
            //}

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
                break;
            case 1: //level
                desiredMenuPosition = Vector3.right * referenceHorizontalResolution;
                menuCam.MoveToLevel();
                Debug.Log("NavigateTo: " + menuIndex);

                break;
            case 2: //shop
                desiredMenuPosition = Vector3.left * referenceHorizontalResolution;
                menuCam.MoveToShop();
                Debug.Log("NavigateTo: " + menuIndex);
                break;
        }
    }

    //----------------------------Shop---------------------------
    private void SetSkin (int index)
    {
        // set the active index on the model
        activeSkinIndex = index;
        SaveManager.Instance.state.activeSkinTShirt = index;  //Set prefs

        // Get the T-shirt and change skin on the model
        Manager.Instance.playerMaterials = playerBodyInMenu.GetComponent<SkinnedMeshRenderer>().materials;
        
        //Debug Materials
        //foreach (Material material in playerMaterials) 
        //{
        //    Debug.Log(material);
        //}

        //Set color of player T Shirt (3rd material)
        Manager.Instance.playerMaterials[2].color = Manager.Instance.playerTshirtColorOptions[index];

        // change buy/set button
        skinBuySetText.text = "Current";

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

    //--------------------------Shop Buttons----------------------
    private void OnSkinSelect(int currentIndex)
    {
        Debug.Log("Selecting Skin Button: " + currentIndex);

        //if button clicked is already selected, do nothing
        if (selectedSkinIndex == currentIndex)
        {
            return;
        }
        else 
        {   
            //make icon bigger
            skinPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = new Vector3(1.125f, 1.125f, 1.125f);
            //skinPanel.GetChild(currentIndex).GetComponent<Image>().color = Manager.Instance.playerTshirtColorOptions[currentIndex];

            //make last selected icon normal size
            skinPanel.GetChild(selectedSkinIndex).GetComponent<RectTransform>().localScale = Vector3.one;
            //skinPanel.GetChild(selectedSkinIndex).GetComponent<Image>().color 
            //    = Color.Lerp(skinPanel.GetChild(currentIndex).GetComponent<Image>().color, new Color(0,0,0,1),0.25f); // dim the colour
;
        }

        //set the selected skin
        selectedSkinIndex = currentIndex;

        //change content of buy/set button (selection can be owned/not owned etc)
        if (SaveManager.Instance.IsSkinOwned(currentIndex))
        {
            //skin owned
            //Already current skin?
            if (activeSkinIndex == currentIndex)
            {
                skinBuySetText.text = "Current";               
            }
            else
            {
                skinBuySetText.text = "Select";
            }

        }
        else
        {
            //skin isn't owned - get skinCost from index and display
            skinBuySetText.text = "Buy: " + skinCost[currentIndex].ToString();
        }
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

    public void OnSkinBuySet()
    {
        Debug.Log("Buy/set Skin Clicked");

        // Is the skin owned?
        if (SaveManager.Instance.IsSkinOwned(selectedSkinIndex))
        {
            // set the skin
            SetSkin(selectedSkinIndex);
        }
        else
        {
            // attempt to buy the skin
            if(SaveManager.Instance.BuySkin(selectedSkinIndex, skinCost[selectedSkinIndex])) //true / false return
            {
                //success
                SetSkin(selectedSkinIndex);

                //change the colour of the button if bought in realtime
                skinPanel.GetChild(selectedSkinIndex).GetComponent<Image>().color = Manager.Instance.playerTshirtColorOptions[selectedSkinIndex];

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

    
   IEnumerator LoadSceneAdditive(int buildIndex)
        {
            Debug.Log("Called LoadSceneAdditive. Bulid index:  " + buildIndex);
            AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
    
            while (!operation.isDone)
            {
                Debug.Log("DDOL loading...");
                yield return null;

            }
            Debug.Log("DDOL Loaded");            //Done
        }


}
