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
        public float fadeDuration = 0;

        public Sprite questSprite;
        public GameObject dungeon;
        public List<NPC> smerigehardcodeOrder;

        Coroutine routine;
        NPC npc;

        float CurrentTypeSpeed
        {
            get
            {
                if (Input.GetButton("Jump")) return typeRate * skipConvAcceleration;
                else return typeRate;
            }
        }


        private void Start()
        {
            if(instance == null)
                instance = this;
            audioS = GetComponent<AudioSource>();

            if(GameManager.instance.vuileviezeint > 0)
            {
                dungeon.SetActive(true);
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if(Input.GetButtonDown("Fire3"))
                dungeon.SetActive(true);
        }

        public void ViezeHardCode()
        {
            ActivateQuest(smerigehardcodeOrder[GameManager.instance.vuileviezeint]);
            switch (GameManager.instance.vuileviezeint)
            {
                case 0:
                    GameManager.instance.vuileviezeint++;
                    break;
            }
        }
        public void ViezeHardCode2()
        {
            GameManager.instance.LoadScene(2);
        }

        /// <summary>
        /// Play entire conversation
        /// </summary>
        /// <param name="_npc">The NPC to play the conversation from</param>
        public void ActivateQuest(NPC _npc)
        {
            if (routine == null)
            {
                npc = _npc;
                image.sprite = _npc.npcSprite;
                routine = StartCoroutine(MoveIntoScreen());
            }
        }

        IEnumerator MoveIntoScreen()
        {
            image.color = Color.black;
            text.text = "";
            yield return FadeNPCInOrOut(true);
            //Loop each sentance
            foreach (NPCSentance s in npc.conversation)
            {
                yield return HandleSentance(s);
            }
            yield return FadeNPCInOrOut(false);
            routine = null;
            dungeon.SetActive(true);
        }

        IEnumerator HandleSentance(NPCSentance sentance)
        {
            HandleSentanceAction(sentance);
            PlayRandomTalkSound();
            //Type out each char
            foreach (char c in sentance.sentance)
            {
                text.text += c;
                //Play talk sound when we reached a space
                if (c == ' ')
                {
                    PlayRandomTalkSound();
                }
                yield return new WaitForSeconds(1 / CurrentTypeSpeed);
            }
            //Press jump when sentence is finished
            bool next = false;
            while (!next)
            {
                if (Input.GetButtonDown("Jump"))
                    next = true;
                yield return null;
            }
            text.text = "";
        }

        IEnumerator FadeNPCInOrOut(bool b)
        {
            float fade = 0;
            if (b)
            {
                //Move NPC into screen
                while (transform.localPosition.y < -.05f)
                {
                    transform.localPosition = Vector2.MoveTowards(transform.localPosition, Vector2.zero, moveSpeed * Time.deltaTime);
                    yield return null;
                }
                //Fade NPC from black
                while (!image.color.Equals(Color.white))
                {
                    image.color = Color.Lerp(image.color, Color.white, fade);
                    fade += Time.deltaTime * fadeDuration;
                    yield return null;
                }
            }
            else
            {
                //Fade color back to black
                while (!image.color.Equals(Color.black))
                {
                    image.color = Color.Lerp(image.color, Color.black, fade);
                    fade += Time.deltaTime * fadeDuration;
                    yield return null;
                }
                //Move NPC out of screen
                while (transform.localPosition.y > -379f)
                {
                    transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(0, -380), moveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
        }

        void HandleSentanceAction(NPCSentance s)
        {
            switch (s.action)
            {
                case NPCAction.GiveWeapon:
                    GameManager.instance.ObtainWeaponSound();
                    GameManager.instance.gameData.TryAssignWeapon(s.weapon.weaponID);
                    MapPopup.instance.DisplayPopup(s.weapon.weaponIcon, "Obtained " + s.weapon.weaponName + "!");
                    break;
                case NPCAction.AddQuest:
                    GameManager.instance.ObtainQuestSound();
                    MapPopup.instance.DisplayPopup(questSprite, "Quest: " + s.questName);
                    break;

            }
        }

        void PlayRandomTalkSound()
        {
            audioS.clip = talkClips[UnityEngine.Random.Range(0, talkClips.Length)];
            audioS.Play();
        }
    }
}
