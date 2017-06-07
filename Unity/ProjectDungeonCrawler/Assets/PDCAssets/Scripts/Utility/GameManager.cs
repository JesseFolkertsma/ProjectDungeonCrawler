using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
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
