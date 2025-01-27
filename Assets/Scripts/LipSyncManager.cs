using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;  // Ensure Cubism SDK is installed and imported
using Live2D.Cubism.Framework;
using System.Linq;

public class LipSyncManager : MonoBehaviour
{

    [SerializeField] private CubismParameter PARAM_MOUTH_OPEN_Y; // Parameter controlling mouth openness
    [SerializeField] private CubismParameter PARAM_MOUTH_FORM; // Parameter controlling mouth shape
    [SerializeField] private CubismModel cubismModel;

    private Coroutine lipSyncCoroutine;


    private float minMouthOpenY = 0.0f;    // Min closed mouth height (mouth closed)
    private float neutralMouthForm = 1f; // Neutral mouth form (mouth normal)





    // Phoneme-to-Mouth Mapping
    private readonly Dictionary<string, (float open, float form)> phonemeMap = new Dictionary<string, (float, float)>()
{
    { "A", (0.85f, 0.8f) },  // Wide open (as in "father")
    { "E", (0.7f, 0.5f) }, // Slightly open (as in "bed")
    { "I", (0.6f, 0.3f) },  // Narrow, stretched horizontally (as in "machine")
    { "O", (0.8f, 0.7f) },  // Round lips (as in "go")
    { "U", (0.65f, 1.0f) }, // Rounded with moderate opening (as in "blue")
    { "M", (0.1f, 0.1f) },  // Closed mouth (as in "muff")
    { "F", (0.4f, 0.6f) },  // Upper teeth touching lip (as in "fun")
    { "S", (0.5f, 0.5f) },  // Slightly open, showing teeth (as in "see")
    { "X", (0.0f, 0.0f) },  // Default closed mouth for unrecognized sounds
    { "AA", (1.0f, 0.9f) }, // Open back unrounded vowel (as in "father")
    { "AE", (1.0f, 0.6f) }, // Open front unrounded vowel (as in "cat")
    { "AY", (1.0f, 0.7f) }, // Diphthong (as in "say")
    { "AW", (1.0f, 0.8f) }, // Diphthong with rounding (as in "how")
    { "EH", (0.85f, 0.4f) },// Open-mid front unrounded vowel (as in "bed")
    { "OW", (1.0f, 0.9f) }, // Diphthong with rounding (as in "go")
    { "OY", (1.0f, 0.8f) }, // Diphthong with rounding (as in "boy")
    { "TH", (0.5f, 0.4f) }, // Voiced dental fricative (as in "this")
    { "SH", (0.5f, 0.5f) }, // Voiceless postalveolar fricative (as in "she")
    { "CH", (0.5f, 0.6f) }, // Voiceless postalveolar affricate (as in "cheese")
    { "NG", (0.2f, 0.2f) }
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


        // Find mouth parameters by their IDs
        PARAM_MOUTH_OPEN_Y = cubismModel.Parameters.FindById("PARAM_MOUTH_OPEN_Y");
        PARAM_MOUTH_FORM = cubismModel.Parameters.FindById("PARAM_MOUTH_FORM");

        // Set the initial mouth state to closed
        if (PARAM_MOUTH_OPEN_Y != null)
        {
            PARAM_MOUTH_OPEN_Y.Value = minMouthOpenY;
            Debug.Log($"MouthOpen assigned: {PARAM_MOUTH_OPEN_Y.Id}");  // Set mouth to closed initially
        }
        if (PARAM_MOUTH_FORM != null)
        {
            PARAM_MOUTH_FORM.Value = neutralMouthForm;// Set mouth form to neutral initially
            Debug.Log($"MouthForm assigned: {PARAM_MOUTH_OPEN_Y.Id}");
        }
        // string text = "Hello, My name is min khant. I'm a flutter developer. I from Myanmar.";
        // StartTalking(text, 1.5f);


    }


    // Method to start talking animation manually
    public void StartTalking(string text, float duration)
    {

        if (lipSyncCoroutine != null)
        {
            StopCoroutine(lipSyncCoroutine); // Safely stop previous coroutine
            Debug.Log($"StartLipSync is still playing");
            lipSyncCoroutine = null;
        }
        UpdateMouthToNeutral();
        Debug.Log($"StartLipSync called with text: {text}, duration: {duration}");
        string[] phoneme = ParseTextToPhonemes(text);
        lipSyncCoroutine = StartCoroutine(LipSyncAnimation(phoneme, duration));

    }

