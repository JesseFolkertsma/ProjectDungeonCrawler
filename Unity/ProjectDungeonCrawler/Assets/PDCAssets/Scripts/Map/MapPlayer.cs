using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour {

	public void Move(List<Vector2> list)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(_Move(list));
    }

    private Coroutine moveRoutine;
    private IEnumerator _Move(List<Vector2> list)
    {
        yield break;
    }
}
