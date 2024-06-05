using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assembles the menu of SampleTypes.
/// Should go on top-level Samples Menu gameobject,
/// which is a parent of the relevant canvas.
///
/// It, DevicesMenu, and ComponentsMenu should probably both inherit from
/// an abstract class called "Add Nodes Menu" or something,
/// but you're going to leave that problem until it's harder to fix.
///
/// </summary>

public class SampleTypesMenu : MonoBehaviour
{
    //we're making the assumption that all components are children of devices-- that might be wrong
    public AbstractNode parentNode;
    public GameObject parent;
    public List<AbstractNode> allSampleTypes;//only the ones which are children of parent component
    public GameObject sampleTypePrefab, floatPrefab;
    //public GeneratorMenu generatorMenu;
    public MenuManager menuManager;


    //positioning stuff
    public float currentY;//where we put the most recent menu option
    public float yInterval;//distance between menu options

    void Start()
    {
        StartCoroutine(WaitForParentInfo());
    }

    public void AssembleSamples(){
      Debug.Log("Finding samples on ", this);//runs
      FindSamples(parentNode);//this is the correct node
      foreach(SampleType sampleType in allSampleTypes){
        GameObject newSampleType;
        //seems like it might be the case that neither the if nor else statement run?
        Debug.Log("We're in that foreach loop.");
        if(sampleType is SampleTypeFloat){
          Debug.Log("Sample Type was a float");
          newSampleType = Instantiate(floatPrefab);
          newSampleType.GetComponent<FloatEditMenu>().associatedNode = (SampleTypeFloat)sampleType;//somewhat fragile//has come back to bite you in the ass
        }
        else{
          newSampleType = Instantiate(sampleTypePrefab);
          Debug.Log("Sample Type was not a float");
        }
        //generatorMenu.menuItems.Add(newSampleType);
        menuManager.menuItems.Add(newSampleType);
        //...and then you need to do some magic to make them stack correctly, and get the name right...
        newSampleType.transform.parent = gameObject.transform;
        newSampleType.transform.rotation = newSampleType.transform.parent.rotation;
        newSampleType.transform.localPosition = new Vector3(-150.0f, currentY, 0.0f);
        newSampleType.GetComponent<PositionMonitor>().SetCorrectPosition();
        //newSampleType.transform.transform.LookAt(Vector3.zero);
        currentY-=yInterval;
        //change the label to the name-- there must be better ways of doing this...
        SetSampleTypeName(newSampleType, sampleType.sampleTypeName);
        //newSampleType.GetComponent<MenuRemovalDetector>().collector = generatorMenu.collectorObject;//change this if we stop using generator menus
        //newSampleType.GetComponent<MenuRemovalDetector>().generatorMenu = generatorMenu;
        newSampleType.GetComponent<MenuRemovalDetector>().menuManager = menuManager;
      }
    }

    private void SetSampleTypeName(GameObject newSampleType, string newName){
      if(newSampleType.transform.GetComponent<VarFinder>().usingTMP){
          newSampleType.transform.GetComponent<VarFinder>().TMPlabel.SetText(newName);
        }
        else{
          newSampleType.transform.GetComponent<VarFinder>().label.text = newName;
        }
    }

    //how this works is /very/ different from how it was for components...
    public void FindSamples(AbstractNode thisNode){
      Debug.Log("Find samples is being given this thing" + thisNode, this);//runs, and it is being given an abstract node
      //this assumes that no sample types will be childen of other sample types
      if(thisNode.gameObject.GetComponent<SampleType>() != null){//this, of all things, somehow gives us a null exception
        allSampleTypes.Add(thisNode);
      }
      else{
        if(thisNode.childNodes.Count!=0){
          foreach(AbstractNode childNode in thisNode.childNodes){//not relying on the scene hierarchy
            if(childNode!=null){
              Debug.Log("Calling FindSamples on " + childNode);
              FindSamples(childNode);
            }
          }//end foreach
        }//end if
      }//end else
    }//end findsamples

    //we use a co-routine to avoid looking for all of the child components of the device
    //before being told what the parent device is.
    //this might be totally unnecessary to do.
    IEnumerator WaitForParentInfo()
    {
        yield return new WaitUntil(() => parentNode != null);
        AssembleSamples();
    }
}