    private string[] ParseTextToPhonemes(string text)
    {
        // Improved phoneme parsing (could be replaced with a more sophisticated method)
        return text.ToUpper().Select(c => c.ToString()).ToArray();
    }



    // Method to stop talking animation
    public void StopTalking()
    {
        Debug.Log("StopLipSync called. Stopping lip-sync.");
        if (lipSyncCoroutine != null)
        {
            StopCoroutine(lipSyncCoroutine);
            lipSyncCoroutine = null;
            Debug.Log("Stopping lip-sync.");
        }
        //    lipSyncCoroutine = StartCoroutine(ResetMouthAfterDelay());  // Small delay for smoother stop

        UpdateMouthToNeutral();
        Debug.Log("Mouth has been reset to neutral state.");

    }

    private IEnumerator ResetMouthAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);

        UpdateMouthToNeutral();
    }

    // Update the mouth parameters in Cubism model
    void UpdateMouthToNeutral()
    {
        // Set the new values for the mouth parameters in Cubism
        PARAM_MOUTH_OPEN_Y.Value = minMouthOpenY;
        PARAM_MOUTH_FORM.Value = neutralMouthForm;
        cubismModel.ForceUpdateNow();

        Debug.Log($"Mouth reset: Open Y = {minMouthOpenY}, Form = {neutralMouthForm}");
    }


    private IEnumerator LipSyncAnimation(string[] phonemes, float totalDuration)
    {

        float phonemeDuration = totalDuration / phonemes.Length;

        foreach (var phoneme in phonemes)
        {
            // Get the values for the current phoneme (open mouth and form)
            if (phonemeMap.TryGetValue(phoneme, out var values))
            {
                // Animate mouth based on the phoneme values (open and form)
                float mouthOpenY = values.open;
                float mouthForm = values.form;

                // Smoothly transition to the new mouth values
                yield return StartCoroutine(AnimateMouth(initialMouthOpen: PARAM_MOUTH_OPEN_Y.Value,
                                                          initialMouthForm: PARAM_MOUTH_FORM.Value,
                                                          targetMouthOpen: mouthOpenY,
                                                          targetMouthForm: mouthForm,
                                                          duration: phonemeDuration));
            }
            else
            {
                Debug.LogWarning($"Unrecognized phoneme: {phoneme}. Defaulting to closed mouth.");
                // Animate to closed mouth if unrecognized
                yield return StartCoroutine(AnimateMouth(initialMouthOpen: PARAM_MOUTH_OPEN_Y.Value,
                                                          initialMouthForm: PARAM_MOUTH_FORM.Value,
                                                          targetMouthOpen: 0.3f,
                                                          targetMouthForm: 0.5f,
                                                          duration: phonemeDuration));
            }

            // Wait for a brief moment before moving to the next phoneme for smoother transitions
            yield return new WaitForSeconds(0.1f); // Adjust this delay as needed
        }

        // Reset mouth to neutral after all phonemes
        yield return StartCoroutine(AnimateMouth(initialMouthOpen: PARAM_MOUTH_OPEN_Y.Value,
                                                  initialMouthForm: PARAM_MOUTH_FORM.Value,
                                                  targetMouthOpen: 0f,
                                                  targetMouthForm: 0f,
                                                  duration: 0.3f)); // Duration for resetting

        Debug.Log("LipSyncAnimation completed.");
        lipSyncCoroutine = null;
    }

    // Separate coroutine for animating mouth transitions
    private IEnumerator AnimateMouth(float initialMouthOpen, float initialMouthForm, float targetMouthOpen, float targetMouthForm, float duration)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            // Lerp between the current and new values for mouth open and form
            PARAM_MOUTH_OPEN_Y.Value = Mathf.Lerp(initialMouthOpen, targetMouthOpen, elapsedTime / duration);
            PARAM_MOUTH_FORM.Value = Mathf.Lerp(initialMouthForm, targetMouthForm, elapsedTime / duration);

            cubismModel.ForceUpdateNow();  // Update the model

            elapsedTime += Time.deltaTime * 0.155f;  // Increment elapsed time

            yield return null;  // Wait for the next frame
        }

        // Ensure final values are set
        PARAM_MOUTH_OPEN_Y.Value = targetMouthOpen;
        PARAM_MOUTH_FORM.Value = targetMouthForm;
        cubismModel.ForceUpdateNow(); // Update model immediately
    }

}
