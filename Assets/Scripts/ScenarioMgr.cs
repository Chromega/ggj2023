using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScenarioMgr : MonoBehaviour
{
   public List<Scenario> scenarios;
   int currentScenarioIdx;

   public UnityEvent<Scenario> OnScenarioChanged;

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
}
