﻿using System.Collections;
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
        foreach (PoolableObject po in poolableObjects)
        {
            if (po.objectName == objectName)
            {
                pObject = po;
                break;
            }
        }
        if (pObject == null)
        {
            Debug.LogWarning("PoolableObject " + "'" + objectName + "'" + " does not exist");
            return null;
        }
        else
        {
            if (pObject.objectsInScene.Count >= pObject.maxObjects)
            {
                GameObject newObject = pObject.objectsInScene.Peek();
                pObject.objectsInScene.Enqueue(pObject.objectsInScene.Dequeue());
                newObject.transform.position = position;
                newObject.transform.rotation = rotation;
                if (pObject.isParticle)
                {
                    newObject.GetComponent<PoolableParticle>().Activate();
                }
                return newObject;
            }
            else
            {
                GameObject newObject = Instantiate(pObject.poolObject, position, rotation);
                pObject.objectsInScene.Enqueue(newObject);
                return newObject;
            }
        }
    }

    public GameObject SpawnSound(AudioClip sound, float volume,Vector3 position, Quaternion rotation)
    {
        string objectName = "SoundObject";
        PoolableObject pObject = null;
        foreach (PoolableObject po in poolableObjects)
        {
            if (po.objectName == objectName)
            {
                pObject = po;
                break;
            }
        }
        if (pObject == null)
        {
            Debug.LogWarning("PoolableObject " + "'" + objectName + "'" + " does not exist");
            return null;
        }
        else
        {
            if (pObject.objectsInScene.Count >= pObject.maxObjects)
            {
                GameObject newObject = pObject.objectsInScene.Peek();
                pObject.objectsInScene.Enqueue(pObject.objectsInScene.Dequeue());
                newObject.transform.position = position;
                newObject.transform.rotation = rotation;
                if (pObject.isParticle)
                {
                    newObject.GetComponent<PoolableParticle>().Activate();
                }
                AudioSource aSource = newObject.GetComponent<AudioSource>();
                aSource.volume = volume;
                aSource.clip = sound;
                aSource.Play();
                return newObject;
            }
            else
            {
                GameObject newObject = Instantiate(pObject.poolObject, position, rotation);
                pObject.objectsInScene.Enqueue(newObject);
                AudioSource aSource = newObject.GetComponent<AudioSource>();
                aSource.volume = volume;
                aSource.clip = sound;
                aSource.Play();
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
    public bool isParticle;
    [HideInInspector]
    public Queue<GameObject> objectsInScene = new Queue<GameObject>();
}