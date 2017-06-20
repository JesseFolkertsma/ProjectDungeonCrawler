using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour {

    public GameFlow instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
}
