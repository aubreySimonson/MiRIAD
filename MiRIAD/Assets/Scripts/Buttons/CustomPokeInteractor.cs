using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; 


///<summary>
///Part of MiRIAD, an authoring tool for digital twins
///The XRTK Poke Interactor system is being annoying. 
///This goes on buttons and makes them actually work when you touch them. 
///???-->followspotfour@gmail.com
///</summary>

public class CustomPokeInteractor : MonoBehaviour
{
    public UnityEvent onClick;
    public AudioSource buttonSound;
    public bool isButtonDown;
    public float buttonDelay;//how long to delay before button can be pressed again

    public Color unselectedColor;
    public Color selectedColor;

     void Start()
    {
        if (onClick == null)
            onClick = new UnityEvent();

        onClick.AddListener(OnClick);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Poke" && !isButtonDown){
            onClick.Invoke();
        }
    }

    void OnClick(){
        isButtonDown = true;
        buttonSound.Play();
        gameObject.GetComponent<Image>().color = selectedColor;
        StartCoroutine(ResetButton());
    }

    IEnumerator ResetButton()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(buttonDelay);
        isButtonDown = false;
        gameObject.GetComponent<Image>().color = unselectedColor;
    }
}
