using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScenarioMgr : MonoBehaviour
{
   public List<Scenario> scenarios;
   int currentScenarioIdx;

   public UnityEvent<Scenario> OnScenarioChanged;
   public UnityEvent<int> OnWordAccepted;
   public UnityEvent OnWordRejected;


   public static ScenarioMgr I { get; private set; }

   private void Awake()
   {
      // Enable credits
      I = this;
      currentScenarioIdx = Random.Range(0, scenarios.Count);
      /*
      ActivateCurrentScenario();
      */
   }

   public void StartScenarios()
   {
      ActivateCurrentScenario();
   }

   private void OnDestroy()
   {
      if (I == this)
         I = null;
   }

   void ActivateCurrentScenario()
   {
      for (int i = 0; i < scenarios.Count; ++i)
      {
         scenarios[i].gameObject.SetActive(i == currentScenarioIdx);
      }
      GetCurrentScenario().Reset();
      OnScenarioChanged.Invoke(GetCurrentScenario());
   }

   public Scenario GetCurrentScenario()
   {
      return scenarios[currentScenarioIdx];
   }

   public void NextScenario()
   {

      ++currentScenarioIdx;
      currentScenarioIdx %= scenarios.Count;
      ActivateCurrentScenario();
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.RightArrow))
         NextScenario();
   }

   public void BroadcastWordAccepted(int idx)
   {
      OnWordAccepted.Invoke(idx);
   }
   
   public void BroadcastWordRejected()
   {
      OnWordRejected.Invoke();
   }
}
