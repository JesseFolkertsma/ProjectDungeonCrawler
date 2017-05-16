using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenEffectHandler : MonoBehaviour {

    public Material curMat;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (curMat != null)
            Graphics.Blit(src, dest, curMat);
    }
}
