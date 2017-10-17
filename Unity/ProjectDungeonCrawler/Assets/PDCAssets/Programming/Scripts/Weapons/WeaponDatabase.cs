using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour {

    Dictionary<int, WeaponData> database;

    public static WeaponDatabase instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        database = new Dictionary<int, WeaponData>();
        database.Add(1, new WeaponData("Revolver", "Nice pewpew", 25f, 100f, 6, 2.5f, AttackType.Raycast, "Revolver_Prefab"));
    }

    public WeaponData GetWeapon(int id)
    {
        if (database.ContainsKey(id))
        {
            return database[id];
        }
        return null;
    }

    public GameObject GetWeaponPrefab(WeaponData data)
    {
        GameObject prefab = Resources.Load(data.prefabName) as GameObject;
        if(prefab == null)
        {
            Debug.Log(data.prefabName + " DOES NOT EXIST!");
        }
        return prefab;
    }

    public GameObject GetWeaponPrefab(int id)
    {
        GameObject prefab = Resources.Load(GetWeapon(id).prefabName) as GameObject;
        if (prefab == null)
        {
            Debug.Log(GetWeapon(id).prefabName + " DOES NOT EXIST!");
        }
        return prefab;
    }
}
