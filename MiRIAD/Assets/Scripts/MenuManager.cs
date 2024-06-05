using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Works with Menu Removal Detector to replace Generator Menu
///
/// </summary>
public class MenuManager : MonoBehaviour
{
    public List<GameObject> itemsRemovedFromList;
    public GameObject collectorObject;
    public List<GameObject> menuItems;

    void Start(){
        collectorObject.SetActive(true);//prevents menu items from disapearing when selected if we accidentally turned this off
    }

    //we do this *when the edit menu gets closed* to avoid parenting issues from holding objects when they're removed
    public void RemoveAllItemsFromList(){
        foreach(GameObject gO in itemsRemovedFromList){
          gO.transform.parent = collectorObject.transform;
          itemsRemovedFromList.Remove(gO);
        }
    }
}
