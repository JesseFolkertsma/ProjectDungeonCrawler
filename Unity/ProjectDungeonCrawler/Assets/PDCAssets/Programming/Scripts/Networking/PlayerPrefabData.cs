using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerPrefabData : NetworkBehaviour {

    const string REMOTE_PLAYER_LAYER = "RemotePlayer";

    //Private serializable variables
    public GameObject[] goToDisable;
    public Behaviour[] compToDisable;

    public virtual void Init(bool isLocal)
    {
        if (!isLocal)
        {
            DisableComponents();
            SetAsRemotePlayer();
        }
        else
        {
            DisableGameObjects();
        }
    }

    void DisableGameObjects()
    {
        foreach (GameObject go in goToDisable)
        {
            go.SetActive(false);
        }
    }

    void DisableComponents()
    {
        for (int i = 0; i < compToDisable.Length; i++)
        {
            Debug.Log("Is a me: " + gameObject.name + " yes and i will disable component: " + compToDisable[i].GetType());
            compToDisable[i].enabled = false;
        }
    }

    void SetAsRemotePlayer()
    {
        StaticFunctions.SetLayerRecursively(transform, REMOTE_PLAYER_LAYER);
    }
}
