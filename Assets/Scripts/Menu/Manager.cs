using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { set; get; } //property so set / get
    public int currentLevel = 0; // know which level the player clicked on
    public int menuFocus = 0; // return from game menu focus
    
    public Material[] playerMaterials;
    public Color[] playerTshirtColorOptions = new Color[10];

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

}
