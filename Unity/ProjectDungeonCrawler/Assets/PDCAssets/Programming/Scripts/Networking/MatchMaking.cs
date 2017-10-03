using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMaking : MonoBehaviour {
    public InputField name;
    public InputField ip;
    public Text error;

    public string[] errorMessages;
    public void JoinGame() {
        if(name.text.Length > 0 || ip.text.Length > 0) {
            Error(2);
        }
        else {
            Error(0);
        }
    }
    public void HostGame() {
        if(name.text.Length > 0) {
            Error(2);
        }
        else {
            Error(1);
        }
    }
    public void Error(int i) {
        error.text = errorMessages[i];
    }
}
