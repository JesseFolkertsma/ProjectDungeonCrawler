using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData {
    public int id;
    public string weaponName;
    public string description;
    public float damage;
    public float range;
    public int maxAmmo;
    public AttackType attackType;
}

[System.Serializable]
public class WeaponInstance
{
    public WeaponData stats;
    public int currentAmmo;
}
