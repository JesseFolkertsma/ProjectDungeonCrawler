using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputFieldCheck : MonoBehaviour {
    public Button targetButton;
    InputField inputField;

    public void Awake() {
        inputField = transform.GetComponent<InputField>();
    }
    public void FieldNavigate() {
        if(inputField.text != "") {
            targetButton.onClick.Invoke();
        }
        else {
        }
    }
    public void FieldEndEdit() {
        if(!string.IsNullOrEmpty(inputField.text) && Input.GetKey(KeyCode.Return)) {
            targetButton.onClick.Invoke();
        }
    }
    public void ButtonStatus() {
        if(inputField.text != "") {
            targetButton.interactable = true;

        }
        else {
            targetButton.interactable = false;
        }
    }
}
