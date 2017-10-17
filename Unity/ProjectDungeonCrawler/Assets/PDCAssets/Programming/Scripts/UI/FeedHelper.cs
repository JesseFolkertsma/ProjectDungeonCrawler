using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedHelper : MonoBehaviour {
    public void Kill() {
        transform.GetComponentInParent<HUDManager>().KillMessage(gameObject);
    }
}
