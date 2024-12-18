using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhonemeTester : MonoBehaviour
{
    public LipSyncManager lipSyncManager;
    private Animator animator;

    // Example test data
    private readonly string[] testPhonemes = { "H", "E", "L", "L", "O" };

    void Start()
    {
        animator = GetComponent<Animator>();

    }

    void Update()
    {
        // Press "T" to trigger the test lip-sync
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Testing Lip Sync with phonemes: " + string.Join(", ", testPhonemes));
            // Disable the animator temporarily if it exists
            if (animator != null)
            {
                animator.enabled = false; // Temporarily disable animations
            }

            // Call the LipSyncManager's method to animate
            if (lipSyncManager != null)
            {
                lipSyncManager.StartLipSync(testPhonemes); // Start lip sync animation
            }
            else
            {
                Debug.LogError("LipSyncManager is not assigned!");
            }
        }
    }
}
