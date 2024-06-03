using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Goes on the edit menu of SampleType Menus for specifically floats.
/// This makes the labels say the right information
/// 
/// This will break if you go back to using TMP, but shouldn't be super involved to fix.
///
/// </summary>
public class FloatEditMenu : MonoBehaviour
{
    public TextMeshPro TMPmaxLabel, TMPminLabel, TMPmeanLabel, TMPlastLabel, TMPtimeStampLabel, TMPtotalLabel;
    public Text maxLabel, minLabel, meanLabel, lastLabel, timeStampLabel, totalLabel;
    public bool usingTMP;

    public SampleTypeFloat associatedNode;//this is the right thing, and yet somehow we send representation menu option bad informaiton

    //each of the prefabs should have an abstract representation option
    private List<AbstractRepresentation> representationOptions;
    public List<GameObject> representationPrefabs;
    public GameObject menuOptionPrefab;
    public GameObject repMenu;
    public GameObject representationCollector;
    public float currentY = 0.8f;//where we put the last menu option
    public float yInterval = 0.1f;//amount to move every menu option down by
    // Start is called before the first frame update

    public GameObject spotWhereRepresentationMenuOptionGoes;//something keeps moving these and you've gotten quite frustrated with it

    //Debug stuff
    public Text debugText;
    void Start()
    {        
        representationOptions = new List<AbstractRepresentation>();
        //check that all representation prefabs have an abstract representation-- otherwise throw them away
        foreach(GameObject repPrefab in representationPrefabs){
            if(repPrefab.GetComponent<AbstractRepresentation>() == null){
                representationPrefabs.Remove(repPrefab);
            }
            else{
                representationOptions.Add(repPrefab.GetComponent<AbstractRepresentation>());
            }
        }
        StartCoroutine(WaitForNodeInfo());
        debugText = GameObject.Find("Debug").GetComponent<Text>();//so fucking fragile and inefficient. Find calls bad. 
        debugText.text = "a float edit menu found the debugger";
    }

    IEnumerator WaitForNodeInfo()
    {
        yield return new WaitUntil(() => associatedNode != null);
        UpdateMenu();
        //CreateRepresentationsMenu();
        CreateRepresentationsMenuSimple();
    }

    public void CreateRepresentationsMenuSimple(){
        foreach(GameObject rep in representationPrefabs){
            rep.GetComponent<RepresentationMenuOptionFloat>().associatedNode = associatedNode;
        }
    }

    public void CreateRepresentationsMenu(){
        foreach(GameObject rep in representationPrefabs){
            Debug.Log("Creating a menu option for a rep named: " + rep.name, this);
            debugText.text = "Creating a menu option for a rep named: " + rep.name;//runs
            //create the menu option
            GameObject menuOption = Instantiate(menuOptionPrefab);//runs
            debugText.text = "Menu option placed at " + menuOption.transform.position;

            //the menu options tend to somehow get connected to the... wrong representation collector? 
            //you have no idea how this is happening
            menuOption.GetComponent<RepresentationMenuOptionFloat>().representationCollector = representationCollector;
            menuOption.GetComponent<RepresentationMenuOptionFloat>().associatedNode = associatedNode;
            debugText.text = "rep collector and associated node have been set";

            //put it where it goes
            menuOption.transform.parent = repMenu.transform;
            menuOption.transform.localScale = new Vector3(500.0f, 500.0f, 500.0f);//when you make something a child of a UI, scale gets weird
            // menuOption.transform.position = spotWhereRepresentationMenuOptionGoes.transform.position;
            // spotWhereRepresentationMenuOptionGoes.GetComponent<Glue>().holdThis = menuOption;
            // Debug.Log("The correct place for the menu option is " + menuOption.transform.position);
            menuOption.transform.position = new Vector3(1.5f, currentY, 0.37f);
            menuOption.transform.rotation = repMenu.transform.rotation;
            menuOption.GetComponent<PositionMonitor>().SetCorrectPosition();
            currentY-=yInterval;

            //change the label to the name
            AbstractRepresentation representation = rep.GetComponent<AbstractRepresentation>();
            SetMenuOptionName(menuOption, representation.name);
            //make the button on the menu option be to instantiate a copy of this thing
            menuOption.GetComponent<RepresentationMenuOptionFloat>().representationPrefab = rep;
        }

    }

    private void SetMenuOptionName(GameObject menuOption, string newName){
      if(menuOption.transform.GetComponent<VarFinder>().usingTMP){
          menuOption.transform.GetComponent<VarFinder>().TMPlabel.SetText(newName);
        }
        else{
          menuOption.transform.GetComponent<VarFinder>().label.text = newName;
        }
    }

    //this should get called any time the information might have changed. 
    //if would be pretty expensive to do this in an update function,
    //so try to be smart about it
    public void UpdateMenu(){
        if(IsDefaultText(maxLabel.text) || float.Parse(maxLabel.text)!=associatedNode.maxVal){
            //update label
            if(TMPmaxLabel!=null){
                TMPmaxLabel.SetText(associatedNode.maxVal.ToString());
            }
            else{
                maxLabel.text = (associatedNode.maxVal.ToString());
            }
        }
        if(IsDefaultText(minLabel.text) || float.Parse(minLabel.text)!=associatedNode.minVal){
            //update label
            if(TMPminLabel!=null){
                TMPminLabel.SetText(associatedNode.minVal.ToString());
            }
            else{
                minLabel.text = (associatedNode.minVal.ToString());
            }
        }
        if(IsDefaultText(meanLabel.text) || float.Parse(meanLabel.text)!=associatedNode.meanVal){
            //update label
            if(TMPmeanLabel!=null){
                TMPmeanLabel.SetText(associatedNode.meanVal.ToString());
            }
            else{
                meanLabel.text = (associatedNode.meanVal.ToString());
            }        
        }
        if(IsDefaultText(lastLabel.text) || float.Parse(lastLabel.text)!=associatedNode.lastSampleValue){
            //update label
            if(TMPlastLabel!=null){
                TMPlastLabel.SetText(associatedNode.lastSampleValue.ToString());
            }
            else{
                lastLabel.text = (associatedNode.lastSampleValue.ToString());
            }         
        }
        if(timeStampLabel.text!=associatedNode.lastTimeStamp.ToString()){
            //update label
            if(TMPtimeStampLabel!=null){
                TMPtimeStampLabel.SetText(associatedNode.lastTimeStamp.ToString());
            }
            else{
                timeStampLabel.text = (associatedNode.lastTimeStamp.ToString());
            }           
        }
        if(IsDefaultText(totalLabel.text) || float.Parse(totalLabel.text)!=associatedNode.numberOfSamples){
            //update label
            if(TMPtotalLabel!=null){
                TMPtotalLabel.SetText(associatedNode.numberOfSamples.ToString());
            }
            else{
                totalLabel.text = (associatedNode.numberOfSamples.ToString());
            }       
        }
    }

    //for checking if the text is our default "there's no information here" string or a float
    //this specifically will break if you go back to using TMP
    private bool IsDefaultText(string text){
        try
        {
            float.Parse(text);
            return false;
        }
        catch
        {
            return true;
        }
    }
}
