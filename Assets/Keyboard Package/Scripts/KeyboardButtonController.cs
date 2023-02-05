using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class KeyboardButtonController : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    [SerializeField] Image containerBorderImage;
    [SerializeField] Image containerFillImage;
    [SerializeField] Image containerIcon;
    [SerializeField] TextMeshProUGUI containerText;
    [SerializeField] TextMeshProUGUI containerActionText;

    //Do this when the mouse is clicked over the selectable object this script is attached to.
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(this.gameObject.name + " Was Clicked.");
        transform.Find("PressBackground").gameObject.SetActive(true);

        if (gameObject.tag == "Mic") {
            Debug.Log("mic down");
            GameManager.Instance.onMicDown();
        }

    }

    //Do this when the mouse click on this selectable UI object is released.
	public void OnPointerUp (PointerEventData eventData)
	{
		//Debug.Log ("The mouse click was released");
        transform.Find("PressBackground").gameObject.SetActive(false);

        if (gameObject.tag == "Mic") {
            Debug.Log("mic up");
            GameManager.Instance.onMicUp();
        }
	}

    //Do this when the cursor exits the rect area of this selectable UI object.
	public void OnPointerExit (PointerEventData eventData)
	{
		//Debug.Log ("The cursor exited the selectable UI element.");
	}

    private void Start() {
        SetContainerBorderColor(ColorDataStore.GetKeyboardBorderColor());
        SetContainerFillColor(ColorDataStore.GetKeyboardFillColor());
        SetContainerTextColor(ColorDataStore.GetKeyboardTextColor());
        //SetContainerActionTextColor(ColorDataStore.GetKeyboardActionTextColor());
    }

    public void SetContainerBorderColor(Color color) => containerBorderImage.color = color;
    public void SetContainerFillColor(Color color) => containerFillImage.color = color;
    public void SetContainerTextColor(Color color) {
        // containerText.color = color;
    }
    public void SetContainerActionTextColor(Color color) {
        // containerActionText.color = color;
        containerIcon.color = color;
    }

    public void AddLetter() {
        if(GameManager.Instance != null) {
            GameManager.I.audioMgr.keyboardType.Play();
            GameManager.Instance.AddLetter(containerText.text);
        } else {
            Debug.Log(containerText.text + " is pressed");
        }
    }
    public void DeleteLetter() {
        if(GameManager.Instance != null) {
            GameManager.Instance.DeleteLetter();
        } else {
            Debug.Log("Last char deleted");
        }
    }
    public void SubmitWord() {
        if(GameManager.Instance != null) {
            GameManager.Instance.SubmitWord();
        } else {
            Debug.Log("Submitted successfully!");
        }
    }
}
