using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class downArrowTrigger : MonoBehaviour
{
    public Rigidbody playerRb;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow Trigger"))
        {
            Debug.Log(collision.gameObject.name);
            gameObject.tag = "Down Arrow";

        }

        if (collision.gameObject.CompareTag("Down Trigger"))
        {

            gameManager.instance.NoteHit();

            // put in a point scoring animation and destroy object here
            gameObject.SetActive(false);

            // grind anim here

            playerRb.transform.Rotate(0f, 90f, 0f);
            
        }

        if (collision.gameObject.CompareTag("Left Trigger") || collision.gameObject.CompareTag("Up Trigger") || collision.gameObject.CompareTag("Right Trigger"))
        {
            gameManager.instance.WrongDirection();

            //put in a destroy animation and end combo trigger here
            gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Missed Arrow Trigger"))
        {
            gameManager.instance.NoteMissed();

            //Have a missed arrow and end combo trigger here
            gameObject.SetActive(false);
        }
    }
}
