﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneralCanvas : MonoBehaviour {
    #region General
    private void Start() {
    }
    private void Update() {
        Controls();
    }
    #endregion
    #region Input
    private void Controls() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            IGMToggle();
        }
        ChatControls();
        SBControls();
    }
    #endregion
    #region     Ingame Menu
    //Variables//
    public Transform ingameMenu;

    //Toggles the IGM on or off depending on the state//
    public void IGMToggle() {
        ingameMenu.GetComponent<CanvasGroup>().blocksRaycasts = !ingameMenu.GetComponent<CanvasGroup>().blocksRaycasts;
        ingameMenu.GetChild(0).GetComponent<Animator>().SetBool("Open", !ingameMenu.GetChild(0).GetComponent<Animator>().GetBool("Open"));
        ingameMenu.GetComponent<Image>().enabled = !ingameMenu.GetComponent<Image>().IsActive();
    }
    //Used to see wich option the player select in the IGM//
    public void IGMOptions(int i) {
        switch (i) {
            case 0:
                SceneManager.LoadScene(0);
                break;
            case 1:
                Application.Quit();
                break;
        }
    }
    #endregion
    #region Notifications/Kill Feed
    
    //Variables//
    public GameObject feedPref;
    public Transform feedWindow;

    List<GameObject> fmList = new List<GameObject>();

    //Put a new message into the local feed window//
    public void FeedMessage(string message) {
        GameObject newFM = Instantiate(feedPref);
        newFM.transform.SetParent(feedWindow, false);
        newFM.transform.SetAsFirstSibling();
        newFM.transform.GetChild(0).GetComponent<Text>().text = message;
        fmList.Add(newFM);

    }
    //Removes the given message out of the feed (This is called by the animator on the messages) //
    public void KillMessage(GameObject message) {
        for (int i = 0; i < fmList.Count; i++) {
            if (fmList[i] == message) {
                Destroy(fmList[i]);
            }
        }
        fmList.Remove(message);
    }
    #endregion
    #region Chat
    //Variables//

    public Transform chat;
    public Transform chatContent;
    public GameObject chatMessage;
    public InputField inputField;

    public bool chatOpen;

    //Checks for input//
    public void ChatControls() {
        if (Input.GetKeyDown(KeyCode.T) && !inputField.isFocused) {
            ToggleChat();
        }
        if (chatOpen) {
            if (Input.GetButtonDown("Cancel")) {
                ToggleChat();
            }
        }
    }
    //Spawns a message in the local chat window//
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
    // Toggles the chat window
    public void ToggleChat() {
        if (chatOpen) {
            chatOpen = false;
            chat.GetComponent<CanvasGroup>().alpha = 0;
            inputField.DeactivateInputField();
            chatContent.parent.GetComponent<Animator>().SetBool("ChatOpen", false);
        }
        else {
            chatOpen = true;
            chat.GetComponent<CanvasGroup>().alpha = 1;
            inputField.ActivateInputField();
            chatContent.parent.GetComponent<Animator>().SetBool("ChatOpen", true);
        }
    }

    public void FieldEndEdit() {
        if(Input.GetKeyDown(KeyCode.Return)) {
            SendMessage(inputField.text);
            inputField.text = "";
            ToggleChat();
           
        }
    }
    #endregion
    #region Hitmarker
    //Variables//
    public Transform hm;

    //Enables hitmark animations//
    public void HitMark() {
        hm.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
    }

    #endregion
    #region Health
    //Variables/
    public Image hp;
    //Updates the health bar with the given data//
    public void UpdateHealth(float currentHP, float maxHP) {
        hp.fillAmount = (currentHP / (maxHP / 100)) / 100;
    }
    #endregion
    #region Scoreboard
    //Variables//

    public Transform scoreBoard;
    public Transform scoreBoardContent;
    public Transform scoreBoardEntryPref;

    public List<BoardEntryHelper> entries = new List<BoardEntryHelper>();

    // Enables ScoreBoard Input
    public void SBControls() {
        if (Input.GetKey(KeyCode.Tab)) {
            SBToggle(true);
        }
        else {
            if (scoreBoard.gameObject.activeInHierarchy != false) {
                SBToggle(false);

            }
        }
    }
    // Toggles the scoreboard
    public void SBToggle(bool foo) {
        scoreBoard.gameObject.SetActive(foo);
    }
    //Adds a new player to the scoreboard
    public void AddScoreBoardEntry(string name, string playerID) {
        Transform newEntry = Instantiate(scoreBoardEntryPref);
        newEntry.SetParent(scoreBoardContent, false);
        BoardEntryHelper n = newEntry.GetComponent<BoardEntryHelper>();
        n.Setup(name, playerID);
        entries.Add(n);
    }
    //Adds kills or deaths to the given player entry
    public void AddScoreBoardStat(string playerID, int kills, int deaths) {
        foreach (BoardEntryHelper entry in entries) {
            if (entry.playerID == playerID) {
                entry.Add(kills, deaths);
                return;
            }
        }
    }
    #endregion

}
