using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardEntryHelper : MonoBehaviour {
    public string playerID;

    public Text[] stats;
    public Text name;
    public int wins;

    public void Setup(string _name, string _playerID) {
        playerID = _playerID;
        name.text = _name;
        wins = 0;
    }
    public void UpdateEntry(int _kills, int _deaths) {
        stats[0].text = _kills.ToString();
        stats[1].text = _deaths.ToString();
    }
    public void ReChild(int i) {
        transform.SetSiblingIndex(i);
    }

    public void Win(bool activate) {
        transform.GetChild(2).GetComponent<Image>().enabled = activate;
        wins += 1;
        transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = wins.ToString();
    }
}
