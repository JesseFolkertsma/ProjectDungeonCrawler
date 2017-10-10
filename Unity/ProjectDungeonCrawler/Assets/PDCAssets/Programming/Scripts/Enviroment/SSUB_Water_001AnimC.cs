using UnityEngine;
using System.Collections;

public class SSUB_Water_001AnimC : MonoBehaviour
{

    public ProceduralMaterial substance;
    public float flow = 1;
    public float speed = 5f;

    float curSpeed;
    bool backwards = false;

    void Start()
    {
        curSpeed = speed;
        substance = GetComponent<Renderer>().sharedMaterial as ProceduralMaterial;
    }

    void LateUpdate()
    {
        flow = flow + (curSpeed* Time.smoothDeltaTime);
        if (flow >= 2000 || flow <= 0) Flip()   ;
        substance.SetProceduralFloat("Water Pattern 2 Disorder", flow);
        substance.SetProceduralFloat("Water Pattern 1 Disorder", flow);
        substance.RebuildTextures();
    }

    void Flip()
    {
        if (backwards)
        {
            backwards = false;
            flow = 1;
            curSpeed = speed;
        }
        else
        {
            backwards = true;
            flow = 1999;
            curSpeed = -speed;
        }
    }
}