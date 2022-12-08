using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatScroller : MonoBehaviour
{

    public float beatTempo;
    public bool hasStarted;
    public bool isScrolling;



    // Start is called before the first frame update
    void Start()
    {
        beatTempo = beatTempo / 60;
    }

    // Update is called once per frame
    void Update()
    {
       if (!hasStarted)
        {

            /*if (Input.anyKeyDown)
            {
                hasStarted = true;
                isScrolling = true;
            } */

        } 

        else
        {
            transform.position -= new Vector3(0f,0f, beatTempo * Time.deltaTime);
        }
    }

}
