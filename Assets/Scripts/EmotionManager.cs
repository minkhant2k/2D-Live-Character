using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionManager : MonoBehaviour
{

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not assigned!");
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Method invoked by Flutter
    public void SetEmotion(string emotion)
    {
        Debug.Log("Received emotion: " + emotion);
        switch (emotion)
        {
            case "angry":
                animator.SetTrigger("madTrigger");
                break;

            case "embarassed":
                animator.SetTrigger("embarassedTrigger");
                break;

            case "agree":
                animator.SetTrigger("agreeTrigger");
                break;

            case "disgree":
                animator.SetTrigger("disagreeTrigger");
                break;

            case "surprise":
                animator.SetTrigger("surpriseTrigger");
                break;
            
            case "hey guys":
                animator.SetTrigger("winkTrigger");
                break;

            case "sad":
                animator.SetTrigger("sadTrigger");
                break;

            default:
                Debug.LogWarning("Unknown emotion: " + emotion);
                break;
        }
    }
}
