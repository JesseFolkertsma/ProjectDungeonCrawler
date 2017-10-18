using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public Image hp;
    public Transform ch;


    public void Update() {
        if (Input.GetKey(KeyCode.Tab)) {
            ToggleScoreBoard(true);
        }
        else {
            ToggleScoreBoard(false);
        }
        if (Input.GetButtonDown("Jump")) {
            AddScoreBoardEntry("COokiemonester", "Cooikeeue");
            AddScoreBoardStat("Cooikeeue", 10, 11);
        }
    }


    public void UpdateHealth(float currentHP, float maxHP) {
        hp.fillAmount = (currentHP / (maxHP / 100)) / 100;
    }
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
    public void FeedMessage(string message) {
        GameObject newFM = Instantiate(feedPref);
        newFM.transform.SetParent(feedWindow, false);
        newFM.transform.SetAsFirstSibling();
        newFM.transform.GetChild(0).GetComponent<Text>().text = message;
        fmList.Add(newFM);

    }
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

    public void ToggleScoreBoard(bool foo) {
        scoreBoard.gameObject.SetActive(foo);
    }
    public void AddScoreBoardEntry(string name, string playerID) {
        Transform newEntry = Instantiate(scoreBoardEntryPref);
        newEntry.SetParent(scoreBoardContent, false);
        BoardEntryHelper n = newEntry.GetComponent<BoardEntryHelper>();
        n.Setup(name, playerID);
        entries.Add(n);
    }
    public void AddScoreBoardStat(string playerID, int kills, int deaths) {
        foreach(BoardEntryHelper entry in entries) {
            if(entry.playerID == playerID) {
                entry.Add(kills, deaths);
                return;
            }
        }
    }

}
