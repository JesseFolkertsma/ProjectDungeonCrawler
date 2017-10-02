using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradePopup : MonoBehaviour {

    public Text traderName;
    public Text itemName;

    public Button accept;
    public Button decline;

    public void Setup(Inventory inv)
    {
        accept.onClick.AddListener(() => inv.AcceptTrade());
        decline.onClick.AddListener(() => inv.DeclineTrade());
    }

    public void SetupUI(string sender, Item item)
    {
        traderName.text = sender + " would like to trade with you!";
        itemName.text = "Item: " + item.name + ", with a value of: " + item.value.ToString();
    }

}
