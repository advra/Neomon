using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ButtonAnimate : MonoBehaviour {

    
    public float animationSpeed = 1.5f; //in seconds
    public Vector3 animateStart;
    public Vector3 animateEnd;
    
    float t;    //time variable
    Animator animateState;
    

    // Use this for initialization
    void Start () {
        animateState = GetComponent<Animator>();
        animateState.SetBool("doBirth", true);

        transform.position = animateStart;
        

    }

    // Update is called once per frame
    void Update()
    {

        if (animateState.GetBool("doBirth"))
        {
            t += Time.deltaTime / animationSpeed;
            transform.localPosition = Vector3.Lerp(animateStart, animateEnd, t);
        }
        else if (animateState.GetBool("doDeath"))
        {
            //do death
        }
        else
        {
            //do idle animation as Default

            if (animateState.GetBool("selected"))
            {
                //do select animation
            }
        }


    }

    void birthAnimation()
    {
        Debug.Log(transform.localPosition);


            t += Time.deltaTime / animationSpeed;
            transform.localPosition = Vector3.Lerp(animateStart, animateEnd, t);

            //animateState.SetBool("doBirth", false);
        
    }


}
