using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Android;

public class VoiceController : MonoBehaviour
{
    const string LANG_CODE = "en-US";

    [SerializeField] TextMeshProUGUI uiText;

    private void Start() {
        Setup(LANG_CODE);

#if UNITY_ANDROID
        SpeechToText.Instance.onPartialResultsCallback = OnPartialSpeechResult;
#endif
        SpeechToText.Instance.onResultCallback = OnFinalSpeechResult;
        TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnSpeakStop;

        CheckPermission();
    }

    void CheckPermission()
    {
#if UNITY_ANDROID
    /*
        if (!Permission.HasAuthorizedPermission(Permission.Microphone)) {
            Permission.RequestUserPermission(Permission.Micorphone);
        }
    */
#endif
    }

    # region Text to Speech

    string ttsQueue = "";
    bool isSpeaking = false;

    public void StartSpeaking(string message)
    {
        if (!isSpeaking) {
            Debug.Log("Start speaking "+message);
            TextToSpeech.Instance.StartSpeak(message);
        } else {
            ttsQueue = message;
        }

    }

    public void StopSpeaking()
    {
        // TextToSpeech.Instance.StopSpeaking(); // no longer implemented?
    }

    void OnSpeakStart()
    {
        Debug.Log("Talking started");
        isSpeaking = true;
    }

    void OnSpeakStop()
    {
        Debug.Log("Talking stopped");
        isSpeaking = false;

        if (ttsQueue != "") {
            StartSpeaking(ttsQueue);
            ttsQueue = "";
        }
    }

    #endregion

    # region Speech to Text

    public void StartListening() {
        SpeechToText.Instance.StartRecording();
    }

    public void StopListening() {
        SpeechToText.Instance.StopRecording();
    }

    void OnFinalSpeechResult(string result) {
        uiText.text = result;
    }

    void OnPartialSpeechResult(string result)
    {
        uiText.text = result;
    }

    #endregion

    void Setup(string code)
    {
        TextToSpeech.Instance.Setting(code, 1, 1);
        SpeechToText.Instance.Setting(code);
    }
}
