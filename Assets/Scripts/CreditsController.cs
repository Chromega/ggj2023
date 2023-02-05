using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public void HideCredits()
    {
        Debug.Log("Hide Credits");
        GameManager.I.audioMgr.keyboardType.Play();
        gameObject.SetActive(false);
    }
}
