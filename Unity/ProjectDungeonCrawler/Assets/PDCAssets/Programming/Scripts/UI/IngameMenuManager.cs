using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameMenuManager : MonoBehaviour {

    CanvasGroup ingameMenu;
    private void Start() {
        ingameMenu = transform.GetComponent<CanvasGroup>();
    }
    private void Update() {
        if (Input.GetButtonDown("Cancel")) {
            Toggle();
        }
    }
    private void Toggle() {
        ingameMenu.blocksRaycasts = !ingameMenu.blocksRaycasts;
        //ingameMenu.interactable = !ingameMenu.interactable;
        transform.GetChild(0).GetComponent<Animator>().SetBool("Open", !transform.GetChild(0).GetComponent<Animator>().GetBool("Open"));
        transform.GetComponent<Image>().enabled = !transform.GetComponent<Image>().IsActive();
    }
    public void Options(int i) {
        switch (i) {
            case 0:
                SceneManager.LoadScene(0); 
                break;
            case 1:
                Application.Quit();
                break;
        }
    }
}
