using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPopup : MonoBehaviour
{

    public static MapPopup instance;

    public float speed = 1;
    public float popupSec = 2;
    public Image image;
    public Text text;

    struct Popup
    {
        public string message;
        public Sprite sprite;

        public Popup(string _message, Sprite _sprite)
        {
            sprite = _sprite;
            message = _message;
        }
    }
    Queue<Popup> waiting = new Queue<Popup>();
    RectTransform myRect;
    Coroutine routine;

    private void Awake()
    {
        instance = this;
        myRect = GetComponent<RectTransform>();
    }

    public void DisplayPopup(Sprite sprite, string message)
    {
        if (routine == null)
        {
            routine = StartCoroutine(Move(new Popup(message, sprite)));
        }
        else
        {
            waiting.Enqueue(new Popup(message, sprite));
        }
    }

    IEnumerator Move(Popup pop)
    {
        image.sprite = pop.sprite;
        text.text = pop.message;
        while (myRect.localPosition.x < 225)
        {
            myRect.localPosition = Vector3.MoveTowards(myRect.localPosition, new Vector2(226, 0), speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(popupSec);
        while (myRect.localPosition.x > 0)
        {
            myRect.localPosition = Vector3.MoveTowards(myRect.localPosition, new Vector2(-1, 0), speed * Time.deltaTime);
            yield return null;
        }
        if (waiting.Count > 0)
        {
            StartCoroutine(Move(waiting.Dequeue()));
        }
    }
}

