using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneralCanvas : MonoBehaviour {


    #region General
    //Variables//
    public static GeneralCanvas canvas;
    private void Awake () {
        chatAnim = chatContent.parent.GetComponent<Animator>();
        canvas = this;
        crosshairStart();
    }
    private void Update() {
        Controls();
        if (Input.GetKey(KeyCode.K)) {
            CHChange(1);
        }
        if (Input.GetKey(KeyCode.J)) {

            CHSpread(.1f);
        }
    }
    public void MatchDataUpdate(MatchData data) {
        //Timer Update
        UpdateTimer(data.seconds, data.matchState);

        //Scoreboard Update
        Debug.Log(data.playerData.Length + "entries: " + entries.Count.ToString());
        foreach(MatchData.PlayerMatchData pmd in data.playerData) {
            if(!AddScoreBoardStat(pmd.playerID, pmd.kills, pmd.deaths))
            {
                AddScoreBoardEntry(pmd.playerName, pmd.playerID);
            }
        }
        if(entries.Count != data.playerData.Length)
        {
            List<BoardEntryHelper> toRemove = new List<BoardEntryHelper>();
            foreach(BoardEntryHelper beh in entries)
            {
                bool found = false;
                foreach (MatchData.PlayerMatchData pmd in data.playerData)
                {
                    if(pmd.playerID == beh.playerID)
                    {
                        found = true;
                    }
                }
                if (found)
                {
                    toRemove.Add(beh);
                }
            }
            foreach(BoardEntryHelper beh in toRemove)
            {
                entries.Remove(beh);
            }
            //Ammo update

        }
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
    public Animator chatAnim;

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
                if (!chatAnim.GetBool("ChatNew")) {
                    chatAnim.SetBool("ChatNew", true);
                }
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
            inputField.interactable = false;
            chatContent.parent.GetComponent<Animator>().SetBool("ChatOpen", false);
        }
        else {

            chatOpen = true;
            chat.GetComponent<CanvasGroup>().alpha = 1;
            inputField.interactable = true;
            inputField.ActivateInputField();
            chatContent.parent.GetComponent<Animator>().SetBool("ChatOpen", true);
        }
    }

    #endregion
    #region Crosshairs
    //Variables//

    Transform currentCH;
    Animator currentCHAnim;
    public Transform crosshairObject;
    public Transform hitMark;

    Coroutine spread;

    public void crosshairStart() {
        foreach(Transform child in crosshairObject) {
            child.GetComponent<CanvasGroup>().alpha = 0;
        }
    }

    public void CHChange(int index) {
        if (currentCH != null) {
            currentCH.GetComponent<CanvasGroup>().alpha = 0;
        }
        currentCH = crosshairObject.GetChild(index);
        currentCHAnim = currentCH.GetComponent<Animator>();
        currentCH.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void CHSpread(float amount) {
        if (currentCHAnim != null) {
            float newAmount = currentCHAnim.GetFloat("Blend") + amount;
            if(newAmount > 1)
            {
                newAmount = 1;
            }
            if (spread != null) {
                StopCoroutine(spread);
            }
            currentCHAnim.SetFloat("Blend", newAmount);
            spread = StartCoroutine(SpreadReset());
        }
    }
    IEnumerator SpreadReset() {
        while (currentCHAnim.GetFloat("Blend") >= 0) {
            float n = currentCHAnim.GetFloat("Blend");
            currentCHAnim.SetFloat("Blend", Mathf.Lerp(n, 0, 0.1f));
            yield return new WaitForSeconds(0.01f);
        }
        currentCHAnim.SetFloat("Blend", 0);
    }
    //Enables hitmark animations//
    public void HitMark() {
        hitMark.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
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
    #region Ammo
    //Variables//
    public Text ammoCount;
    public Transform ammoParent;
    public Animator bulletAnim;
    public Transform bulletPref;

    public void SetAmmoCount(bool playGone, bool infinite, int max, int current) {
        ammoCount.text = current + " / " + max;
        if (infinite) {
           
        }
        if (playGone) {
            Gone();
        }
    }
    public void Reloading(bool active) {
        bulletAnim.SetBool("Reloading", active);
    }
    public void Gone() {
        Transform newBullet = Instantiate(bulletPref);
        newBullet.SetParent(ammoParent, false);
        newBullet.GetComponent<Animator>().SetTrigger("Gone");
    }

#endregion
    #region Timers
    //Variables//
    public Text timerMatch;
    public Text timerWarmup;
    public void UpdateTimer(float i, MatchManager.MatchState state) {
        Vector2 timer = TimeDiffuse(i);
        if (timer.x < 0 || timer.y < 0)
            return;

        string minutes = timer.x.ToString();
        string seconds = timer.y.ToString();
        if (timer.y < 10) {
            seconds = "0" + timer.y.ToString();
        }
        switch (state)
        {
            case MatchManager.MatchState.WaitForPlayers:
                timerMatch.text = "Waiting for other players";
                break;
            case MatchManager.MatchState.Warmup:
                timerMatch.text = "Warmup time \n" + minutes + " :" + seconds;
                break;
            case MatchManager.MatchState.Playing:
                timerMatch.text = "Time left \n" + minutes + " :" + seconds;
                break;
            case MatchManager.MatchState.MatchEnd:
                timerMatch.text = "Restarting match \n" + minutes + " :" + seconds;
                break;
        }
    }
    Vector2 TimeDiffuse(float i) {
        Vector2 times;
        double minutes = 0;
        double seconds = 0;
        minutes = i / 60;
        seconds = ((minutes - Math.Truncate(minutes)) * 60);
        minutes = Math.Truncate(minutes);
        times = new Vector2((float)minutes, (float)seconds);
        return times;
    }
    #endregion
    #region Scoreboard
    //Variables//

    public Transform scoreBoard;
    public Transform scoreBoardContent;
    public Transform scoreBoardEntryPref;

    public List<BoardEntryHelper> entries = new List<BoardEntryHelper>();

    bool win = false;

    public void MatchEnd()
    {
        win = true;
        SBToggle(true);
        PlayerWin(true);
    }

    public void ResetMatch()
    {
        win = false;
    }

    // Enables ScoreBoard Input
    public void SBControls() {
        if (win)
        {
            SBToggle(true);
        }
        else
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                SBToggle(true);
            }
            else
            {
                if (scoreBoard.gameObject.activeInHierarchy != false)
                {
                    SBToggle(false);

                }
            }
        }
        //Testing Controls //
        //if (Input.GetKeyDown(KeyCode.T)) {
        //    AddScoreBoardEntry("hary", "misterHary");
        //}
        //if (Input.GetKeyDown(KeyCode.Y)) {
        //    AddScoreBoardEntry("hary2", "misterHary2");
        //}
        //if (Input.GetKeyDown(KeyCode.U)) {
        //    AddScoreBoardStat("misterHary",1,0);
        //}
        //if (Input.GetKeyDown(KeyCode.I)) {
        //    AddScoreBoardStat("misterHary2",1,0);
        //}
        //if (Input.GetKeyDown(KeyCode.P)) {
        //    PlayerWin(win);
        //    win = !win;
        //}
    }
    public void PlayerWin(bool activate) {
        GetHighestKillCount(entries).Win(activate);
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
    public bool AddScoreBoardStat(string playerID, int kills, int deaths) {
        foreach (BoardEntryHelper entry in entries) {
            if (entry.playerID == playerID) {
                entry.UpdateEntry(kills, deaths);
                Arrange();
                return true;
            }
        }
        return false;
    }
    public void Arrange() {
        print("Arranging");
        List<BoardEntryHelper> newList = new List<BoardEntryHelper>();
        List<BoardEntryHelper> theList = ConvertList<BoardEntryHelper>(entries);
        for (int i = 0; i < theList.Count; i++) {
            BoardEntryHelper entry = GetHighestKillCount(theList);
            entry.ReChild(i);
            theList.Remove(entry);
        }
    }
    BoardEntryHelper GetHighestKillCount(List<BoardEntryHelper> list) {
        int highest = 0;
        int current = 0;
        BoardEntryHelper highestBoi = list[0];
        foreach(BoardEntryHelper entry in list) {
            int.TryParse(entry.stats[0].text, out current);
            if (current > highest ) {
                highest = current;
                highestBoi = entry;
            }
        }
        return highestBoi;
    }
    #endregion
    #region Deathscreen 
    public void DeathscreenActivate(bool activate)
    {
        int intensity = 0;
        if (activate)
        {
            intensity = 1;
        }
        foreach(GreyscaleFX fx in FindObjectsOfType<GreyscaleFX>()) {
            fx.intensity = intensity;
        }
    }
    #endregion
    #region Tools
    public List<T> ConvertList<T>(List<T> convertable) {
        List<T> ret = new List<T>();
        foreach (T transgender in convertable)
            ret.Add(transgender);
        return ret;
    }
#endregion
}
