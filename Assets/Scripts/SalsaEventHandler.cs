using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using CrazyMinnow.SALSA; // Ensure you include the SALSA namespace

public class SalsaEventHandler : MonoBehaviour
{
    // Reference to your Cubism Model
    public CubismModel cubismModel;
    private Salsa salsa;

    public AudioSource audioSource;

    // Parameter names for mouth open/close
    private const string MouthOpenParam = "PARAM_MOUTH_OPEN_Y";
    private const string MouthFormParam = "PARAM_MOUTH_FORM";

    void Start()
    {
        // Ensure Cubism model is assigned
        if (cubismModel == null)
        {
            Debug.LogError("Cubism model not found! Please assign it.");
            cubismModel = GetComponent<CubismModel>();
        }
        else
        {
            Debug.Log("Cubism model is already defined.");
        }
        // Get the Salsa component attached to this GameObject
        salsa = GetComponent<Salsa>();
        

        if (salsa == null)
        {
            Debug.LogError("Salsa component not found! Please add it to this GameObject.");
            return;
        }
        else
        {
            Debug.Log("Salsa component is already attached");
        }

        // Subscribe to SALSA events for viseme changes
        // salsa.OnViseme += HandleViseme; // Subscribe to viseme event

        // Subscribe to SALSA events
        Salsa.StartedSalsaing += OnStartedSalsaing;
        Salsa.StoppedSalsaing += OnStoppedSalsaing;
    }

    public void PlayAudio()
    {
        audioSource.Play(); // Play the audio
    }
    private void OnStartedSalsaing(object sender, Salsa.SalsaNotificationArgs e)
    {
        Debug.Log("SALSA started lip syncing.");
        // Trigger any animations or actions here
    }

    private void OnStoppedSalsaing(object sender, Salsa.SalsaNotificationArgs e)
    {
        Debug.Log("SALSA stopped lip syncing.");
        // Trigger any animations or actions here
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        Salsa.StartedSalsaing -= OnStartedSalsaing;
        Salsa.StoppedSalsaing -= OnStoppedSalsaing;
    }
    // void OnDestroy()
    // {
    //     // Unsubscribe from events when this object is destroyed
    //     if (salsa != null)
    //     {
    //         salsa.OnViseme -= HandleViseme; // Unsubscribe from viseme event
    //     }
    // }

    // void Update()
    // {
    //     // Example of polling for current viseme
    //     string currentViseme = salsa.GetCurrentViseme(); // Replace with actual method if available
    //     float intensity = salsa.GetCurrentIntensity(); // Replace with actual method if available

    //     if (!string.IsNullOrEmpty(currentViseme))
    //     {
    //         HandleViseme(currentViseme, intensity);
    //     }
    // }

    // Handle viseme changes from SALSA
    private void HandleViseme(string visemeName, float intensity)
    {
        Debug.Log($"Viseme: {visemeName}, Intensity: {intensity}");

        switch (visemeName)
        {
            case "SaySmall":
                OnMouthOpen(intensity); // Call mouth open method based on intensity
                OnMouthForm(intensity); // Call mouth form method based on intensity
                break;
            case "SayMedium":
                OnMouthOpen(intensity);
                break;
            case "SayLarge":
                OnMouthOpen(intensity);
                break;
            // Add cases for other visemes as needed
            default:
                break;
        }
    }

    // Event for mouth open
    public void OnMouthOpen(float intensity)
    {
        Debug.Log($"Raw Intensity: {intensity}");
        float scaledIntensity = Mathf.Clamp(intensity * 2.0f, 0f, 1.0f); // Adjust scaling as needed

        Debug.Log($"Scaled Intensity: {scaledIntensity}");

        var mouthOpenParameter = cubismModel.Parameters.FindById(MouthOpenParam);
        if (mouthOpenParameter != null)
        {
            mouthOpenParameter.Value = scaledIntensity;
            Debug.Log("Mouth Open Param updated.");
        }
        else
        {
            Debug.LogError("Mouth Open Param not found! Please assign it.");
        }
    }

    // Event for mouth form (e.g., shape or smile intensity)
    public void OnMouthForm(float intensity)
    {
        var mouthFormParameter = cubismModel.Parameters.FindById(MouthFormParam);
        if (mouthFormParameter != null)
        {
            mouthFormParameter.Value = Mathf.Clamp(intensity, 0f, 1.0f);
            Debug.Log("Mouth Form Param updated.");
        }
        else
        {
            Debug.LogError("Mouth Form Param not found! Please assign it.");
        }
    }
}