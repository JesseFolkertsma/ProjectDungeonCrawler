using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameplayCanvas : MonoBehaviour {
    #region General
    private void Start() {

    }
    private void Update() {
        Controls();
    }
    #endregion
    #region Hitmarker
    //Variables//
    Transform hm;

    //Enables hitmark animations//
    public void HitMark() {
        hm.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
    }

    #endregion
    #region Health
    //Variables/
    Image hp;
    //Updates the health bar with the given data//
    public void UpdateHealth(float currentHP, float maxHP) {
        hp.fillAmount = (currentHP / (maxHP / 100)) / 100;
    }
    #endregion
    #region Controls
    private void Controls() {
        SBControls();
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
                entry.UpdateEntry(kills, deaths);
                return;
            }
        }
    }
    #endregion
}

