using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animStateController : MonoBehaviour
{

    public bool startPlayAnim, kickflipAnim, ollieAnim, indyAnim, indyRampAnim, anim5050, popShuvItAnim, wobbleAnim, wobbleAvoidRailAnim, leaningLeftAnim, leaningRightAnim;
    Animator animator;

    public AudioSource audioSource;
    public AudioClip popShuvItAudio;
    public AudioClip kickflipAudio;
    public AudioClip indyAudio;
    public AudioClip grindAudio;
    public AudioClip ollieAudio;
    public AudioClip wobbleAudio;
    public AudioSource skatingAudioSource;
    public AudioClip skatingClip;



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
        indyRamp();
        wobbleAvoidRail();
        popshuvit();

    }


    //starts the animation of the player jumping on the skateboard
    public void startPlayingAnimation()
    {
        if (startPlayAnim == true)
        {
            animator.SetBool("StartGame?", true);

            if (skatingAudioSource.isPlaying == !true)
            {
                skatingAudioSource.PlayOneShot(skatingClip);
            }
        }
    }


    //triggers kickflip animation
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

    public void kickflipClipTrigger()
    {
        audioSource.PlayOneShot(kickflipAudio);
    }




    //triggers ollie animation
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

    public void ollieClipTrigger()
    {
        if (anim5050 == !true)
        {
            audioSource.PlayOneShot(ollieAudio);
        }
    }



    //triggers indy animation
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

    public void indyClipTrigger()
    {
        audioSource.PlayOneShot(indyAudio);
    }




    //triggers indy off ramp animation
    public void indyRamp()
    {
        if (indyRampAnim == true)
        {
            animator.SetBool("IndyRamp?", true);
        }

        if (indyRampAnim == !true)
        {
            animator.SetBool("IndyRamp?", false);
        }
    }

    public void indyRampEnd()
    {
        indyRampAnim = false;
    }


    //triggers popshuvit animation
    public void popshuvit()
    {
        if (popShuvItAnim == true)
        {
            animator.SetBool("PopShuvIt?", true);
        }

        if (popShuvItAnim == !true)
        {
            animator.SetBool("PopShuvIt?", false);
        }
    }

    public void popShuvItEnd()
    {
        popShuvItAnim = false;
    }

    public void popShuvItClipTrigger()
    {
        audioSource.PlayOneShot(popShuvItAudio);
    }



    //triggers grind animation
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

    public void grindClipTrigger()
    {

        audioSource.PlayOneShot(grindAudio);

    }





    //triggers wobble animation
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

    public void wobbleClipTrigger()
    {

        audioSource.PlayOneShot(wobbleAudio, 0.4f);

    }



    //triggers a wobble animation that avoids the grind rail
    public void wobbleAvoidRail()
    {
        if (wobbleAvoidRailAnim == true)
        {
            animator.SetBool("WobbleAvoidRail?", true);
        }

        if (wobbleAvoidRailAnim == !true)
        {
            animator.SetBool("WobbleAvoidRail?", false);
        }
    }

    public void wobbleAvoidRailEnd()
    {
        wobbleAvoidRailAnim = false;
    }





    //triggers leaning left animation
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



    //triggers leaning right animation
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
