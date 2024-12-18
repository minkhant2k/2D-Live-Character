using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;  // Ensure Cubism SDK is installed and imported
using Live2D.Cubism.Framework;

public class LipSyncManager : MonoBehaviour
{

    private CubismParameter mouthOpen; // Parameter controlling mouth openness
    private CubismParameter mouthForm; // Parameter controlling mouth shape
    private CubismModel cubismModel;

    // Phoneme-to-Mouth Mapping
    private readonly Dictionary<string, (float open, float form)> phonemeMap = new Dictionary<string, (float, float)>()
{
    { "A", (0.9f, 0.6f) },   // Open wide
    { "E", (0.7f, 0.4f) },   // Slightly open
    { "O", (0.5f, 1.0f) },   // Rounded
    { "F", (0.3f, 0.8f) },   // Teeth showing
    { "X", (0.0f, 0.0f) },   // Closed mouth
    { "U", (0.6f, 0.9f) },   // Rounded with moderate open

    // Add missing phonemes with appropriate mappings
    { "H", (0.4f, 0.3f) },   // Slightly open mouth, breathy
    { "L", (0.5f, 0.2f) },   // Tongue behind teeth (use slight open)
    { "W", (0.6f, 1.0f) },   // Rounded lips, moderate open
    { "R", (0.5f, 0.7f) },   // Slightly rounded mouth, mid-open
    { "Y", (0.7f, 0.9f) }    // Open, with rounded shape
};


    void Start()
    {
        // Get the Cubism Model component from the GameObject
        cubismModel = GetComponent<CubismModel>();

        if (cubismModel == null)
        {
            Debug.LogError("CubismModel component not found!");
            return;
        }
        // Debug all parameters in the Cubism model
        // foreach (var param in cubismModel.Parameters)
        // {
        //     Debug.Log($"Found parameter: {param.Id}");
        // }

        // Find mouth parameters by their IDs
        mouthOpen = cubismModel.Parameters.FindById("PARAM_MOUTH_OPEN_Y");
        mouthForm = cubismModel.Parameters.FindById("PARAM_MOUTH_FORM");

        // Debug parameter assignments
        if (mouthOpen != null && mouthForm != null)
        {
            mouthOpen.Value = 0.5f;
            mouthForm.Value = 1.0f;
            Debug.Log($"MouthOpen assigned: {mouthOpen.Id}");
            Debug.Log($"MouthForm assigned: {mouthForm.Id}");

        }
        else
        {
            Debug.Log("Params not assigned");
        }

    }

    // Public method to start lip-sync with phonemes
    public void StartLipSync(string[] phonemes)
    {
        // StopAllCoroutines(); // Stop any ongoing lip-sync
        if (mouthOpen == null || mouthForm == null)
        {
            Debug.LogError("Mouth parameters are null. Cannot start lip sync.");
            return;
        }
        StartCoroutine(PlayLipSync(phonemes));
    }

    // // Coroutine to animate phonemes
    // private IEnumerator PlayLipSync(string[] phonemes)
    // {
    //     foreach (string phoneme in phonemes)
    //     {
    //         if (phonemeMap.ContainsKey(phoneme))
    //         {
    //             var (open, form) = phonemeMap[phoneme];

    //             if (mouthOpen != null && mouthForm != null)
    //             {
    //                 // Log parameter values
    //                 Debug.Log($"Updating phoneme: {phoneme} - Open: {open}, Form: {form}");
    //                 mouthOpen.Value = open;
    //                 mouthForm.Value = form;
    //             }
    //             else
    //             {
    //                 Debug.LogError("Mouth parameters are null.");
    //                 yield break;
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"Phoneme not recognized: {phoneme}. Defaulting to closed mouth.");

    //         }

    //         yield return new WaitForSeconds(0.1f); // Adjust timing for smoother lip-sync
    //     }

    //     // Reset the mouth at the end
    //     if (mouthOpen != null) mouthOpen.Value = 0f;
    //     if (mouthForm != null) mouthForm.Value = 0f;
    // }
    private IEnumerator PlayLipSync(string[] phonemes)
    {
        foreach (var phoneme in phonemes)
        {
            Debug.Log($"Processing phoneme: {phoneme}");

            if (!phonemeMap.TryGetValue(phoneme, out var values))
            {
                Debug.LogWarning($"Phoneme not recognized: {phoneme}. Defaulting to closed mouth.");
                values = phonemeMap["X"];
            }
            else
            {
                Debug.Log($"Mouth Open: {values.open}");
                Debug.Log($"Mouth Form: {values.form}");
            }

            // Smoothly transition to new values
            float duration = 0.1f; // The time it takes to transition to the new values
            float elapsedTime = 0f;

            float initialMouthOpen = mouthOpen.Value;
            float initialMouthForm = mouthForm.Value;

            while (elapsedTime < duration)
            {
                mouthOpen.Value = Mathf.Lerp(initialMouthOpen, values.open, elapsedTime / duration);
                mouthForm.Value = Mathf.Lerp(initialMouthForm, values.form, elapsedTime / duration);

                cubismModel.ForceUpdateNow(); // Update the model

                elapsedTime += Time.deltaTime; // Increment the time elapsed
                yield return null; // Wait for the next frame
            }

            mouthOpen.Value = values.open;
            mouthForm.Value = values.form;

            cubismModel.ForceUpdateNow();

            // Allow changes to propagate
            yield return new WaitForSeconds(0.2f);
            Debug.Log($"Mouth Open: {mouthOpen.Value}, Mouth Form: {mouthForm.Value}");
        }
        // Reset mouth to closed at the end
        if (mouthOpen == null) mouthOpen.Value = 0f;
        if (mouthForm == null) mouthForm.Value = 0f;
        cubismModel.ForceUpdateNow();
    }

}
