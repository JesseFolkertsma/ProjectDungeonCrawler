using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobComponent : MonoBehaviour {

    bool isSetup = false;

    public Transform camHolder;
    [SerializeField] float duckSpeed;
    Vector3 camHolderPos;
    Camera cam;

    float yOffset = 0f;
    float xOffset = 0f;
    bool goUp;

    public void SetupHeadBob(Camera _cam)
    {
        cam = _cam;
        camHolder = cam.transform.parent;
        isSetup = true;
        camHolderPos = camHolder.localPosition;
    }

    public void DuckHead(Vector3 _offset)
    {
        StopAllCoroutines();
        StartCoroutine(DuckHeadRoutine(_offset));
        //Vector3 _newCamPos = camHolder.localPosition;
        //_newCamPos.y -= 5;
        //camPos = _newCamPos;
    }

    public void LiftHead()
    {
        StopAllCoroutines();
        StartCoroutine(LiftHeadRoutine());
    }

    IEnumerator DuckHeadRoutine(Vector3 _offset)
    {
        while (true)
        {
            Vector3 _newPos = Vector3.zero;
            _newPos -= _offset;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, _newPos, duckSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator LiftHeadRoutine()
    {
        while (true)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, Vector3.zero, duckSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

  public void BobHead(float _bobHeight, float _bobSpeed, bool isFixedDeltatime)
    {
        if (isSetup)
        {
            //Check what timescale to use
            float _timeScale = Time.deltaTime;
            if (isFixedDeltatime)
                _timeScale = Time.fixedDeltaTime;

            //Calculate bob position
            if (goUp)
            {
                yOffset = camHolderPos.y + _bobHeight;
                if (camHolder.transform.localPosition.y >= yOffset - .01)
                {
                    goUp = false;
                }
            }
            else
            {
                yOffset = camHolderPos.y - _bobHeight;
                if (camHolder.transform.localPosition.y <= yOffset + .01)
                {
                    goUp = true;
                    xOffset = camHolderPos.x + Random.Range(-.1f, .1f);
                }
            }

            //Apply bob position
            Vector3 newPos = new Vector3(xOffset, yOffset, camHolderPos.z);
            camHolder.transform.localPosition = Vector3.MoveTowards(camHolder.transform.localPosition, newPos, _timeScale * _bobSpeed);
        }
    }

    public void JumpBob(float strenght)
    {

    }
}
