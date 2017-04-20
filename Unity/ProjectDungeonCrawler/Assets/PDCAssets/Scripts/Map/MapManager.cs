using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour {
    [SerializeField]
    private RectTransform playerIcon;
    [Tooltip("The maximal distance when you can interact with an item on the map"), SerializeField]
    public float distanceToInteract;
    [SerializeField]
    private float speed;
    private Coroutine curCoroutine;
    public string loadLevelName = "GeneratorTest";

	public void Travel(RectTransform t)
    {
        if (curCoroutine != null)
            StopCoroutine(curCoroutine);
        curCoroutine = StartCoroutine(_Travel(t));
    }

    private IEnumerator _Travel(RectTransform t)
    {
        while(Vector2.Distance(playerIcon.position, t.position) > distanceToInteract){
            
            playerIcon.position = Vector3.MoveTowards(playerIcon.position, t.position, speed * Time.deltaTime);
            yield return null;
        }

        //nothing is happening with the place yet, so Im just going to load a random level
        SceneManager.LoadScene(loadLevelName);
    }
}
