using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class Trade {
    public Item item;
    public string seller;
    //public SerializableDictionary<string, Inventory> receivers;
    public List<string> receivers;
    public int itemI;

    public Trade()
    {
        item = null;
        seller = null;
        receivers = new List<string>();
    }

    public Trade(Item _item, string _sellerID, string[] _receiversID, int ii){
        item = _item;
        seller = _sellerID;
        receivers = new List<string>();
        foreach(string i in _receiversID)
        {
            Debug.Log("___________ADDING NEW PLAYER TO TRADE " + i);
            receivers.Add(i);
        }
        itemI = ii;
    }

    public void Accept(string playerID){
        //receivers[playerID].Add(item);
        ItemManager.instance.GiveItemToPlayerID(playerID, item);
        receivers.Remove(playerID);
        foreach(string i in receivers)
        {
            //i.Value.DeclineTrade();
        }
    }

    public void Decline(string playerID){
        receivers.Remove(playerID);
        if(receivers.Count < 1){
            Stop();
        }
    }
    public void Stop(){

    }

    public static Trade DeserializeFromNetwork(byte[] networkSerializedTrade)
    {
        MemoryStream stream = new MemoryStream(networkSerializedTrade);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        return (Trade)binaryFormatter.Deserialize(stream);
    }

    public byte[] SerializeForNetwork()
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(stream, this);
        return stream.GetBuffer();
    }
}
