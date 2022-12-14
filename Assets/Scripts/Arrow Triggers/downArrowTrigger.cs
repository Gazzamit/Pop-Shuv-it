using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class downArrowTrigger : MonoBehaviour
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
        if (collision.gameObject.CompareTag("Arrow Trigger")) // Do not change
        {
            //Debug.Log(collision.gameObject.name);
            gameObject.tag = "Down Arrow"; //This script is a component of a Arrow Prefab with similar name

        }

        if (collision.gameObject.CompareTag("Down Trigger")) //Down arrow needs down trigger for correct movement
        {
            if(dragging.touchSide == "Left") //Set to Left or Right to make the effect appear that side
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

            //Debug.Log("HitPosZ: " + hitPosZ);
            
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

            // put in a point scoring animation and destroy object here
            gameObject.SetActive(false);

            // grind anim here
            animStateControl.anim5050 = true;
            //playerRb.transform.Rotate(0f, 90f, 0f);

        }

        //This is a list of incorrect movements
        if (collision.gameObject.CompareTag("Left Trigger") || collision.gameObject.CompareTag("Up Trigger") || collision.gameObject.CompareTag("Right Trigger"))
        {
            gameManager.instance.WrongDirection();

            //put in a destroy animation and end combo trigger here
            gameObject.SetActive(false);
        }

        //This triggers after anything hits it on either side. Don not chenge.
        if (collision.gameObject.CompareTag("Missed Arrow Trigger"))
        {
            gameManager.instance.NoteMissed();
            animStateControl.wobbleAnim = true;

            //Have a missed arrow and end combo trigger here
            gameObject.SetActive(false);
        }
    }
}
