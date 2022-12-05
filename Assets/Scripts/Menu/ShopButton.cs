using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public Transform centreShop;
    public Transform shopContent;

    public void onClickItem()
    {
        float move = centreShop.position.x - transform.position.x;//distance to this game object
        //Debug.Log("Have to Move: " + move);
        MenuScene.newPos = new Vector3(shopContent.position.x + move, shopContent.position.y, shopContent.position.z);
        MenuScene.selectMove = true;
    }

    public void SelectItem(int item)
    {
        //store selected ship here??    
    }
}
