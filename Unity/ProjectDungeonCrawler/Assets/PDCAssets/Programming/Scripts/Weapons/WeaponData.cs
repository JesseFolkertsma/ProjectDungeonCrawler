using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData {
    public string weaponName;
    public string description;
    public int damage;
    public float range;
    public float spread;
    public int maxAmmo;
    public int currentAmmo;
    public float attackRate;
    public AttackType attackType;
    public bool canHoldMouseDown;
    public bool canZoom;

    public WeaponData() { }

    public WeaponData(string _name, string _desc, int _dmg, float _range, int _maxAmmo, float _aRate, AttackType _aType, float _spread, bool _canHoldMouseDown = false)
    {
        weaponName = _name;
        description = _desc;
        damage = _dmg;
        range = _range;
        maxAmmo = _maxAmmo;
        attackRate = _aRate;
        attackType = _aType;
        canHoldMouseDown = _canHoldMouseDown;
        spread = _spread;
    }
}
