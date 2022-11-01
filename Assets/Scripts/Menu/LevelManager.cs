using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { set; get; } //property so set / get

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public int currentLevel = 0; // know which level the player clicked on
    public int menuFocus = 0; // return from game menu focus

}
