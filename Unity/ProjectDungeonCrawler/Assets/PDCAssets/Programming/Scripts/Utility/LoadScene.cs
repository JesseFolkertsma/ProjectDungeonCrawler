using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour {

    public string sceneName;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadSceneNow()
    {

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
