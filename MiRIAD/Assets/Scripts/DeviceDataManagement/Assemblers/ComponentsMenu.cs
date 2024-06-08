using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assembles the menu of components.
/// Should go on top-level Create Components gameobject,
/// which is a parent of the relevant canvas.
///
/// It and DevicesMenu should probably both inherit from
/// an abstract class called "Add Nodes Menu" or something,
/// but you're going to leave that problem until it's harder to fix.
///
/// This should go on the components menu, which is a child of the device prefab.
/// </summary>

public class ComponentsMenu : MonoBehaviour
{
    //we're making the assumption that all components are children of devices-- that might be wrong
    public Device parentNode;
    public GameObject parentDevice;
    public List<AbstractNode> allComponents;//only the ones which are children of parent device
    public GameObject componentPrefab;
    public GeneratorMenu generatorMenu;

    public GameObject camera;//main camera, for the menus to face towards when generated


    //positioning stuff
    public float currentY;//where we put the most recent menu option
    public float yInterval;//distance between menu options

    void Start()
    {
        StartCoroutine(WaitForParentInfo());
        if(generatorMenu == null){
            generatorMenu = gameObject.GetComponent<GeneratorMenu>();
        }
        if(camera == null){
          camera = GameObject.FindWithTag("MainCamera");
        }
    }

    public void AssembleComponents(){
      FindComponents(parentNode);
      foreach(Component component in allComponents){
        GameObject newComponent = Instantiate(componentPrefab);
        generatorMenu.menuItems.Add(newComponent);
        //tell the samples menu of this component what component to start checking the node tree from. A bit messy.
        newComponent.GetComponentInChildren<SampleTypesMenu>(true).parentNode = component;//true = include inactive
        //...and then you need to do some magic to make them stack correctly, and get the name right...
        newComponent.transform.parent = gameObject.transform;
        Vector3 correctLocation = new Vector3(-122f, currentY, -18f);
        newComponent.transform.localPosition = correctLocation;
        StartCoroutine(PutBackWhereItGoes(newComponent, correctLocation));
        //newComponent.transform.LookAt(Vector3.zero);
        newComponent.transform.rotation = newComponent.transform.parent.rotation;
        currentY-=yInterval;
        //change the label to the name-- there must be better ways of doing this...
        SetComponentName(newComponent, component.componentName);
      }
    }

    private void SetComponentName(GameObject newComponent, string newName){
      if(newComponent.transform.GetComponent<VarFinder>().usingTMP){
          newComponent.transform.GetComponent<VarFinder>().TMPlabel.SetText(newName);
        }
        else{
          newComponent.transform.GetComponent<VarFinder>().label.text = newName;
        }
    }

    public void FindComponents(AbstractNode thisNode){
      if(thisNode.nodeName == "ComponentStream"){
        allComponents.Add(thisNode);
      }
      else{
        foreach(AbstractNode childNode in thisNode.childNodes){//not relying on the scene hierarchy
          FindComponents(childNode);
        }
      }
    }

    //we use a co-routine to avoid looking for all of the child components of the device
    //before being told what the parent device is.
    //this might be totally unnecessary to do.
    IEnumerator WaitForParentInfo()
    {
        yield return new WaitUntil(() => parentNode != null);
        AssembleComponents();
    }

    //something, somewhere, is putting devices back at the world origin, 
    //and rather than hunting it down, this function just puts them back where they go after
    IEnumerator PutBackWhereItGoes(GameObject putThisBack, Vector3 correctPosition)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.1f);
        //Debug.Log("component is at " + putThisBack.transform.position + ". Putting it back at the correct location, which is " + correctPosition);
        putThisBack.transform.localPosition = correctPosition;
        putThisBack.transform.rotation = putThisBack.transform.parent.rotation;
    }
}
