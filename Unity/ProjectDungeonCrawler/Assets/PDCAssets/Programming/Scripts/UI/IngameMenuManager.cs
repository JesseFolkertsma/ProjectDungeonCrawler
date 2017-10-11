using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        ingameMenu.interactable = !ingameMenu.interactable;
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.active);
    }
}
