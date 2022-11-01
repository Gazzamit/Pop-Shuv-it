using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { set; get; } //access from anywhere in the
    public SaveState state; //this will be converted to/from a string

    private void Awake()
    {
        //ResetSave();
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
    public bool IsSkinOwned(int index)
    {
        return (state.skinOwned & (1 << index)) != 0; //bit operators 0000 - return 1 if owned, 0 if not
    }

    // Route owned?
    public bool IsRouteOwned(int index)
    {
        return (state.routeOwned & (1 << index)) != 0; //bit operators 0000 - return 1 if owned, 0 if not
    }

    //attempt to buy skin / route (return true / false)
    public bool BuySkin(int index, int cost)
    {
        if (state.token >= cost)
        {
            //enough tokens
            state.token -= cost;
            UnlockSkin(index);
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
    public void UnlockSkin(int index)
    {
        //toggle on the bit at index (bit operator)
        state.skinOwned |= 1 <<index;
    }
    public void UnlockRoute(int index)
    {
        //toggle on the bit at index (bit operator)
        state.routeOwned |= 1 <<index;
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

