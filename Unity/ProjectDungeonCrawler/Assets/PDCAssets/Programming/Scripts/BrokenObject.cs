using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenObject : MonoBehaviour {
    Vector3 exploc;
    public int explosionForce;
    public int explosionRadius;
    public void Start() {
        foreach (Rigidbody child in transform.GetComponentsInChildren<Rigidbody>()) {
            child.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }
    }
}
 