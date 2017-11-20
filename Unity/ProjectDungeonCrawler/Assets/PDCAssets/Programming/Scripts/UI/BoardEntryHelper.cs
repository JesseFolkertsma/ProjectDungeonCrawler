using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardEntryHelper : MonoBehaviour {
    public string playerID;
    public int kills;
    public int deaths;

    public Text[] stats;
    public Text name;

    public void Setup(string _name, string _playerID) {
        playerID = _playerID;
        name.text = _name;
    }

    public void Fill() {
        stats[0].text = kills.ToString();
        stats[1].text = deaths.ToString();

    }
    public void Add(int _kills, int _deaths) {
        kills += _kills;
        deaths += _deaths;
        Fill();
    }
}
