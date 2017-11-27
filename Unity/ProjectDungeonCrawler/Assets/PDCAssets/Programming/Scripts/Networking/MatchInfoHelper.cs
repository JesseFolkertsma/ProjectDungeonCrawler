using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
public class MatchInfoHelper : MonoBehaviour {
    public Text matchName;
    public Text matchTime;
    public Text matchWarmupTime;
    public Text matchMinPlayer;

    public MatchInfo match;

    public void GetMatchInfo() {
        print("matchmin = " + matchMinPlayer.text);
        match = new MatchInfo(matchName.text, System.Convert.ToInt32(matchTime.text), System.Convert.ToInt32(matchWarmupTime.text), System.Convert.ToInt32(matchMinPlayer), 0);
        print(match.matchName + match.matchMaxTime + match.matchWarmupTime + match.matchMinPlayers + match.matchType);
        print(match.matchMinPlayers);
    }
}
