using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponData {
    public string weaponName;
    public string description;
    public float damage;
    public float range;
    public int maxAmmo;
    public float attackRate;
    public AttackType attackType;

    public string prefabName;

    public WeaponData() { }

    public WeaponData(string _name, string _desc, float _dmg, float _range, int _maxAmmo, float _aRate, AttackType _aType, string _prefab)
    {
        weaponName = _name;
        description = _desc;
        damage = _dmg;
        range = _range;
        maxAmmo = _maxAmmo;
        attackRate = _aRate;
        attackType = _aType;
        prefabName = _prefab;
    }
}
