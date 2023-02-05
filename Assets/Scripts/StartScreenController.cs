using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenController : MonoBehaviour
{
    public GameObject credits;

    public void StartGame()
    {
        Debug.Log("Start Game");
        gameObject.SetActive(false);
        GameManager.I.audioMgr.keyboardType.Play();
        GameManager.I.StartGame();
    }

    public void ShowCredits()
    {
        Debug.Log("Show Credits");
        GameManager.I.audioMgr.keyboardType.Play();
        credits.SetActive(true);
    }
}
