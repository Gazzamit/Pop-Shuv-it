using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rightArrowTrigger : MonoBehaviour
{
    public Rigidbody playerRb;
    public animStateController animStateControl;


    //floats for calc of hit accuracy
    public float earlyHitPos = 1.9f;
    public float centreHitPos = -0.63f;
    public float lateHitPos = -2.0f;
    private float earlyHitRange, lateHitRange = 0f, hitPosZ, perPercent, goodPercent;
 
    public GameObject normalEffect, goodEffect, perfectEffect, missEffect; 
    public Vector3 effectsOffset; // Normal / Good / Perfect text

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
            if(dragging.touchSide == "Left")
            {
                effectsOffset = new Vector3 (-4.62f,2.43f,-5.43f); // Left offset for perfect / good / normal text
            }
            else
            {
                effectsOffset = new Vector3 (4.62f,2.43f,-5.43f); // Right offset for perfect / good / normal text
            }

            earlyHitRange = earlyHitPos - centreHitPos;
            lateHitRange = lateHitPos - centreHitPos;
            perPercent = gameManager.perfectPercent;
            goodPercent = gameManager.goodPercent;
            hitPosZ = dragging.hitPositionZ;

            //Debug.Log("Early Perfect hit < " + (centreHitPos + (earlyHitRange * perPercent / 100)));
            //Debug.Log("Late Perfect hit > " + (centreHitPos + (lateHitRange * perPercent / 100)));
            //Debug.Log("Early Good hit < " + (centreHitPos + (earlyHitRange * goodPercent / 100)));
            //Debug.Log("Late Good hit > " + (centreHitPos + (lateHitRange * goodPercent / 100)));

            //Debug.Log("HitPosX: " + hitPosZ);
            
            if (hitPosZ < (centreHitPos + (earlyHitRange * perPercent / 100)) &&
                hitPosZ > (centreHitPos + (lateHitRange * perPercent / 100)))
             {
                gameManager.instance.PerfectHit();
                Instantiate(perfectEffect, effectsOffset, Quaternion.identity);
            }
            else
            if (hitPosZ < (centreHitPos + (earlyHitRange * goodPercent / 100)) &&
                hitPosZ > (centreHitPos + (lateHitRange * goodPercent / 100)))
            {
                gameManager.instance.GoodHit();
                Instantiate(goodEffect, effectsOffset, Quaternion.identity);
            }
            else
            {
                gameManager.instance.NormalHit();
                Instantiate(normalEffect, effectsOffset, Quaternion.identity);
            }




            //playerRb.transform.Translate(3.0f, 0f, 0f);
            //Indy animation here
            animStateControl.indyAnim = true;

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
           // playerRb.transform.Translate(3.0f, 0f, 0f);
            animStateControl.wobbleAnim = true;

            //Have a missed arrow and end combo trigger here
            gameObject.SetActive(false);
        }
    }


}
