using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public Image hp;
    public Transform ch;


    public void Update() {
    }
    public void UpdateHealth(float currentHP, float maxHP) {
        hp.fillAmount = (currentHP / (maxHP / 100)) / 100;
    }
    public void HitMark() {
        ch.GetChild(0).GetComponent<Animator>().SetTrigger("Hit");
    }
    
}
