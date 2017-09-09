using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePopup : MonoBehaviour {

    public static GamePopup instance;

    public float speed = 1;
    public float popupSec = 2;
    public Text text;
    Queue<string> waiting = new Queue<string>();
    RectTransform myRect;
    Coroutine routine;

    private void Awake()
    {
        instance = this;
        myRect = GetComponent<RectTransform>();
    }

    public void DisplayPopup(string message)
    {
        if(routine == null)
        {
            routine = StartCoroutine(Move(message));
        }
        else
        {
            waiting.Enqueue(message);
        }
    }

    IEnumerator Move(string pop)
    {
        text.text = pop;
        while(myRect.localPosition.y > -70)
        {
            myRect.localPosition = Vector3.MoveTowards(myRect.localPosition, new Vector2(0, -71), speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(popupSec);
        while (myRect.localPosition.y < 0)
        {
            myRect.localPosition = Vector3.MoveTowards(myRect.localPosition, new Vector2(0, 1), speed * Time.deltaTime);
            yield return null;
        }
        if(waiting.Count > 0)
        {
            StartCoroutine(Move(waiting.Dequeue()));
        }
    }
}
