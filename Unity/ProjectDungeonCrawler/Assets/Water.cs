using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour {
    Material myMaterial;
    public float time;
    public float value;
    public float valueRate;

    public bool water;

    public void Awake()
    {
        myMaterial = transform.GetComponent<Renderer>().material;
        if(myMaterial == null)
        {
            print("Hary");
        }
    }
    public void Start()
    {
        StartCoroutine(Change());
    }
    public IEnumerator Change() {
        //Change value
        while (water) {
            if (value >= 100) {
               value = 0;
            }
            else
            {
                value += valueRate;
                myMaterial.SetFloat("Water_Pattern_1_Disorder", value);
            }
            print("Loopdiloop");
            yield return new WaitForSeconds(time);
        }
    }
}
