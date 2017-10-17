using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public Transform chatContent;
    public GameObject chatMessage;
    public InputField inputField;

    public bool chatOpen;

    public void SendMessage(string message) {
        if (message != "") {
            if (!chatOpen) {
                chatContent.parent.GetComponent<Animator>().SetTrigger("ChatNew");
            }
            GameObject newMessage = Instantiate(chatMessage);
            newMessage.transform.SetParent(chatContent, false);
            newMessage.GetComponent<Text>().text = message;
        }
    }
    public void Update() {
        if (Input.GetKeyDown(KeyCode.T) && !inputField.isFocused){
            ToggleChat();
        }
        if (Input.GetKeyDown(KeyCode.N)){
            SendMessage("Hary Likes bread");
        }
    }
    public void ToggleChat() {
        if (chatOpen) {
            chatOpen = false;
            transform.GetComponent<CanvasGroup>().alpha = 0;
            inputField.DeactivateInputField();
            chatContent.parent.GetComponent<Animator>().SetBool("ChatOpen", false);

        }
        else {
            chatOpen = true;
            transform.GetComponent<CanvasGroup>().alpha = 1;
            inputField.ActivateInputField();
            chatContent.parent.GetComponent<Animator>().SetBool("ChatOpen", true);
        }
    }
    public void FieldEndEdit() {
        if (!string.IsNullOrEmpty(inputField.text) && Input.GetKey(KeyCode.Return)) {
            SendMessage(inputField.text);
            inputField.text = "";
            ToggleChat();
        }
        else {
            ToggleChat();
        }

    }
}
