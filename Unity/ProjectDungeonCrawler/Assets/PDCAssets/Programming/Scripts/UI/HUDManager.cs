using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public Image hp;
    public Transform ch;
    public NWPlayerCombat pc;

    public void Init(NWPlayerCombat _pc)
    {
        pc = _pc;
    }

    public void Update() {
        ScoreBoardControls();
        ChatControls();
    }

    //Updates the health bar with the given values
    public void UpdateHealth(float currentHP, float maxHP) {
        hp.fillAmount = (currentHP / (maxHP / 100)) / 100;
    }

    // Enables the hitmarker animation
    public void HitMark() {
        ch.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
    }

    // FEED //

    public GameObject feedPref;
    public Transform feedWindow;

    List<GameObject> fmList = new List<GameObject>();


    private void Start() {
        FeedMessage("Hary");
    }

    // Can be called to put a feed message into the local feed window
    public void FeedMessage(string message) {
        GameObject newFM = Instantiate(feedPref);
        newFM.transform.SetParent(feedWindow, false);
        newFM.transform.SetAsFirstSibling();
        newFM.transform.GetChild(0).GetComponent<Text>().text = message;
        fmList.Add(newFM);

    }

    // Kills the given message if it exists in the feed
    public void KillMessage(GameObject message) {
        for (int i = 0; i < fmList.Count; i++) {
            if (fmList[i] == message) {
                Destroy(fmList[i]);
            }
        }
        fmList.Remove(message);
    }

    // Scoreboard //

    public Transform scoreBoard;
    public Transform scoreBoardContent;
    public Transform scoreBoardEntryPref;

    public List<BoardEntryHelper> entries = new List<BoardEntryHelper>();

    // Enables ScoreBoard Input
    public void ScoreBoardControls() {
        if (Input.GetKey(KeyCode.Tab)) {
            ToggleScoreBoard(true);
        }
        else {
            ToggleScoreBoard(false);
        }
    }
    // Toggles the scoreboard
    public void ToggleScoreBoard(bool foo) {
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
        foreach(BoardEntryHelper entry in entries) {
            if(entry.playerID == playerID) {
                entry.Update(kills, deaths);
                return;
            }
        }
    }

    // Chat //
    public Transform chat;
    public Transform chatContent;
    public GameObject chatMessage;
    public InputField inputField;

    public bool chatOpen;


    // Enables chat input
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

    // Spawns a message in the local chat window
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

    }
}


