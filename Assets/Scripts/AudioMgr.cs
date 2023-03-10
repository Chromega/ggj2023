using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMgr : MonoBehaviour
{
   public AudioSource mid;
   public AudioSource percussion;
   public AudioSource upper;
   public AudioSource intro;
   public AudioSource outro;
   public AudioSource keyboardType;
   public AudioSource[] wordAccepted;
   public AudioSource wordRejected;

   // Start is called before the first frame update
   void Start()
   {
      ScenarioMgr.I.OnScenarioChanged.AddListener(ScenarioChanged);
      ScenarioMgr.I.OnWordAccepted.AddListener(WordAccepted);
      ScenarioMgr.I.OnWordRejected.AddListener(WordRejected);
      //ScenarioChanged(ScenarioMgr.I.GetCurrentScenario());
   }

   void ScenarioChanged(Scenario s)
   {
      if (s == null)
         return;

      float time = mid.time;

      mid.clip = s.sfxMid;
      percussion.clip = s.sfxPercussion;
      upper.clip = s.sfxUpper;
      mid.time = time;
      percussion.time = time;
      upper.time = time;
      mid.Play();
      percussion.Play();
      upper.Play();

      intro.clip = s.sfxIntro;
      intro.Play();
   }

   void WordAccepted(int idx)
   {
      wordAccepted[idx].Play();
   }

   void WordRejected()
   {
      wordRejected.Play();
   }
}
