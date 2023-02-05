using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;
   [SerializeField] TextMeshProUGUI textBox;
   [SerializeField] TextMeshProUGUI printBox;
   [SerializeField] TextMeshProUGUI errorLabel;
   [SerializeField] TextMeshProUGUI hintLabel;
   public DownloadableImage[] downloadableImages;
   public UnityEvent<string> OnWordSubmitted;
   public VoiceController voiceController;
   public float timeInSecondsBeforeHint = 3.0f;
   private float timeSinceLastCorrectAction = 0.0f;
   public AudioMgr audioMgr;
   public ScenarioMgr scenarioMgr;


   Coroutine errorFadeCoroutine;

   public static GameManager I { get; private set; }

   private void Awake()
   {
      I = this;
   }

   private void OnDestroy()
   {
      if (I == this)
         I = null;
   }

   private void Start()
   {
      Instance = this;
      printBox.text = "";
      textBox.text = "";
      hintLabel.text = "";
      errorLabel.enabled = false;
   }

   public void StartGame() {
    scenarioMgr.StartScenarios();
   }


   string keyToHint = ""; // used to stop flashing previous key

   private void StopHinting() {
        GameObject[] nextKeys = GameObject.FindGameObjectsWithTag(keyToHint.ToLower());
        foreach (GameObject key in nextKeys)
        {
            key.transform.Find("HintBackground").gameObject.SetActive(false);
        }
   }

   private void Update()
   {
        timeSinceLastCorrectAction += Time.deltaTime;
        if (timeSinceLastCorrectAction >= timeInSecondsBeforeHint) {
            FlashKeyBoardHint();
        }
   }

   private void FlashKeyBoardHint() {
    // Execute hinting
     string nextKeyTag = "";
     if (hintLabel.text == "") {
        nextKeyTag = "";
     } else if (textBox.text == "") {
        nextKeyTag = hintLabel.text.Substring(0, 1);
     } else if (textBox.text == hintLabel.text) {
        nextKeyTag = "enter";
     } else if (textBox.text.Length > hintLabel.text.Length) {
        nextKeyTag = "delete";
     } else {
        if (inputMatchesHint() == false) {
            nextKeyTag = "delete";
        }
        if (nextKeyTag == "") {
            nextKeyTag = hintLabel.text[textBox.text.Length].ToString();
        }
     }

     // if the new key is DIFFERENT from the previous key being hinted, stop flashing that key
     if (keyToHint != "" && nextKeyTag != keyToHint) {
        StopHinting();
     }

     // start to highlight the new key
     keyToHint = nextKeyTag;
     if (nextKeyTag != "") {
        GameObject[] nextKeys = GameObject.FindGameObjectsWithTag(nextKeyTag.ToLower());
        foreach (GameObject key in nextKeys)
        {
            GameObject hintBg = key.transform.Find("HintBackground").gameObject;
            hintBg.SetActive(true);
            Image hintImage = hintBg.GetComponent<Image>();
            hintImage.color = new Color(hintImage.color.r, hintImage.color.g, hintImage.color.b, Mathf.Sin(Time.time * Mathf.PI * 2.0f));
        }
     }
   }

   bool inputMatchesHint() {
        if (hintLabel.text == "") {
            // no hint, by default correct!
            return true;
        }
        if (textBox.text.Length > hintLabel.text.Length) {
            return false;
        }
        for (int i = 0; i < textBox.text.Length; i++)
        {
            if (textBox.text[i] != hintLabel.text[i]) {
                return false;
            }
        }
        return true;
   }

   public void DeleteLetter()
   {
      if (textBox.text.Length != 0)
      {
         textBox.text = textBox.text.Remove(textBox.text.Length - 1, 1);
      }
      if (inputMatchesHint()) {
        timeSinceLastCorrectAction = 0.0f;
      }
   }

   public void AddLetter(string letter)
   {
      textBox.text = textBox.text + letter;

      string speechText = textBox.text.ToLower();
      voiceController.StartSpeaking(speechText);

      if (inputMatchesHint()) {
        timeSinceLastCorrectAction = 0.0f;
      }
   }

   public void onMicDown() {
       voiceController.StartListening();
   }

    public void onMicUp() {
        voiceController.StopListening();
    }

   public void SubmitWord()
   {
      printBox.text = textBox.text;

      // clear out the final hint
      if (textBox.text == hintLabel.text) {
        hintLabel.text = "";
      }

      textBox.text = "";

      // Debug.Log("Text submitted successfully!");
      //StartCoroutine(GetImages(printBox.text));
      OnWordSubmitted.Invoke(printBox.text);

      timeSinceLastCorrectAction = 0.0f;
   }

   public void ShowHintWord(string text) {
        hintLabel.text = text;
   }


   IEnumerator GetImages(string word)
   {
      DataContainer<List<string>> urls = new DataContainer<List<string>>();
      yield return NounProjectMgr.GetImageUrls(word, urls, downloadableImages.Length);
      for (int i = 0; i < downloadableImages.Length; ++i)
      {
         if (i >= urls.data.Count)
            downloadableImages[i].Clear();
         else
            downloadableImages[i].SetURL(urls.data[i]);
      }
   }

   public void WordNotFound(string word)
   {
      errorLabel.text = "COULDN'T FIND " + word.ToUpper();
      if (errorFadeCoroutine != null)
         StopCoroutine(errorFadeCoroutine);
      errorFadeCoroutine = StartCoroutine(WordNotFoundFade());
   }

   IEnumerator WordNotFoundFade()
   {
      errorLabel.enabled = true;
      errorLabel.alpha = 1;

      yield return new WaitForSeconds(1.0f);

      float t = 0;
      const float kTotalTime = 1f;
      while (t < kTotalTime)
      {
         errorLabel.alpha = 1 - t / kTotalTime;
         yield return null;
         t += Time.deltaTime;
      }
      errorLabel.enabled = false;
   }
}
