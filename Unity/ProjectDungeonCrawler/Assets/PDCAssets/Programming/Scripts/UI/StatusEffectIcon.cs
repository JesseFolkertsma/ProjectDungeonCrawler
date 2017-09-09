using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PDC.StatusEffects;

namespace PDC.UI
{
    public class StatusEffectIcon : MonoBehaviour
    {

        [SerializeField] Image mainImage;
        [SerializeField] Image loadImage;
        [SerializeField] Image fillImage;

        float startTime;
        float endTime;
        float duration;

        public void SetupIcon(StatusEffect effect)
        {
            print(effect.name);
            if(effect.effectIcon != null)
                mainImage.sprite = effect.effectIcon;

            startTime = Time.time;
            endTime = Time.time + effect.effectDuration;
            duration = effect.effectDuration;
            StartCoroutine(Timer());
        }

        IEnumerator Timer()
        {
            while(Time.time < endTime)
            {
                fillImage.fillAmount = 1 - ((Time.time - startTime) / duration);
                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }
    }
}
