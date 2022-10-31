using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightArrowTrigger : MonoBehaviour
{
    public Rigidbody playerRb;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow Trigger"))
        {
            gameObject.tag = "Right Arrow";


            // move right anim here
            
            Debug.Log(collision.gameObject.name);

        }

        if (collision.gameObject.CompareTag("Right Trigger"))
        {
            playerRb.transform.Translate(3.0f, 0f, 0f);
            gameManager.instance.NoteHit();

            // put in a point scoring animation and destroy object here
            gameObject.SetActive(false);

            

        }

        if (collision.gameObject.CompareTag("Left Trigger") || collision.gameObject.CompareTag("Up Trigger") || collision.gameObject.CompareTag("Down Trigger"))
        {
            gameManager.instance.WrongDirection();

            //put in a destroy animation and end combo trigger here
            gameObject.SetActive(false);
        }

        if(collision.gameObject.CompareTag("Missed Arrow Trigger"))
        {
            gameManager.instance.NoteMissed();

            //Have a missed arrow and end combo trigger here
            gameObject.SetActive(false);
        }
    }


}
