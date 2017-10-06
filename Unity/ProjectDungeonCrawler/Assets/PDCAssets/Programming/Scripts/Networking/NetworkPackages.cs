using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NetworkPackages {
    [System.Serializable]
    public class DamagePackage
    {
        public float damage;

        public DamagePackage(float _damage)
        {
            damage = _damage;
        }

        public DamagePackage() { }
    }
}
