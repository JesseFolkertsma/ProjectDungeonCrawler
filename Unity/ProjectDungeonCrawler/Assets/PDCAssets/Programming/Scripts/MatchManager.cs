using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : NetworkBehaviour {

    public int updateTime = 1;
    public int matchTime = 180;
    public List<MatchData.PlayerMatchData> playerData = new List<MatchData.PlayerMatchData>();

    int matchTimeLeft = 180;

    private void Start()
    {
        matchTimeLeft = matchTime;
        StartCoroutine(UpdateMatchDataRoutine());
    }

    IEnumerator UpdateMatchDataRoutine()
    {
        while (true)
        {
            matchTimeLeft -= updateTime;
            UpdateMatchData(new MatchData(matchTimeLeft, playerData.ToArray(), true));
            yield return new WaitForSeconds(1 / updateTime);
        }
    }

    void UpdateMatchData(MatchData matchData)
    {
        NetworkedUI pc = FindObjectOfType<NetworkedUI>();
        if (pc == null) return;
        pc.RpcUpdateMatch(matchData);
    }
}

[System.Serializable]
public class MatchData
{
    public int seconds;
    public PlayerMatchData[] playerData;
    public bool warmup;

    public MatchData() { }
    public MatchData(int _seconds, PlayerMatchData[] _playerData, bool _warmup)
    {
        seconds = _seconds;
        playerData = _playerData;
        warmup = _warmup;
    }

    public class PlayerMatchData
    {
        public string playerID;
        public int kills;
        public int deaths;
    }
}
