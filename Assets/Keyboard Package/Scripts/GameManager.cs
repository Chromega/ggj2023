using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

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

   public void DeleteLetter()
   {
      if (textBox.text.Length != 0)
      {
         textBox.text = textBox.text.Remove(textBox.text.Length - 1, 1);
      }
   }

   public void AddLetter(string letter)
   {
      textBox.text = textBox.text + letter;

      string speechText = textBox.text.ToLower();
      voiceController.StartSpeaking(speechText);
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
      textBox.text = "";
      // Debug.Log("Text submitted successfully!");
      //StartCoroutine(GetImages(printBox.text));
      OnWordSubmitted.Invoke(printBox.text);
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
