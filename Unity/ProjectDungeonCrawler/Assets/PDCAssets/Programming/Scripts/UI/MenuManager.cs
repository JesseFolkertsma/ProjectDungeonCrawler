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
                DisableMenu(i);
            }
        }
        EnableMenu(currentMenu);


    }
    public void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if(previousMenus.Count > 0) {
                NavigateBack();
            }
        }
    }
    public void EnableButtons(int i)
    {
        foreach (Transform n in menus[i])
        {
            n.GetChild(0).GetComponent<Button>().interactable = true;
        }
    }
    public void DisableButtons(int i) {
        foreach (Transform n in menus[i]){
            n.GetChild(0).GetComponent<Button>().interactable = false;
            print("Disabled :" + menus[i].name);
        }
    }
    public void DisableMenu(int i) {
        if (menus[i].GetChild(0).name == "Parent") {
            DisableButtons(i);
        }
        else {
            menus[i].GetComponent<Animator>().SetBool("Open", false);

        }
    }
    public void EnableMenu(int i) {
        if (menus[i].GetChild(0).name == "Parent") {
            EnableButtons(i);
        }
        else {
            menus[i].GetComponent<Animator>().SetBool("Open", true);
        }
    }
    public void NavigateBM(int i) {
        DisableButtons(currentMenu);
        EnableButtons(i);
        previousMenus.Add(currentMenu);
        currentMenu = i;
    }
    public void NavigateM(int i) {
        //Disable current menu
        //Enable overloaded menu
        DisableMenu(currentMenu);
        EnableMenu(i);
        previousMenus.Add(currentMenu);
        currentMenu = i;
    }
    public void NavigateBack(){
        DisableMenu(currentMenu);
        EnableMenu(previousMenus[previousMenus.Count - 1]);
        currentMenu = previousMenus[previousMenus.Count - 1];
        previousMenus.RemoveAt(previousMenus.Count - 1);
    }
}
