using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitIndicator : MonoBehaviour {

    public static HitIndicator instance;
    public float waitTime = .3f;
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
        instance = this;
    }

    public void Hit()
    {
        StartCoroutine(HitRoutine());
    }

    IEnumerator HitRoutine()
    {
        image.enabled = true;
        yield return new WaitForSeconds(waitTime);
        image.enabled = false;
    }
}
