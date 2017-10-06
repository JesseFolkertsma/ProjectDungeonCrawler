using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class Inventory : NetworkBehaviour {
    public List<EquippedWeapon> weapons;
    public EquippedWeapon equippedWeapon;

    public int availableSlots = 3;
}

public class EquippedWeapon
{
    public WeaponInstance instance;
    public WeaponVisuals visual;
}
