using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBlood : MonoBehaviour {
    Image image;
    AudioSource aSource;
    public static ScreenBlood instance;
    public float fadeInTime = .3f;
    public float fadeOutTime = 2f;
    public float stayInScreenTime = 2f;

    private void Awake()
    {
        aSource = GetComponent<AudioSource>();
        image = GetComponent<Image>();
        image.CrossFadeAlpha(0, .1f, true);
        instance = this;
    }

    public void GetHit()
    {
        aSource.Play();
        image.CrossFadeAlpha(1, fadeInTime, false);
        Invoke("FadeOut", stayInScreenTime);
    }

    void FadeOut()
    {
        image.CrossFadeAlpha(0, fadeOutTime, false);
    }
}
