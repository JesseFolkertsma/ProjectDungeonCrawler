using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour
{
    public static ItemManager instance;

    [SerializeField]
    public Dictionary<int, Trade> ongoingTrades;

    private void Awake()
    {
        instance = this;
        ongoingTrades = new Dictionary<int, Trade>();
    }


    public void TradeAccepted(int tradeID, string playerID)
    {
        Debug.Log("I am accepted by: " + playerID + " und das trade id ez: " + tradeID.ToString());
        Trade trade = ongoingTrades[tradeID];
        GiveItemToPlayerID(playerID, trade.item);
        foreach(string s in trade.receivers)
        {
            if(s != playerID)
            {
                Inventory i = GameObject.Find(s).GetComponent<Inventory>();
                i.TargetCloseTrade(i.GetComponent<NetworkIdentity>().connectionToClient);
            }
        }
        Inventory seller = GameObject.Find(trade.seller).GetComponent<Inventory>();
        seller.TargetDrop(seller.GetComponent<NetworkIdentity>().connectionToClient, trade.SerializeForNetwork());
        ongoingTrades.Remove(tradeID);
    }

    public void TradeDeclined(int tradeID, string playerID)
    {
        Trade trade = ongoingTrades[tradeID];
        trade.receivers.Remove(playerID);
        if(trade.receivers.Count < 1)
        {
            ongoingTrades.Remove(tradeID);
        }
    }
    
    public void SendTrade(byte[] serTrade, string playerID)
    {
        Trade trade = Trade.DeserializeFromNetwork(serTrade);
        int newTradeID = CreateRandomTradeID();
        Debug.Log("New trad ID ez: " + newTradeID.ToString());
        ongoingTrades.Add(newTradeID, trade);
        foreach (string s in trade.receivers)
        {
            GameObject target = GameObject.Find(s);
            target.GetComponent<Inventory>().TargetRecieveTrade(target.GetComponent<NetworkIdentity>().connectionToClient, serTrade, newTradeID);
        }
    }

    public int CreateRandomTradeID()
    {
        int rng = UnityEngine.Random.Range(0, 1000);
        if (ongoingTrades.Count > 0)
        {
            if (ongoingTrades.ContainsKey(rng))
            {
                rng = CreateRandomTradeID();
            }
        }
        return rng;
    }

    public void GiveItemToPlayerID(string id, Item itemToGive)
    {
        GameObject go = GameObject.Find(id);
        go.GetComponent<Inventory>().TargetAdd(go.GetComponent<NetworkIdentity>().connectionToClient, itemToGive);
    }
}
