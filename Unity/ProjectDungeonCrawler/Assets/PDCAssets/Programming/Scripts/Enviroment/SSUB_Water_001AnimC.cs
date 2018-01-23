using UnityEngine;
using System.Collections;

public class SSUB_Water_001AnimC : MonoBehaviour
{

    public ProceduralMaterial substance;
    public float flow = 1;
    public float speed = 5f;
    public float valueMax;

    float curSpeed;
    bool backwards = false;

    void Start()
    {
        curSpeed = speed;
        //substance = GetComponent<Renderer>().sharedMaterial as ProceduralMaterial;
    }

    void LateUpdate()
    {
        flow = flow + (curSpeed* Time.smoothDeltaTime);
        if (flow >= valueMax - 1) flow = 0;
        substance.SetProceduralFloat("Waterfall Pattern Flow", flow);
        //substance.SetProceduralFloat("Water Pattern 2 Disorder", flow);
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