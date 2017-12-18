using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {


    public static GameManager instance;

    [Header("Game Data")]
    [Tooltip("The layers that the player can hit with attacks")]
    public LayerMask hitableLayers;
    public PoolableObject[] poolableObjects;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject SpawnObject(string objectName, Vector3 position, Quaternion rotation)
    {
        PoolableObject pObject = null;
        foreach(PoolableObject po in poolableObjects)
        {
            if(po.objectName == objectName)
            {
                pObject = po;
                break;
            }
        }
        if(pObject == null)
        {
            Debug.LogError("PoolableObject " + "'" + objectName + "'" + " does not exist");
            return null;
        }
        else
        {
            if(pObject.objectsInScene.Count >= 50)
            {
                return pObject.objectsInScene.Dequeue();
            }
            else
            {
                GameObject newObject = Instantiate(pObject.poolObject, position, rotation);
                pObject.objectsInScene.Enqueue(newObject);
                return newObject;
            }
        }
    }
}

[System.Serializable]
public class PoolableObject
{
    public string objectName;
    public GameObject poolObject;
    public int maxObjects;
    [HideInInspector]
    public Queue<GameObject> objectsInScene;
}