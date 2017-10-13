using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Feed : MonoBehaviour {
    public GameObject feedPref;
    public Transform feedWindow;

    List<GameObject> fmList = new List<GameObject>();


    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            FeedMessage("Hary gotz a kyl");
        }
    }
    public void FeedMessage(string message) {
        GameObject newFM = Instantiate(feedPref);
        newFM.transform.SetParent(feedWindow, false);
        newFM.transform.SetAsFirstSibling();
        newFM.transform.GetChild(0).GetComponent<Text>().text = message;
        fmList.Add(newFM);
        
    }
    public void KillMessage(GameObject message) {
        for(int i = 0; i < fmList.Count ; i++) {
            if(fmList[i] == message) {
                Destroy(fmList[i]);
            }
        }
        fmList.Remove(message);
    }
}
