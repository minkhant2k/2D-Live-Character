using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSyncTester : MonoBehaviour
{
    public LipSyncManager lipSyncManager;

    void Start()
    {
        TextAsset testText = Resources.Load<TextAsset>("TestText");
        if (testText != null)
        {
            var testData = JsonUtility.FromJson<TestData>(testText.text);
            lipSyncManager.StartLipSync(testData.phonemes);
        }
    }

    [System.Serializable]
    public class TestData
    {
        public string text;
        public string[] phonemes;
    }
}
