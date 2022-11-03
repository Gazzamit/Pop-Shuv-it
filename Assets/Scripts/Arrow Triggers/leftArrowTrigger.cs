using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class leftArrowTrigger : MonoBehaviour
{

    public Rigidbody playerRb;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Arrow Trigger"))
        {
            gameObject.tag = "Left Arrow";

        }

        if (collision.gameObject.CompareTag("Left Trigger"))
        {
            // move right anim here
            playerRb.transform.Translate(-3.0f, 0.0f, 0.0f);
            // put in a point scoring animation and destroy object here
            gameManager.instance.NoteHit();
            gameObject.SetActive(false);
            
        }

        if (collision.gameObject.CompareTag("Right Trigger") || collision.gameObject.CompareTag("Up Trigger") || collision.gameObject.CompareTag("Down Trigger"))
        {
            //put in a destroy animation and end combo trigger here
            gameManager.instance.WrongDirection();
            gameObject.SetActive(false);
           
        }

        if (collision.gameObject.CompareTag("Missed Arrow Trigger"))
        {
            //Have a missed arrow and end combo trigger here
            playerRb.transform.Translate(-3.0f, 0.0f, 0.0f);
            gameManager.instance.NoteMissed();
            gameObject.SetActive(false);
           
        }
    }


}
