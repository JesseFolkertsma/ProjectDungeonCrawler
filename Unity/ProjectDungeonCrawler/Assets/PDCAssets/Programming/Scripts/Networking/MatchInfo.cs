using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchInfo : MonoBehaviour {
    public string matchName;
    public int matchMaxTime = 60;
    public int matchWarmupTime = 15;
    public int matchMinPlayers = 2;

    public enum MatchType { FFADM, CTF };
    public MatchType matchType = MatchType.FFADM;

    public MatchInfo(string name, int maxTime, int warmupTime, int minPlayers, int _matchType) {
        matchName = name;
        matchMaxTime = maxTime;
        matchWarmupTime = warmupTime;
        matchMinPlayers = minPlayers;
        matchType = (MatchType)_matchType;
    }
}
