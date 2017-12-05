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
        public Vector3 hitPosition;

        public DamagePackage(byte _damage, string _hitter, string _hitterID, Vector3 _hitPos)
        {
            damage = _damage;
            hitter = _hitter;
            hitterID = _hitterID;
            hitPosition = _hitPos;
        }

        public DamagePackage() { }
    }
}
