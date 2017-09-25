using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Weapons;

public class WeaponDatabase : MonoBehaviour {

    public static WeaponDatabase instace;

    private void Awake()
    {
        if(instace == null)
        {
            instace = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<WeaponInDatabase> dataBase;

    public GameObject GetWeaponByID(int id)
    {
        GameObject wep = null;

        foreach(WeaponInDatabase wid in dataBase)
        {
            if (wid.IsID(id))
            {
                wep = wid.weaponPrefab;
                break;
            }
        }

        return wep;
    }
}

[System.Serializable]
public class WeaponInDatabase
{
    public int id;
    public GameObject weaponPrefab;

    public string Name
    {
        get
        {
            return weaponPrefab.GetComponent<Weapon>().weaponName;
        }
    }

    public bool IsID(int _id)
    {
        return (_id == id);
    }
}
