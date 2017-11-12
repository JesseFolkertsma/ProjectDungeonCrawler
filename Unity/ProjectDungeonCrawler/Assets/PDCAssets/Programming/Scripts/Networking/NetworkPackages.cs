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
        public string hitterID;

        public DamagePackage(float _damage, string _hitter, string _hitterID)
        {
            damage = _damage;
            hitter = _hitter;
            hitterID = _hitterID;
        }

        public DamagePackage() { }
    }
}
