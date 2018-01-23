using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedBillboarding : MonoBehaviour {

    Transform target;

    public void SetupForClient(NWPlayerCombat _target)
    {
        target = _target.transform;
    }

    private void Update()
    {
        if(target != null)
        {
            Vector3 lookat = new Vector3(transform.position.x, target.position.y, transform.position.z);
            transform.rotation.SetLookRotation(lookat);
        }
    }
}
