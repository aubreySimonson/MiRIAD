using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// This script is not part of Theo's original repo.
///
/// Assembles the menu of devices.
/// Should go on top-level Create Devices gameobject, 
/// which is a parent of the relevant canvas.
///
/// It and ComponentsMenu should probably both inherit from 
/// an abstract class called "Add Nodes Menu" or something, 
/// but you're going to leave that problem until it's harder to fix.
///
/// </summary>

public class DevicesMenu : MonoBehaviour
{
    public MTConnectParser parser;//just for getting the root node. To make this more general, we should probably have the parser give this to something else.
    public GameObject rootNode;
    public List<AbstractNode> allDevices;
    public GameObject devicePrefab;
    public Transform putDevicesHere;
    public GeneratorMenu generatorMenu;

    //positioning stuff
    private float currentY;//where we put the most recent menu option
    public float yInterval;//distance between menu options


    // Start is called before the first frame update
    void Start()
    {
      StartCoroutine(WaitForRootNode());
      currentY = 1.12f;
      //AssembleDevices();
      if(parser == null){
        parser = GameObject.FindObjectOfType<MTConnectParser>();
      }
      if(generatorMenu == null){
        generatorMenu = gameObject.GetComponent<GeneratorMenu>();
      }
    }

    //we use a co-routine to avoid trying to assemble devices from our little custom data structure
    //before our custom data structure is ready
    IEnumerator WaitForRootNode()
    {
        yield return new WaitUntil(() => parser.rootNode != null);
        rootNode = parser.rootNode;
        AssembleDevices();
    }

    public void AssembleDevices(){
      FindDevices(rootNode.GetComponent<AbstractNode>());//use this if we can't be sure that devices will have a device component instead of a generic abstract node
      foreach(Device device in allDevices){
        GameObject newDevice = Instantiate(devicePrefab);//runs
        generatorMenu.menuItems.Add(newDevice);
        //tell the components menu of this device what device to start checking the node tree from. A bit messy.
        newDevice.GetComponentInChildren<ComponentsMenu>(true).parentNode = device;//true = include inactive
        //...and then you need to do some magic to make them stack correctly, and get the name right...
        Vector3 correctLocation = new Vector3(-0.16f, currentY, 0.561f);
        newDevice.transform.position = correctLocation;
        StartCoroutine(PutBackWhereItGoes(newDevice, correctLocation));//never runs
        newDevice.transform.rotation = Quaternion.identity;//this might not actually do anything because the devices menu is always at a sensible location
        currentY-=yInterval;
        //change the label to the name-- there must be better ways of doing this...
        SetDeviceName(newDevice, device.deviceName);
        Debug.Log("new device was placed at "+ newDevice.transform.position);
      }
    }

    private void SetDeviceName(GameObject newDevice, string newName){
      if(newDevice.transform.GetComponent<VarFinder>().usingTMP){
          newDevice.transform.GetComponent<VarFinder>().TMPlabel.SetText(newName);
        }
        else{
          newDevice.transform.GetComponent<VarFinder>().label.text = newName;
        }
    }
    public void FindDevices(AbstractNode thisNode){
      if(thisNode.nodeName == "DeviceStream"){
        allDevices.Add(thisNode);
      }
      else{
        foreach(AbstractNode childNode in thisNode.childNodes){//not relying on the scene hierarchy
          FindDevices(childNode);
        }
      }
    }

    //something, somewhere, is putting devices back at the world origin, 
    //and rather than hunting it down, this function just puts them back where they go after
    IEnumerator PutBackWhereItGoes(GameObject putThisBack, Vector3 correctPosition)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(0.1f);
        Debug.Log("item is at " + putThisBack.transform.position + ". Putting it back at the correct location, which is " + correctPosition);
        putThisBack.transform.position = correctPosition;
    }
}
