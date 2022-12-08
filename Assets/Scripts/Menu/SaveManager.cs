using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public bool RESET_SAVE = false;

    public static SaveManager Instance { set; get; } //access from anywhere in the
    public SaveState state; //this will be converted to/from a string

    private void Awake()
    {
        //TEMP for reset state

        if (RESET_SAVE)
        {
            ResetSave();
            RESET_SAVE = false;
            Debug.Log("--------------");
            Debug.Log("SAVE WAS RESET");
            Debug.Log("--------------");
        }
        else
        {
            Debug.Log ("Save not reset");
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();

        //test state
        //Debug.Log(IsSkinOwned(0));
        //UnlockSkin(0);
        //Debug.Log(IsSkinOwned(0));

        //to save/load data
        //SaveManager.Instance.Save()
        //SaveManager.Instance.state.yourVariableToSave = x; 
        //SaveManager.Instance.Load()
        //x = SaveManager.Instance.state.yourVariableToLoad

    }

    // Save state to saveState script to player prefs via serialize
    public void Save()
    {
        PlayerPrefs.SetString("save", Helper.Serialize<SaveState>(state)); // convert state into a string
    }

    //load state from saveState script from playerprefs via deserialize
    public void Load()
    {
        //have already a saved state?
        if (PlayerPrefs.HasKey("save"))
        {
            Debug.Log("Loading Player Prefs");
            state = Helper.Deserialize<SaveState>(PlayerPrefs.GetString("save")); // convert string to state
        }
        else //first save
        {
            state = new SaveState();
            Save();
            Debug.Log("First Save");
        }
    }

    // Skin owned?
    public bool IsSkinShirtOwned(int index)
    {
        return (state.skinShirtOwned & (1 << index)) != 0; //bit operators 0000 - return 1 if owned, 0 if not
    }

    //board owned?
    public bool IsSkinBoardOwned(int index)
    {
        return (state.skinBoardOwned & (1 << index)) != 0; //bit operators 0000 - return 1 if owned, 0 if not
    }

    // Route owned?
    public bool IsRouteOwned(int index)
    {
        return (state.routeOwned & (1 << index)) != 0; //bit operators 0000 - return 1 if owned, 0 if not
    }

    //attempt to buy skin / route (return true / false)
    public bool BuySkinShirt(int index, int cost)
    {
        if (state.token >= cost)
        {
            //enough tokens
            state.token -= cost;
            UnlockSkinShirt(index);
            Save(); //save progress
            return true;
        }
        else
        {
            //not enough tokens
            return false;
        }
    }

    public bool BuySkinBoard(int index, int cost)
    {
        if (state.token >= cost)
        {
            //enough tokens
            state.token -= cost;
            UnlockSkinBoard(index);
            Save(); //save progress
            return true;
        }
        else
        {
            //not enough tokens
            return false;
        }
    }

    public bool BuyRoute(int index, int cost)
    {
        if (state.token >= cost)
        {
            state.token -= cost;
            UnlockRoute(index);
            Save(); //save progress
            return true;
        }
        else
        {
            //not enough tokens
            return false;
        }
    }

    //unlock skin or route
    public void UnlockSkinShirt(int index)
    {
        //toggle on the bit at index (bit operator)
        state.skinShirtOwned |= 1 << index;
    }
    public void UnlockSkinBoard(int index)
    {
        //toggle on the bit at index (bit operator)
        state.skinBoardOwned |= 1 << index;
    }
    public void UnlockRoute(int index)
    {
        //toggle on the bit at index (bit operator)
        state.routeOwned |= 1 << index;
    }

    //Complete the level
    public void CompleteLevel(int index)
    {
        //if this level is the current active level
        //Debug.Log("SM - Save: state.completedLevel: " + state.completedLevel + " index: " + index);
        if (state.completedLevel == index)
        {
            state.completedLevel++;
            Debug.Log("Increment Level Completed and Save. Completed Level: " + state.completedLevel);
            Save();
        }
    }

    //reset save file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }
}

