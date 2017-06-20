using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour {

    public float speed = 2;

	public void Move(List<Vector2> list)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(_Move(list));
    }

    private Coroutine moveRoutine;
    private IEnumerator _Move(List<Vector2> list)
    {
        //for testing purposes
        for(int i = 0; i < list.Count; i++)
        {
            transform.position = list[i];
            yield return new WaitForSeconds(0.01f);
        }
    }
}
