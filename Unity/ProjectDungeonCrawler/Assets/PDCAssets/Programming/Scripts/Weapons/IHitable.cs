﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IHitable {
    string objectID { get; }
    string objectName { get; }
    NetworkInstanceId networkID { get; }
    [ClientRpc]
    void RpcGetHit(NetworkPackages.DamagePackage dmgPck);
}
