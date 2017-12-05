using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour {
    public GameObject replacePref;
    public bool hary;
    public void Update() {
        if (hary) {
            Replace();
            hary = false;
        }
    }
    public void Replace() {
        GameObject newObject = Instantiate(replacePref,transform.position, transform.rotation);
        newObject.transform.position = transform.TransformPoint(transform.position);
        Destroy(gameObject);
    }
}
