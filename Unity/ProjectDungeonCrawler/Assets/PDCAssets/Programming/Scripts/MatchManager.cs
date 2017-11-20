using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : NetworkBehaviour {

    public int updateTime = 1;
    public int matchTime = 180;
    public List<MatchData.PlayerMatchData> playerData = new List<MatchData.PlayerMatchData>();

    int matchTimeLeft = 180;

    IEnumerator UpdateMatchDataRoutine()
    {
        while (true)
        {
            matchTime -= updateTime;
            UpdateMatchData(new MatchData(matchTimeLeft, playerData.ToArray()));
            yield return new WaitForSeconds(1 / updateTime);
        }
    }

    void UpdateMatchData(MatchData matchData)
    {

    }
}

[System.Serializable]
public class MatchData
{
    public int seconds;
    public PlayerMatchData[] playerData;

    public MatchData() { }
    public MatchData(int _seconds, PlayerMatchData[] _playerData)
    {
        seconds = _seconds;
        playerData = _playerData;
    }

    public class PlayerMatchData
    {
        public string playerID;
        public int kills;
        public int deaths;
    }
}
