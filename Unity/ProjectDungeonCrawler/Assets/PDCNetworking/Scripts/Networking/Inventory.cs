using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class Inventory : NetworkBehaviour {
    public List<Item> inventory = new List<Item>();
    public List<iLHelper> inventoryListings = new List<iLHelper>();

    public GameObject canvas;
    public GameObject tradePopup;
    public Transform content;

    public GameObject iL;
    public GameObject newCanvas;
    public GameObject newTradePopup;

    bool invActive = false;

    public float tradeRadius;
    TradePopup popup;
    Trade currentTrade;
    int currentTradeID;

    public bool isTrading = false;

    public Inventory() { }

    private void Start(){
        if (isLocalPlayer)
        {
            canvas = Instantiate(newCanvas);
            content = canvas.transform.GetChild(0);
            tradePopup = Instantiate(newTradePopup, canvas.transform);
            popup = tradePopup.GetComponent<TradePopup>();
            popup.Setup(this);
            tradePopup.SetActive(false);
            if (tradePopup == null)
                Debug.Log("WAT DE FUCK VUILE TEEFUS MENGOL");
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            invActive = !invActive;
            content.gameObject.SetActive(invActive);
        }
    }

    public void Add(Item item){
        inventory.Add(item);
        iLHelper n = Instantiate(iL, Vector3.zero, Quaternion.identity).GetComponent<iLHelper>();
        n.transform.parent = content;
        n.Fill(this, item, inventory.Count - 1);
        inventoryListings.Add(n);
        //Add item in UI
    }

    [TargetRpc]
    public void TargetAdd(NetworkConnection conn, Item item)
    {
        Add(item);
    }

    public void Drop(int i){
        inventory.RemoveAt(i);
        int n = inventoryListings[i].listIndex;
        Destroy(inventoryListings[i].gameObject);
        inventoryListings.RemoveAt(i);
        Refresh(n);
        //Delete item in UI

    }

    [TargetRpc]
    public void TargetDrop(NetworkConnection conn, byte[] _trade)
    {
        Trade trade = Trade.DeserializeFromNetwork(_trade);
        Drop(trade.itemI);
    }

    public void AcceptTrade()
    {
        if (isTrading)
        {
            tradePopup.SetActive(false);
            isTrading = false;
            CmdAcceptTrade(currentTradeID, gameObject.name);
        }
    }

    [Command]
    public void CmdAcceptTrade(int tradeID, string playerID)
    {
        ItemManager.instance.TradeAccepted(tradeID, playerID);
    }

    [Command]
    public void CmdDeclineTrade(int tradeID, string playerID)
    {
        ItemManager.instance.TradeDeclined(tradeID, playerID);
    }

    public void DeclineTrade()
    {
        tradePopup.SetActive(false);
        isTrading = false;
        CmdDeclineTrade(currentTradeID, gameObject.name);
    }

    [TargetRpc]
    public void TargetCloseTrade(NetworkConnection conn)
    {
        tradePopup.SetActive(false);
        isTrading = false;
    }

    [TargetRpc]
    public void TargetAcceptTrade(NetworkConnection conn, Item item)
    {
        Add(item);
        tradePopup.SetActive(false);
    }

    public void SetupTrade(int i)
    {
        Collider[] n = Physics.OverlapSphere(transform.position, 1000);
        List<string> remotePlayerList = new List<string>();
        foreach (Collider nn in n)
        {
            if (nn.transform.root.tag == "Player" && nn.transform.root != transform)
            {
                remotePlayerList.Add(nn.transform.root.name);
            }
        }
        if (remotePlayerList.Count > 0)
        {
            Trade newTrade = new Trade(inventory[i], gameObject.name, remotePlayerList.ToArray(), i);
            CmdSetupTrade(newTrade.SerializeForNetwork());
        }
        else
        {
            print("No frendz to trad hary with");
        }
    }

    [Command]
    public void CmdSetupTrade(byte[] trade)
    {
        ItemManager.instance.SendTrade(trade, gameObject.name);
    }

    #region TestTrades
    public void EasyTrade(int itemToTrade)
    {
        CmdEasyTrade(inventory[itemToTrade]);
    }

    [Command]
    public void CmdEasyTrade(Item itemToTrade)
    {
        Debug.Log("Looking for tradey friendz");
        Collider[] n = Physics.OverlapSphere(transform.position, 1000);
        List<Inventory> remotePlayerList = new List<Inventory>();
        Debug.Log(n.Length + " is my amount of maybe frendz");
        foreach (Collider nn in n)
        {
            if (nn.transform.root.tag == "Player" && nn.transform.root != transform)
            {
                Debug.Log("Added frend : " + nn.transform.root.name);
                remotePlayerList.Add(nn.transform.root.GetComponent<Inventory>());
            }
        }
        if (remotePlayerList.Count > 0)
        {
            foreach (Inventory i in remotePlayerList)
            {
                i.RpcEasyRecieve(gameObject.name, itemToTrade);
            }
        }
        else
        {
            print("No frendz to trad hary with");
        }
    }

    [ClientRpc]
    public void RpcEasyRecieve(string sender, Item itemToTrade)
    {
        Debug.Log("Hello! iz me " + gameObject.name + "! An i gots a trade frem my fren: " + sender + ", and he wan to gifes me: " + itemToTrade.name + " wit value: " + itemToTrade.value.ToString());
        tradePopup.SetActive(true);
        popup.SetupUI(gameObject.name, itemToTrade);
    }
    #endregion

    [TargetRpc]
    public void TargetRecieveTrade(NetworkConnection conn, byte[] serIncomingTrade, int tradeID)
    {
        Trade trade = Trade.DeserializeFromNetwork(serIncomingTrade);
        currentTradeID = tradeID;
        Debug.Log("Hello am nem eh jeff jk is is: " + gameObject.name + " en i gotz ze trade wit id: " + tradeID.ToString());
        if (trade.seller != gameObject.name)
        {
            isTrading = true;
            OpenPopup(trade);
            Debug.Log("Hello! iz me " + gameObject.name + "! An i gots a trade frem my fren: " + trade.seller + ", and he wan to gifes me: " + trade.item.name + " with a value of: " + trade.item.value);
        }
    }

    void OpenPopup(Trade trade)
    {
        tradePopup.SetActive(true);
        popup.SetupUI(trade.seller, trade.item);
    }

    public void Refresh(int i){
        for(;i < inventoryListings.Count; i++){
            inventoryListings[i].Renumber(this, i);
        }
    }
    public void ChangeValue(int i){
        print(i);
        inventory[i].value = Int32.Parse(inventoryListings[i].valueField.text);
        inventoryListings[i].Refresh(this);
    }
}
