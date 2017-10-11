using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour {

    public AudioSource source;

	public void Play(AudioClip clip, float volume)
    {
        source.clip = clip;
        source.volume = volume;
        source.Play();
        StartCoroutine(WaitUntilDestroy(clip.length));
    }

    private IEnumerator WaitUntilDestroy(float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
