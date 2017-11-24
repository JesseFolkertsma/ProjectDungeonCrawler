using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MatchManager : NetworkBehaviour {
    public enum MatchState
    {
        WaitForPlayers,
        Warmup,
        Playing,
        MatchEnd,
    };

    public static MatchManager instance;

    public MatchState matchState = MatchState.Warmup;

    public int updateTime = 1;
    public int playersNeeded = 2;
    public int warmupTime = 60;
    public int matchTime = 60;
    public int matchEndTime = 10;
    public List<MatchData.PlayerMatchData> playerData = new List<MatchData.PlayerMatchData>();

    int warmUpTimeLeft;
    int matchTimeLeft;
    int matchEndTimeLeft;
    Coroutine match;
    NetworkedUI netUI;

    private void Start()
    {
        instance = this;
        netUI = FindObjectOfType<NetworkedUI>();
        ResetMatch(false);
    }

    public void ResetMatch(bool killPlayers)
    {
        matchState = MatchState.WaitForPlayers;
        warmUpTimeLeft = warmupTime;
        matchTimeLeft = matchTime;
        matchEndTimeLeft = matchEndTime;
        if (match != null)
            StopCoroutine(match);
        match = StartCoroutine(UpdateMatchDataRoutine());
        if(killPlayers)
            ResetPlayersAndStats();
        netUI.RpcResetMatch();
    }

    public void JoinMatch(string playerID, string playerName)
    {
        MatchData.PlayerMatchData newPlayer = new MatchData.PlayerMatchData();
        newPlayer.playerName = playerName;
        newPlayer.playerID = playerID;
        playerData.Add(newPlayer);

        //GeneralCanvas.canvas.AddScoreBoardEntry(playerName, playerID);
        Debug.Log("My friendly boi: " + playerName + " with id: " + playerID + " has joined!");
    }

    public void LeaveMatch(string playerID)
    {
        MatchData.PlayerMatchData toRemove = null;
        foreach(MatchData.PlayerMatchData pmd in playerData)
        {
            if(pmd.playerID == playerID)
            {
                toRemove = pmd;
                break;
            }
        }
        if(toRemove != null)
        {
            playerData.Remove(toRemove);
        }
    }

    public void PlayerKilled(string killerID, string victimID)
    {
        bool killerFound = false;
        bool victimFound = false;
        foreach (MatchData.PlayerMatchData pmd in playerData)
        {
            if (killerFound && victimFound)
                return;
            if(pmd.playerID == killerID)
            {
                pmd.kills++;
                killerFound = true;
                continue;
            }
            else if(pmd.playerID == victimID)
            {
                pmd.deaths++;
                victimFound = true;
                continue;
            }
        }
    }

    IEnumerator UpdateMatchDataRoutine()
    {
        while (true)
        {
            switch (matchState)
            {
                case MatchState.WaitForPlayers:
                    int playersInServer = PlayerManager.PlayerList().Count;
                    Debug.Log(playersInServer);
                    UpdateMatchData(new MatchData(0, playerData.ToArray(), matchState));
                    if (playersInServer >= playersNeeded)
                    {
                        EnoughPlayers();
                    }
                    break;
                case MatchState.Warmup:
                    warmUpTimeLeft -= updateTime;
                    UpdateMatchData(new MatchData(warmUpTimeLeft, playerData.ToArray(), matchState));
                    if (warmUpTimeLeft <= 0)
                    {
                        EndWarmUp();
                    }
                    break;
                case MatchState.Playing:
                    matchTimeLeft -= updateTime;
                    UpdateMatchData(new MatchData(matchTimeLeft, playerData.ToArray(), matchState));
                    if(matchTimeLeft <= 0)
                    {
                        MatchEnd();
                    }
                    break;
                case MatchState.MatchEnd:
                    matchEndTimeLeft -= updateTime;
                    UpdateMatchData(new MatchData(matchEndTimeLeft, playerData.ToArray(), matchState));
                    if (matchEndTimeLeft <= 0)
                    {
                        ResetMatch(true);
                    }
                    break;
            }
            yield return new WaitForSeconds(1 / updateTime);
        }
    }

    void EnoughPlayers()
    {
        matchState = MatchState.Warmup;
    }

    void EndWarmUp()
    {
        matchState = MatchState.Playing;
        ResetPlayersAndStats();
    }

    void MatchEnd()
    {
        matchState = MatchState.MatchEnd;
        netUI.RpcEndMatch();
    }

    public void ResetPlayersAndStats()
    {
        foreach (KeyValuePair<string, NWPlayerCombat> kvp in PlayerManager.PlayerList())
        {
            kvp.Value.RpcDie();
        }
        foreach (MatchData.PlayerMatchData pmd in playerData)
        {
            pmd.Reset();
        }
    }

    void UpdateMatchData(MatchData matchData)
    {
        if (netUI == null) return;
        netUI.RpcUpdateMatch(matchData);
    }

}

[System.Serializable]
public class MatchData
{
    public int seconds;
    public PlayerMatchData[] playerData;
    public MatchManager.MatchState matchState;

    public MatchData() { }
    public MatchData(int _seconds, PlayerMatchData[] _playerData, MatchManager.MatchState _matchState)
    {
        seconds = _seconds;
        playerData = _playerData;
        matchState = _matchState;
    }

    public class PlayerMatchData
    {
        public string playerID;
        public string playerName;
        public int kills;
        public int deaths;

        public void Reset()
        {
            kills = 0;
            deaths = 0;
        }
    }
}
