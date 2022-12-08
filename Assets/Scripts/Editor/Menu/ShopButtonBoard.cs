using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonBoard : MonoBehaviour
{
    public Transform centreShopBoard;
    public Transform shopContentBoard;

    public void onClickItem()
    {
        float move = centreShopBoard.position.x - transform.position.x;//distance to this game object
        //Debug.Log("Have to Move: " + move);
        MenuScene.newPosBoard = new Vector3(shopContentBoard.position.x + move, shopContentBoard.position.y, shopContentBoard.position.z);
        MenuScene.selectMoveBoard = true;
    }

}
