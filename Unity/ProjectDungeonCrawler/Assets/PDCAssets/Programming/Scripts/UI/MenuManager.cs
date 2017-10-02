using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
    public  Transform[] menus;
    int currentMenu;
    public List<int> previousMenus;

    public void Start()
    {
        for(int i = 0; i < menus.Length; i++)
        {
            if(i != currentMenu)
            {
                DisableButtons(i);
            }
        }
        EnableButtons(currentMenu);


    }
    public void EnableButtons(int i)
    {
        foreach (Transform n in menus[i])
        {
            n.GetChild(0).GetComponent<Button>().interactable = true;
        }
    }
    public void DisableButtons(int i)
    {
        foreach (Transform n in menus[i])
        {
            n.GetChild(0).GetComponent<Button>().interactable = false;
            print("Disabled :" + menus[i].name);
        }
    }
    public void Navigate(int i) {
        DisableButtons(currentMenu);
        EnableButtons(i);
        previousMenus.Add(currentMenu);
        currentMenu = i;
    }
    public void NavigateBack()
    {

    }
}
