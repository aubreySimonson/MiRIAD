using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Part of MiRIAD, an authoring tool for digital twins
///
///This script contains a simple helper function for the 'see more' button of the menu
///???-->followspotfour@gmail.com
///</summary>
public class ToggleThis : MonoBehaviour
{
    public GameObject thingToToggle;
    public GeneratorMenu generatorMenu;

    public void Toggle(){
      AddRemovedItemsToCollector();
      thingToToggle.SetActive(!thingToToggle.activeSelf);

    }

    //this is an annoying workaround to deal with the way that grabbing things works in the XRTK
    public void AddRemovedItemsToCollector(){
      if(generatorMenu!=null){
        foreach(GameObject gO in generatorMenu.itemsRemovedFromList){
          gO.transform.parent = generatorMenu.collectorObject.transform;
          generatorMenu.itemsRemovedFromList.Remove(gO);
        }
      }
    }
}
