using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


///<summary>
///Part of Untitled Digital Twin Authoring Tool
///This script goes on menus where grabbing an object and removing it from the menu makes it a persistent thing.
///</summary>

public class GeneratorMenu : MonoBehaviour
{
    public GameObject collectorObject;//stuff taken from the menu become children of this.
    public Collider menuCollider;
    public List<GameObject> menuItems;

    public List<GameObject> itemsRemovedFromList;
    public Text debugText;

    // Start is called before the first frame update
    void Start()
    {
      if(menuCollider==null){
        menuCollider = gameObject.GetComponent<Collider>();
      }
      collectorObject.SetActive(true);//prevents menu items from disapearing when selected if we accidentally turned this off
      debugText = GameObject.Find("Debug").GetComponent<Text>();//so fucking fragile and inefficient. Find calls bad. 
    }

    void OnTriggerExit(Collider other){
      if(menuItems.Contains(other.gameObject)){
        //the following line is what is causing the problem, so just doing it in a more complicated way should fix the problem, right?
        //other.gameObject.transform.parent = collectorObject.transform;//works in desktop, not on headset...
        itemsRemovedFromList.Add(other.gameObject);
      }
    }
}
