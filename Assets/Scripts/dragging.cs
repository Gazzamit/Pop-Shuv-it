using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragging : MonoBehaviour
{
    private float dist;
    public bool isDragging = false;
    private Vector3 offset;
    private Transform toDrag;

    public static float hitPositionZ;

    //beatScroller beatScrollerScript;
    //public float beatTempo = 0;

    public LayerMask arrows;

    public static string touchSide;
    
    // Update is called once per frame
    void Update()
    {
        if (!gameManager.instance.showPauseOptions) // Prevent Drag on Pause Screen. requires preloader
        {
            Vector3 v3;
            if (Input.touchCount != 1)
            {
                isDragging = false;
            
                return;
            }

            Touch touch = Input.touches[0];
            Vector3 pos = touch.position;



            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos);
                RaycastHit hit;
                //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, duration: 50f);

                // dragging has begun
                if (Physics.Raycast(ray, out hit, 999f, arrows))

                //Debug.Log("Collider Hit: " + hit.collider.name);
                {

                    // Add New Tricks / Arrow Tags here with || 
                    if (hit.collider.tag == "Left Arrow" || hit.collider.tag == "Right Arrow" || hit.collider.tag == "Up Arrow" || hit.collider.tag == "Down Arrow")
                    {
                        Rigidbody arrowRigidbody;
                        arrowRigidbody = hit.rigidbody;

                        arrowRigidbody.isKinematic = false;

                        Debug.Log("Arrow Collider Hit: " + hit.collider.name);

                        hitPositionZ = hit.collider.transform.position.z;
                        //Debug.Log("Collider Position: " +  hit.collider.transform.position);
                        if (hit.collider.transform.position.x < 0f)
                        {
                            touchSide = "Left"; 
                        }
                        else
                        {
                            touchSide = "Right";
                        }
            
                        toDrag = hit.transform;
                        dist = hit.transform.position.z - Camera.main.transform.position.z;
                        //v3 = new Vector3(pos.x, (beatTempo * Time.deltaTime) + pos.y, dist);
                        v3 = new Vector3(pos.x, pos.y, dist);
                        v3 = Camera.main.ScreenToWorldPoint(v3);
                        offset = toDrag.position - v3;
                        isDragging = true;

                    }
                }

            }

            if (isDragging && touch.phase == TouchPhase.Moved)
            {

                v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
                v3 = Camera.main.ScreenToWorldPoint(v3);
                toDrag.position = v3 + offset;
            }

            if (isDragging && touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                
                isDragging = false;
            }
        }
    }
}
