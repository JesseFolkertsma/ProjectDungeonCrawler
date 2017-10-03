using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NWPlayerCombat : NetworkBehaviour {

    public Inventory inventory;
    public bool isActive = true;

    private void Update()
    {
        if (!isActive)
            return;

        CheckInput();
    }

    void CheckInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {

        }
    }
}
