using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentFix : MonoBehaviour {
    public void Start() {
        transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
    }
}
