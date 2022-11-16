using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animStateController : MonoBehaviour
{

    public bool startPlayAnim, kickflipAnim, ollieAnim, indyAnim;
    Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("StartGame?", false);
       

    }

    // Update is called once per frame
    void Update()
    {
        startPlayingAnimation();
        kickflip();
        ollie();
        indy();

    }


    public void startPlayingAnimation()
    {
        if (startPlayAnim == true)
        {
            animator.SetBool("StartGame?", true);

        }


    }

    

    public void kickflip()
    {
        if (kickflipAnim == true)
        {
            
            animator.SetBool("Kickflip?", true);
            

       
        }

        if (kickflipAnim == !true)
        {

            animator.SetBool("Kickflip?", false);
          

        }

    }

    public void kickflipEnd()
    {
        kickflipAnim = false;

    }


    public void ollie()
    {
        if (ollieAnim == true)
        {

            animator.SetBool("Ollie?", true);



        }

        if (ollieAnim == !true)
        {

            animator.SetBool("Ollie?", false);


        }

    }

    public void ollieEnd()
    {
        ollieAnim = false;

    }

    public void indy()
    {
        if (indyAnim == true)
        {
            animator.SetBool("Indy?", true);

        }

        if (indyAnim == !true)
        {
            animator.SetBool("Indy?", false);

        }



    }

    public void indyEnd()
    {
        indyAnim = false;

    }


}
