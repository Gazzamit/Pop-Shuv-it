using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButtonShirt : MonoBehaviour
{
    public Transform centreShopShirt;
    public Transform shopContentShirt;

    public void onClickItem()
    {
        float move = centreShopShirt.position.x - transform.position.x;//distance to this game object
        //Debug.Log("Have to Move: " + move);
        MenuScene.newPosShirt = new Vector3(shopContentShirt.position.x + move, shopContentShirt.position.y, shopContentShirt.position.z);
        MenuScene.selectMoveShirt = true;
    }

}
