using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Goes on menu items. Moves them to be a child of the collector object rather than the menu
/// if they are picked up and moved sufficiently far. 
/// Superceedes some of the functionality of Generator Menus.
///
/// </summary>

public class MenuRemovalDetector : MonoBehaviour
{
    private Vector3 startPosition;
    public float tolerance;//how far should the thing have to move before we call it "removed from the menu"?
    private bool removed = false;
    public Text debugText;
    public MenuManager menuManager;
    public GeneratorMenu generatorMenu;//for while we're still using both-- you should really clean this up eventually

    void Start(){
        debugText = GameObject.Find("Debug").GetComponent<Text>();//so fucking fragile and inefficient. Find calls bad. 
        SetStartPosition();
    }


    public void SetStartPosition(){
        startPosition = gameObject.transform.localPosition;
    }
 

    // Update is called once per frame
    void Update()
    {
        DetectMovement();
    }

    private void DetectMovement(){
        if(!removed){
            if(gameObject.transform.localPosition.x<startPosition.x-tolerance || gameObject.transform.localPosition.x>startPosition.x+tolerance){
                RemoveFromMenu();
            }
            if(gameObject.transform.localPosition.y<startPosition.y-tolerance || gameObject.transform.localPosition.y>startPosition.y+tolerance){
                RemoveFromMenu();
            }
            if(gameObject.transform.localPosition.z<startPosition.z-tolerance || gameObject.transform.localPosition.z>startPosition.z+tolerance){
                RemoveFromMenu();
            }
        }
    }

    private void RemoveFromMenu(){
        debugText.text = "removing item from menu";
        removed = true;
        //gameObject.transform.parent = collector.transform;//might not work because we're holding it
        menuManager.itemsRemovedFromList.Add(gameObject);
    }
}
