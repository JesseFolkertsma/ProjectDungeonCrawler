using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PDC.Saving;
using PDC.Characters;
using PDC.Weapons;
using PDC.NPCS;
using PDC.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerData gameData;

    public GameObject untaggedHitdecal;
    public GameObject woodHitdecal;
    public GameObject stoneHitdecal;
    public GameObject fleshHitdecal;

    public delegate void OnSceneExit();
    public OnSceneExit onSceneExit;

    public int vuileviezeint;
    public bool dungeonClear = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Exit Dungeon"))
        {
            LoadScene(0);
        }
    }

    public void LoadScene(int sceneID)
    {
        if (onSceneExit != null)
            onSceneExit();
        SceneManager.LoadScene(sceneID);
    }

    //public void GatherGameData()
    //{
    //    print("SAVE");
    //    gameData = new PlayerData();
    //    gameData.playerStats = PlayerCombat.instance.characterStats;
    //    foreach(Weapon w in PlayerCombat.instance.weapons)
    //    {
    //        if(w != null)
    //            gameData.playerWeapons.Add(w.weaponID);
    //    }
    //    gameData.playerConsumables = PlayerCombat.instance.consumables;
    //}

    //public void LoadGameData()
    //{
    //    if(gameData != null)
    //    {
    //        PlayerCombat.instance.characterStats = gameData.playerStats;
    //        PlayerCombat.instance.consumables = gameData.playerConsumables;
    //        int[] weapons = gameData.playerWeapons.ToArray();
    //        foreach(int w in weapons)
    //        {
    //            GameObject newWep = Instantiate(WeaponDatabase.instace.GetWeaponByID(w));
    //            Weapon newWepComp = newWep.GetComponent<Weapon>();
    //            PlayerCombat.instance.PickupWeapon(newWepComp);
    //        }
    //    }
    //}

    /// <summary>
    /// Spawn Decal for given tag at given position and rotation
    /// </summary>
    /// <param name="tag">Tag of object hit</param>
    /// <param name="position">Decal position</param>
    /// <param name="rotation">Decal rotation</param>
    /// <returns></returns>
    public GameObject SpawnDecal(string tag, Vector3 position, Quaternion rotation)
    {
        switch (tag)
        {
            case "Untagged":
                return Instantiate(untaggedHitdecal, position, rotation);
            case "Wood":
                return Instantiate(woodHitdecal, position, rotation);
            case "Stone":
                return Instantiate(stoneHitdecal, position, rotation);
            case "Flesh":
                return Instantiate(fleshHitdecal, position, rotation);
        }
        return Instantiate(untaggedHitdecal, position, rotation);
    }

    /// <summary>
    /// Spawn Decal for given tag with position, rotation and parent
    /// </summary>
    /// <param name="tag">Tag of object hit</param>
    /// <param name="position">Decal position</param>
    /// <param name="rotation">Decal rotation</param>
    /// <param name="parent">Decal parent</param>
    public GameObject SpawnDecal(string tag, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject go = SpawnDecal(tag, position, rotation);
        go.transform.parent = parent;
        return go;
    }

    public delegate void OnAnimationEnd();
    public void CheckWhenAnimationTagEnds(Animator anim, string tagName, OnAnimationEnd effectAfterEnd)
    {
        StartCoroutine(AnimatorCheckRoutine(anim, tagName, effectAfterEnd));
    }

    IEnumerator AnimatorCheckRoutine(Animator anim, string tagName, OnAnimationEnd effectAfterEnd)
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsTag(tagName))
        {
            yield return new WaitForEndOfFrame();
        }
        while (anim.GetCurrentAnimatorStateInfo(0).IsTag(tagName))
        {
            yield return new WaitForEndOfFrame();
        }
        print("Animation: " + tagName + " has ended.");
        effectAfterEnd();
    }

    public void StartCoversation(NPC npc)
    {
        UIQuestGiver.instance.ActivateQuest(npc);
    }
}
