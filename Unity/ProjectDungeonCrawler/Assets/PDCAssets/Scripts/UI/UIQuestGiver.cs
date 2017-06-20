using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PDC.NPCS;
using UnityEngine.UI;

namespace PDC.UI
{
    public class UIQuestGiver : MonoBehaviour
    {
        public static UIQuestGiver instance;

        public AudioSource audioS;
        public AudioClip[] talkClips; 
        public Image image;
        public Text text;
        public float typeRate = 1;
        public float skipConvAcceleration = 2;
        public float moveSpeed = 250;
        public float fadeDuration = 2;

        float CurrentTypeSpeed
        {
            get
            {
                if (Input.GetButton("Jump")) return typeRate * skipConvAcceleration;
                else return typeRate;
            }
        }

        NPC npc;

        private void Start()
        {
            if(instance == null)
                instance = this;
            audioS = GetComponent<AudioSource>();
        }

        public void ActivateQuest(NPC _npc)
        {
            npc = _npc;
            image.sprite = _npc.npcSprite;
            image.color = Color.black;
            text.text = "";
            StartCoroutine(MoveIntoScreen());
        }

        IEnumerator MoveIntoScreen()
        {
            while (transform.localPosition.y < -.05f)
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, moveSpeed * Time.deltaTime);
                yield return null;
            }
            print("Moved in");
            //image.CrossFadeColor(Color.white, fadeDuration, false, false);
            float elapsedTime = 0f;
            while (!image.color.Equals(Color.white))
            {
                image.color = Color.Lerp(image.color, Color.white, elapsedTime/fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            print("faded in");
            foreach (string s in npc.conversation)
            {
                print("talking: " + s);
                PlayRandomTalkSound();
                foreach (char c in s)
                {
                    text.text += c;
                    if (c == ' ')
                    {
                        PlayRandomTalkSound();
                    }
                    yield return new WaitForSeconds(1 / CurrentTypeSpeed);
                }
                print("Finised sentance");
                bool next = false;
                while (!next)
                {
                    if (Input.GetButtonDown("Jump"))
                        next = true;
                    yield return null;
                }
                text.text = "";
            }
            //image.CrossFadeColor(Color.black, fadeDuration, false, false);
            float elapsedTime2 = 0f;
            while (!image.color.Equals(Color.black))
            {
                image.color = Color.Lerp(image.color, Color.black, elapsedTime2/fadeDuration);
                elapsedTime2 += Time.deltaTime;
                yield return null;
            }
            while (transform.localPosition.y > -379f)
            {
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(0, -380), moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        void PlayRandomTalkSound()
        {
            audioS.clip = talkClips[UnityEngine.Random.Range(0, talkClips.Length)];
            audioS.Play();
        }
    }
}
