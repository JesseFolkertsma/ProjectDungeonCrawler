using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IHitable {
    string objectName { get; }
    NetworkConnection networkConn { get; }
    NetworkInstanceId networkID { get; }
    void GetHit(NetworkPackages.DamagePackage dmgPck);
}
