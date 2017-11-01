using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class StaticFunctions {
    
    public static void SetLayerRecursively(Transform objectToSet, string layerName)
    {
        objectToSet.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform t in objectToSet)
        {
            SetLayerRecursively(t, layerName);
        }
    }
}
