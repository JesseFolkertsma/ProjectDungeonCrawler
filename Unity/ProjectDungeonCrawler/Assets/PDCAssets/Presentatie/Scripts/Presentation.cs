using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Presentation : MonoBehaviour {
    public Transform[] slides;
    public Animator[] slidesAnim;
    public List<Animator> anims;

    public List<int> animHistory;

    public int currentAnim;
    public int nextAnim;
    public int currentSlide;
    public int previousSlide;

    private void Awake() {
        GetSlides();
        GetAnims();
        slidesAnim[0].SetBool("open", true);
    }
    private void Update() {
        Controls();
    }
    private void Controls() {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            SlideChange(false);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            SlideChange(true);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ActivateAnim(true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ActivateAnim(false);
        }
    }

    private void GetSlides() {
        List<Transform> temp = new List<Transform>();
        List<Animator> temp2 = new List<Animator>();
        foreach(Transform child in transform) { 
            if (child.name[0] == "*"[0]) {
                temp.Add(child);
                temp2.Add(child.GetComponent<Animator>());
                print("Succed pass");
            }
            print("fail pass");
        }
        slides = temp.ToArray();
        slidesAnim = temp2.ToArray();
        animHistory = new List<int>(slides.Length);

    }
    private void GetAnims() {
        anims.Clear();
        if (animHistory.Count > currentSlide) {
            currentAnim = animHistory[currentSlide];
            
        }
        else {
            currentAnim = 0;
        }

        foreach(Animator anim in slides[currentSlide].GetComponentsInChildren<Animator>()){
            anims.Add(anim);
        }
        anims.RemoveAt(0);
        
    }
    public void SlideChange(bool next) {
        if (slides.Length != 0) {
            if (next && currentSlide < slides.Length - 1) {
                slidesAnim[currentSlide].SetBool("open", false);
                previousSlide = currentSlide;
                currentSlide++;
                slidesAnim[currentSlide].SetBool("open", true);
            }
            else if(!next && currentSlide >= 1) {
                if (currentSlide >= animHistory.Count) {
                    animHistory.Add(currentAnim);
                }
                slidesAnim[currentSlide].SetBool("open", false);
                previousSlide = currentSlide;
                currentSlide--;
                slidesAnim[currentSlide].SetBool("open", true);
            }
            else {
                print("No option");
                return;

            }

            if (currentSlide >= animHistory.Count) {
                animHistory.Add(currentAnim);
            }
            else {
                animHistory[previousSlide] = currentAnim;
            }
            GetAnims();

        }

    }
    public void ActivateAnim(bool next) {
        if (anims.Count != 0 ) {
            if (next && currentAnim < anims.Count) {
                anims[currentAnim].SetTrigger("go");
                currentAnim++;
            }
            else if(!next && currentAnim >= 1){
                currentAnim--;
                anims[currentAnim].SetTrigger("default");
            }


        }
    }
    public void LoadScene() {
        SceneManager.LoadScene("MainMenu");
    }
}
