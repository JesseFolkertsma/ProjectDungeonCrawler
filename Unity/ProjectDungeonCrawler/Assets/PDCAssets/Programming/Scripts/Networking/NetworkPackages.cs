using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPackages {
    [System.Serializable]
    public class DamagePackage
    {
        public float damage;
        public string hitter;

        public DamagePackage(float _damage, string _hitter)
        {
            damage = _damage;
            hitter = _hitter;
        }

        public DamagePackage() { }
    }
}
