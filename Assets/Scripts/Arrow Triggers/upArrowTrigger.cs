using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class upArrowTrigger : MonoBehaviour
{
    public Rigidbody playerRb;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Arrow Trigger"))
        {
            gameObject.tag = "Up Arrow";

        }

        if (collision.gameObject.CompareTag("Up Trigger"))
        {
            gameManager.instance.NoteHit();

            // jump anim here
            playerRb.transform.Translate(0f, 5f, 0f);

            // put in a point scoring animation and destroy object here
            gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Left Trigger") || collision.gameObject.CompareTag("Down Trigger") || collision.gameObject.CompareTag("Right Trigger"))
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
