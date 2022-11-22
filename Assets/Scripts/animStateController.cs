using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animStateController : MonoBehaviour
{

    public bool startPlayAnim, kickflipAnim, ollieAnim, indyAnim, anim5050, wobbleAnim, leaningLeftAnim, leaningRightAnim;
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
        grind5050();
        wobble();
        leaningLeft();
        leaningRight();



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


    public void grind5050()
    {
        if (anim5050 == true)
        {
            animator.SetBool("5050?", true);

        }

        if (anim5050 == !true)
        {
            animator.SetBool("5050?", false);

        }



    }

    public void anim5050End()
    {
        anim5050 = false;

    }


    public void wobble()
    {
        if (wobbleAnim == true)
        {
            animator.SetBool("Wobble?", true);

        }

        if (wobbleAnim == !true)
        {
            animator.SetBool("Wobble?", false);

        }


    }

    public void wobbleEnd()
    {
        wobbleAnim = false;

    }



    public void leaningLeft()
    {
        if (leaningLeftAnim == true)
        {

            animator.SetBool("LeaningLeft?", true);



        }

        if (leaningLeftAnim == !true)
        {

            animator.SetBool("LeaningLeft?", false);


        }

    }

    public void leaningLeftEnd()
    {
        leaningLeftAnim = false;

    }



    public void leaningRight()
    {
        if (leaningRightAnim == true)
        {

            animator.SetBool("LeaningRight?", true);



        }

        if (leaningRightAnim == !true)
        {

            animator.SetBool("LeaningRight?", false);


        }

    }

    public void leaningRightEnd()
    {
        leaningRightAnim = false;

    }





}
