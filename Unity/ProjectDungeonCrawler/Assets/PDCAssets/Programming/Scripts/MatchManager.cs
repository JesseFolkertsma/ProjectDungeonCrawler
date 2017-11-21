using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : NetworkBehaviour {

    public static MatchManager instance;

    public int updateTime = 1;
    public int matchTime = 60;
    public int warmupTime = 60;
    public bool warmup = true;
    public List<MatchData.PlayerMatchData> playerData = new List<MatchData.PlayerMatchData>();

    int warmUpTimeLeft;
    int matchTimeLeft;
    Coroutine match;

    private void Start()
    {
        ResetMatch();
        instance = this;
    }

    public void ResetMatch()
    {
        warmup = true;
        warmUpTimeLeft = warmupTime;
        matchTimeLeft = matchTime;
        if (match != null)
            StopCoroutine(match);
        match = StartCoroutine(UpdateMatchDataRoutine());
    }

    public void JoinMatch(string playerID, string playerName)
    {
        MatchData.PlayerMatchData newPlayer = new MatchData.PlayerMatchData();
        newPlayer.playerName = playerName;
        newPlayer.playerID = playerID;
        playerData.Add(newPlayer);
    }

    IEnumerator UpdateMatchDataRoutine()
    {
        while (true)
        {
            if (warmup)
            {
                warmUpTimeLeft -= updateTime;
                UpdateMatchData(new MatchData(warmUpTimeLeft, playerData.ToArray(), warmup));
                if (warmUpTimeLeft <= 0)
                {
                    EndWarmUp();
                }
            }
            else
            {
                matchTimeLeft -= updateTime;
                UpdateMatchData(new MatchData(matchTimeLeft, playerData.ToArray(), warmup));
            }
            yield return new WaitForSeconds(1 / updateTime);
        }
    }

    void EndWarmUp()
    {
        warmup = false;
    }

    void UpdateMatchData(MatchData matchData)
    {
        NetworkedUI netUI = FindObjectOfType<NetworkedUI>();
        if (netUI == null) return;
        netUI.RpcUpdateMatch(matchData);
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
        public string playerName;
        public int kills;
        public int deaths;
    }
}
