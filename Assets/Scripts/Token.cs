using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    private void OnTriggerEnter (Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player / Token Collision");
            SaveManager.Instance.state.token++;
            SaveManager.Instance.Save();
            Destroy(gameObject);
        }
    }
    
}
