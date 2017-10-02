using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class iLHelper : MonoBehaviour
{
    Text name;
    Button drop;
    Button trade;
    Text value;

    public InputField valueField;

    public int nameId;
    public int dropId;
    public int tradeId;
    public int valueFId;
    public int valueId;

    public int listIndex;
    public void Awake()
    {
        name = transform.GetChild(nameId).GetComponent<Text>();
        drop = transform.GetChild(dropId).GetComponent<Button>();
        trade = transform.GetChild(tradeId).GetComponent<Button>();
        value = transform.GetChild(valueId).GetComponent<Text>();
        valueField = transform.GetChild(valueFId).GetComponent<InputField>();
    }
    public void Fill(Inventory inv, Item item, int index)
    {
        listIndex = index;
        name.text = item.name;
        value.text = item.value.ToString();
        drop.onClick.AddListener(() => inv.Drop(index));
        trade.onClick.AddListener(() => inv.SetupTrade(index));
        valueField.onEndEdit.AddListener(delegate { inv.ChangeValue(index); });
    }
    public void Renumber(Inventory inv, int i)
    {
        listIndex = i;
        valueField.onEndEdit.RemoveAllListeners();
        valueField.onEndEdit.AddListener(delegate { inv.ChangeValue(listIndex); });
        drop.onClick.RemoveAllListeners();
        drop.onClick.AddListener(() => inv.Drop(listIndex));
    }
    public void Refresh(Inventory inv)
    {
        valueId = inv.inventory[listIndex].value;
        value.text = valueId.ToString();

    }
}