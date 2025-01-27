using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlutterUnityIntegration;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.Android;
using CrazyMinnow.SALSA;
using Newtonsoft.Json.Linq;
using System.IO;
using uLipSync;
using ChatdollKit.Extension.uLipSync;
using uLipSync.Timeline;

public class CharacterCtrl : MonoBehaviour
{

    [SerializeField]
    private GameObject targetObject;
    private AudioSource audioSource;

    private AudioClip audioClip;
    private Salsa salsa;

    [SerializeField] public uLipSyncWebGL lipSync;

    private LipSyncManager lipSyncManager;


    private void Awake()
    {
        // Assign targetObject via serialized field or inspector
        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned. Please assign it in the inspector.");
        }

        // Cache components or add if missing
        audioSource = GetComponent<AudioSource>() ?? targetObject.GetComponent<AudioSource>();
        salsa = GetComponent<Salsa>() ?? targetObject.GetComponent<Salsa>();
        lipSyncManager = GetComponent<LipSyncManager>() ?? targetObject.GetComponent<LipSyncManager>();
    }
    void Start()
    {
        if (salsa == null)
        {
            Debug.LogError("Salsa component not found!");
            salsa = GetComponent<Salsa>();
        }

        if (lipSyncManager == null)
        {
            Debug.LogError("LipSyncManager component not found!");
            lipSyncManager = GetComponent<LipSyncManager>();
        }
        UnityMessageManager.Instance.OnMessage += OnFlutterMessage;



        RequestPermissions();
    }


    private void RequestPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Debug.Log("Permission requested.");
        }
        else
        {
            Debug.Log("Permission already granted.");
        }
    }

    void OnFlutterMessage(string message)
    {
        Debug.Log("Received message from Flutter: " + message);
        if (targetObject != null)
        {
            EmotionManager emotionManager = targetObject.GetComponent<EmotionManager>();
            emotionManager?.SetEmotion(message);
        }
        else
        {
            Debug.LogWarning("Target GameObject not found!");
        }
    }

    [System.Obsolete]
    void OnVueMessage(string msg)
    {
        Debug.Log("Received message from Vue: " + msg);
        if (targetObject != null)
        {
            EmotionManager emotionManager = targetObject.GetComponent<EmotionManager>();
            emotionManager?.SetEmotion(msg);
        }
        else
        {
            Debug.LogWarning("Target GameObject not found!");
        }
    }



    public void ReceiveAudioFilePath(string filePath)
    {
        if (IsValidFilePath(filePath))
        {
            StartCoroutine(PlayAudioFromFile(filePath));
        }
        else
        {
            Debug.LogError("Invalid audio file path.");
        }
    }

    private bool IsValidFilePath(string path)
    {
        return File.Exists(path);
    }

    private IEnumerator PlayAudioFromFile(string filePath)
    {
        Debug.Log($"Attempting to load audio from path: {filePath}");
        string uriPath = "file://" + filePath;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uriPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error loading audio clip: {www.error}");
                yield break;
            }

            AudioClip audioClipFromWebRequest = DownloadHandlerAudioClip.GetContent(www);
            if (audioClipFromWebRequest == null)
            {
                Debug.LogError("Failed to load audio clip from web request.");
                yield break;
            }

            Debug.Log($"Audio clip loaded from web request. Clip name: {audioClipFromWebRequest.name}, Length: {audioClipFromWebRequest.length} seconds, Channels: {audioClipFromWebRequest.channels}, Frequency: {audioClipFromWebRequest.frequency}");

            audioClip = audioClipFromWebRequest;

            if (audioClip.loadState != AudioDataLoadState.Loaded)
            {
                Debug.LogError($"Audio clip failed to load: {audioClip.loadState}");
                yield break;
            }

            Debug.Log($"Audio clip loaded successfully. Clip name: {audioClip.name}, Length: {audioClip.length} seconds, Channels: {audioClip.channels}, Frequency: {audioClip.frequency}");
            PlayAudio(audioClip);
        }
    }


    private IEnumerator DownloadAndPlayAudio(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = audioClip;
                audioSource.Play();
                

              
               

            // Pass the AudioClip to uLipSyncWebGL for lip-sync processing
            // Replace with the correct method or functionality if available
            // lipSync.ProcessAudio(audioClip); // This method does not exist in uLipSyncWebGL
            }
        }
    }

    private void PlayAudio(AudioClip audioClip)
    {
        audioSource.clip = audioClip;

        salsa.audioSrc = audioSource;
        salsa.audioSrc.clip = audioClip;

        salsa.audioSrc.loop = false;
        salsa.audioSrc.Play();

        Debug.Log("SALSA AudioSource Clip: " + salsa.audioSrc.clip?.length);
        Debug.Log("Audio playback started.");
    }


    // Method to handle messages from Flutter
    public void OnTTS(string message)
    {
        var jsonData = JObject.Parse(message);
        Debug.Log($"JsonData from flutter : {jsonData}");
        string method = jsonData["fun"].ToString();

        string text = jsonData["text"].ToString();
        float duration = jsonData["duration"].ToObject<float>();

        Debug.Log($"method : {method}, Received text: {text}, duration: {duration}");

        if (method == "StartLipSync")
        {

            // Trigger StartLipSync in CharacterCtrl
            lipSyncManager.StartTalking(text, duration);
        }
        else if (method == "StopLipSync")
        {
            // Trigger StopLipSync in CharacterCtrl
            lipSyncManager.StopTalking();
        }
    }
}