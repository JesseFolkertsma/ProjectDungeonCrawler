using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {

    Text text;
    float deltaTime;
    public float updateTime = 1f;

    private void Awake()
    {
        text = GetComponent<Text>();
        StartCoroutine(UpdateFPS());
    }

    private void Update()
    {
    }

    IEnumerator UpdateFPS()
    {
        while (true)
        {
            deltaTime += Time.deltaTime;
            deltaTime /= 2;
            text.text = Mathf.Round((1 / deltaTime)).ToString();
            yield return new WaitForSeconds(updateTime);
        }
    }
}
