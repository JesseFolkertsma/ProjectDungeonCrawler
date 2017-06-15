using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.Saving;
using PDC.Characters;
using PDC.Weapons;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameData gameData;

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

    public void GatherGameData()
    {
        print("SAVE");
        gameData = new GameData();
        gameData.playerStats = PlayerCombat.instance.characterStats;
        foreach(Weapon w in PlayerCombat.instance.weapons)
        {
            if(w != null)
                gameData.playerWeapons.Add(w.weaponID);
        }
        gameData.playerConsumables = PlayerCombat.instance.consumables;
    }

    public void LoadGameData()
    {
        if(gameData != null)
        {
            PlayerCombat.instance.characterStats = gameData.playerStats;
            PlayerCombat.instance.consumables = gameData.playerConsumables;
            int[] weapons = gameData.playerWeapons.ToArray();
            foreach(int w in weapons)
            {
                GameObject newWep = Instantiate(WeaponDatabase.instace.GetWeaponByID(w));
                Weapon newWepComp = newWep.GetComponent<Weapon>();
                PlayerCombat.instance.PickupWeapon(newWepComp);
            }
        }
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
}
