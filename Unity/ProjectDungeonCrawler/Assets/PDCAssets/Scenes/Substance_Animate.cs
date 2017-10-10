using UnityEngine;
using System.Collections;

public class SSUB_Water_001AnimC : MonoBehaviour
{

    public ProceduralMaterial substance;
    public float flow = 1;

    void Start()
    {
        substance = GetComponent<Renderer>().sharedMaterial as ProceduralMaterial;
    }

    void LateUpdate()
    {
        flow = flow + (2 * Time.smoothDeltaTime);
        substance.SetProceduralFloat("Flow", flow);
        substance.RebuildTextures();
    }
}