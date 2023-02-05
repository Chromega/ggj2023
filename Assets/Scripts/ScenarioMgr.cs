using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioMgr : MonoBehaviour
{
   public List<Scenario> scenarios;
   int currentScenarioIdx;

   public static ScenarioMgr I { get; private set; }

   private void Awake()
   {
      I = this;
      currentScenarioIdx = Random.Range(0, scenarios.Count);
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
   }
   
   Scenario GetCurrentScenario()
   {
      return scenarios[currentScenarioIdx];
   }

   public void NextScenario()
   {
      GetCurrentScenario().gameObject.SetActive(false);
      ++currentScenarioIdx;
      currentScenarioIdx %= scenarios.Count;
      GetCurrentScenario().gameObject.SetActive(true);
   }
}
