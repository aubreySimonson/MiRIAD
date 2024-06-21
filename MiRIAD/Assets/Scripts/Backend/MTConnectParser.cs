using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;//for file reading
using UnityEngine.UI;
using System;//for try/catch blocks

///<summary>
///Part of MiRIAD, an authoring tool for digital twins
/// This reads data, and assembles it into a graph of Nodes (which really need a better name...)
/// It should be possible to replace this with other parsers from other sources,
/// and have the rest of the project not particularly care.
/// 
/// This is a little messy and could use refactoring.
///???-->followspotfour@gmail.com
///</summary>

public class MTConnectParser : MonoBehaviour
{
  //public enum RemoteURL {SMSTestbed, Metalogi};
  //public RemoteURL remote;
  public bool useStaticSampleData;//if true, load from a file. Otherwise, look at the url. Not working rn because we don't have any static sample data.
  public ServerTalker serverTalker;//this is a script where we hide all of our code related to getting anything from the internet.
  public URLManager uRLManager;
  
  public string remoteUrl = ""; 
  public string fileName;//this should be just the name of the file, with no type extension. Put the file in the Resources folder.

  public GameObject nodePrefab;

  public bool collapseDuplicateSamples;//do this whenever you have a lot of data-- for example, time-series data-- to prevent your computer from freezing

  public int totalNodes = 0;//we don't use this information for anything, but it's cool to know
  public Text debugText;//leaving this null won't throw errors or break anything

  public GameObject rootNode;
  public NodeManager nodeManager;//spaghetti, but having connections both ways makes it easier to deal with async stuff

  //variables for random guesses about how to read xml data on an android device below.
  private TextAsset XMLObject;
  public TextAsset textAsset;
  StringReader xml;
  string extractedContent;
  


  void Awake(){//awake runs before the first frame, so that other things can use this data
    //fancy switcher to make it easier to try different URLs
    // if(remote == RemoteURL.Metalogi){
    //   remoteUrl="https://demo.metalogi.io/current";
    // }
    // else{
    //   remoteUrl = "https://smstestbed.nist.gov/vds/current";
    // }
    remoteUrl = uRLManager.urls[uRLManager.urlIndex];
    ReadSampleData();
  }

#region get data

  public void ReadSampleData(){
    if(useStaticSampleData){
      LoadStaticSampleData();
    }
    else{
      debugText.text = "Attempting to get data from " + remoteUrl;
      serverTalker.GetDataSnapshot(remoteUrl);//this will then call SetAndReadWebData after it hears back from the server
    }
    
  }

  //spaghetti, but server talker needs to do a coroutine to do a web request, 
  //and this is how it sends that information back when its done
  public void SetAndReadWebData(string data){
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(data);
    XmlNodeList metaLevelNodes = xmlDoc.ChildNodes;
    XmlNode topLevelNode = metaLevelNodes[2];//the first 2 are metadata
    XmlNodeList topLevelNodes = topLevelNode.ChildNodes;
    XmlNode allContent = topLevelNodes[1];
    CreateNodeGameObject(allContent, true);
    Debug.Log("Total nodes: " + totalNodes);
    debugText.text = "Total nodes: " + totalNodes;
  }

  private void LoadStaticSampleData(){
    XmlDocument xmlDoc = new XmlDocument();
    //textAsset = (TextAsset)Resources.Load(fileName, typeof(TextAsset));
    xmlDoc.LoadXml ( textAsset.text );
    XmlNodeList topLevelNodes = xmlDoc.ChildNodes;
    XmlNode allContent = topLevelNodes[1];
    CreateNodeGameObject(allContent, true);
    Debug.Log("Total nodes: " + totalNodes);
    debugText.text = "Total nodes: " + totalNodes;
  }

#endregion
  
# region create non-sample-type nodes
  private AbstractNode CreateNodeGameObject(XmlNode node, bool doRecursion){
    if(node == null|| node.Name == "#text") return null;
    totalNodes++;
    AbstractNode thisNodeUnity = CreateNodeHelper(node);
    if(doRecursion && node.Name!="Samples"){//don't do recursion on sets of samples
      NodeRecursion(node, thisNodeUnity);
    }
    return thisNodeUnity;
  }

  private AbstractNode CreateNodeGameObject(XmlNode node, AbstractNode parentNode, bool doRecursion){
    if (parentNode == null || node == null || node.Name == "#text") return null;

    if(node.Name!="#text"){//trims white space
      totalNodes++;
      AbstractNode thisNodeUnity = CreateNodeHelper(node);
      if(thisNodeUnity == null){
        return null;
      }
      thisNodeUnity.parentNode = parentNode;
      thisNodeUnity.gameObject.transform.parent = parentNode.gameObject.transform;//stacks them in the hierarchy-- optional
      if(doRecursion && node.Name!="Samples"){//don't do recursion on sets of samples
        NodeRecursion(node, thisNodeUnity);
      }
      return thisNodeUnity;
    }
    return null;
  }

  private AbstractNode CreateNodeHelper(XmlNode node){
    GameObject thisNodeGo = Instantiate(nodePrefab);//instantiate an empty game object
    AbstractNode thisNodeUnity = AddCorrectNodeType(node, thisNodeGo);
    if(thisNodeUnity==null){//add correct node type doesn't always return a node
      Destroy(thisNodeGo);//don't leave an abandoned game object-- obviously some refactoring could make all of this more efficient
      return null;
    }
    if(rootNode==null){
      rootNode=thisNodeGo;
      nodeManager.rootNode = thisNodeUnity;
    }
    thisNodeUnity.nodeName = node.Name;
    thisNodeUnity.nodeID = GetID(node);
    return thisNodeUnity;
  }

  private void NodeRecursion(XmlNode node, AbstractNode thisNodeUnity){
    XmlNodeList childNodes = node.ChildNodes;
    thisNodeUnity.childNodes = new List<AbstractNode>();
    if(collapseDuplicateSamples && node.Name == "Samples"){//don't do recursion on children of samples
      HandleDuplicateSamples(childNodes, thisNodeUnity);
    }//end if
    else{
        foreach(XmlNode childNode in childNodes){
          thisNodeUnity.childNodes.Add(CreateNodeGameObject(childNode, thisNodeUnity, true));
        }//end for
    }//end else
  }

//creates one sample type game object per node name
  private void HandleDuplicateSamples(XmlNodeList childNodes, AbstractNode thisNodeUnity){
    List<string> nodeNames = new List<string>();
    foreach(XmlNode childNode in childNodes){
      if(!nodeNames.Contains(childNode.Name)){
        nodeNames.Add(childNode.Name);
        thisNodeUnity.childNodes.Add(CreateNodeGameObject(childNode, thisNodeUnity, false));
      }
    }
  }

  //a node could need an abstract node script, 
  //or a more specific scripts which inherits from it and has special information.
  //we hide checking for every possible node type we care about down here
  private AbstractNode AddCorrectNodeType(XmlNode node, GameObject thisNodeGo){
    AbstractNode thisNodeUnity;
    if(node.Name == "DeviceStream" || node.Name == "Device"){
      thisNodeUnity = thisNodeGo.AddComponent<Device>();//inherits from abstract node
      thisNodeUnity.GetComponent<Device>().deviceName = node.Attributes["name"].Value;//we assume that all devices will have a name in this format. do they?
      thisNodeUnity.nodeID = node.Attributes["uuid"].Value;
    }
    else if(node.Name == "ComponentStream"){
      thisNodeUnity = thisNodeGo.AddComponent<Component>();//inherits from abstact node
      thisNodeUnity.GetComponent<Component>().componentName = node.Attributes["component"].Value;
      thisNodeUnity.nodeID = node.Attributes["componentId"].Value;
    }
    else if (node.Name == "Samples"){
      thisNodeUnity = thisNodeGo.AddComponent<SamplesHolder>();//inherits from abstact node
      SamplesAggregator(node, thisNodeUnity, thisNodeGo);
      thisNodeUnity.nodeID = GetID(node);
    }
    else{
      thisNodeUnity = thisNodeGo.AddComponent<AbstractNode>();
      thisNodeUnity.nodeID = GetID(node);
    }
    return thisNodeUnity;
  }


  #endregion

  private void SamplesAggregator(XmlNode holderNode, AbstractNode holderNodeUnity, GameObject holderGO){
    //get all children of the node
    XmlNodeList childNodes = holderNode.ChildNodes;

    //make a samples type for every sample name-- makes it easier to assign samples to their sampleTypes
    List<string> sampleNames = new List<string>();   
    //make a list of sampletypes, gameobjects they are attached to
    List<SampleType> sampleTypes = new List<SampleType>();
    List<AbstractNode> samplesButAbstract = new List<AbstractNode>();//this just prevents us from casting in 187
    List<GameObject> sampleTypeGOs = new List<GameObject>();

    foreach(XmlNode childNode in childNodes){
      SampleType thisSampleType;
      if(!sampleNames.Contains(childNode.Name)){//if we haven't seen this name before
        sampleNames.Add(childNode.Name);
        GameObject thisSampleTypeGO = Instantiate(nodePrefab);
        sampleTypeGOs.Add(thisSampleTypeGO);

        //add sample node of correct type
        thisSampleType = AddSampleTypeOfCorrectType(childNode, thisSampleTypeGO); 
        sampleTypes.Add(thisSampleType); 
        samplesButAbstract.Add(thisSampleType);
        //set its name
        thisSampleType.sampleTypeName = childNode.Name;
        thisSampleType.parentNode = holderNodeUnity;
        thisSampleTypeGO.transform.parent = holderGO.transform;
      }
      else{//if we've seen this sampletype before, find it
        thisSampleType = FindSampleTypeByName(sampleTypes, childNode.Name);
      }
      //then, now that we know that the sample type exists, do things with the data from this sample:
      //increase the number of samples we've found
      thisSampleType.numberOfSamples++;


      //time stamp things-- updates timestamp if more recent
      bool updateVal = CheckForMoreRecentTimeStamp(childNode, thisSampleType);

      //get the id for this sample type
      thisSampleType.nodeID = GetID(childNode);


      //special things we only do for floats
      if(thisSampleType is SampleTypeFloat){
        try{
          float f = float.Parse(childNode.InnerText);
          Debug.Log(f + "was a float");
          SampleTypeFloat thisSampleTypeFloat = (SampleTypeFloat)thisSampleType;
          thisSampleTypeFloat.total += f;
          if(updateVal){
            thisSampleTypeFloat.lastSampleValue = f;
          }
          if(f>thisSampleTypeFloat.maxVal){
            thisSampleTypeFloat.maxVal = f;
          }
          if(f<thisSampleTypeFloat.minVal){
            thisSampleTypeFloat.minVal = f;
          }
          thisSampleTypeFloat.meanVal = thisSampleTypeFloat.total/(float)thisSampleTypeFloat.numberOfSamples;
          //Debug.Log("The new mean of " + thisSampleTypeFloat.sampleTypeName + " is " + thisSampleTypeFloat.meanVal);
        }
        catch (Exception e){
          Debug.LogError("Attempted to make a SampleTypeFloat from " + childNode.InnerText + ", which is not a float");
        }
      }//end special float things
    }//end foreach
    holderNodeUnity.childNodes = samplesButAbstract;
  }//end samples aggregator

  ///returns true if there's a more recent timestamp
  ///also updates the lasttimestamp of the sampletype
  private bool CheckForMoreRecentTimeStamp(XmlNode node, SampleType sampleType){
    bool updateVal = false;//flag for a few lines later
    try{
      System.DateTime timeStamp = System.DateTime.Parse(node.Attributes["timestamp"].Value);
      if(sampleType.lastTimeStamp == null || timeStamp>sampleType.lastTimeStamp){
        //Debug.Log("timestamp: " + timeStamp);
        sampleType.lastTimeStamp = timeStamp;
        return true;
      }//end if
    }//end try
    catch{
      Debug.Log("no correctly formatted timestamp for this sample");
    }
    return false;
  }


  //this is an inefficient, really brute force method and someone else should fix it later.
  //maybe you in the future when you're better at this
  private SampleType FindSampleTypeByName(List<SampleType> list, string name){
    foreach(SampleType sampleType in list){
      if(sampleType.sampleTypeName == name){
        return sampleType;
      }
    }
    return null;
  }

  //this is structured a bit stupidly because addcomponent is very picky--
  //there doesn't seem to be a way to add a component to a gameobject /after/ it's created
  private SampleType AddSampleTypeOfCorrectType(XmlNode node, GameObject go){
    //Debug.Log("Attempting to parse " + node.InnerText);
    //note that InnerText will return /all/ of the inner text of our node, so if out node isn't a leaf, this will get weird
    //check if it's a float
    float value;
    if(float.TryParse(node.InnerText, out value)==true){
      Debug.Log("This sample was a float. The text was " + node.InnerText);//runs sometimes!
      SampleTypeFloat newFloat = go.AddComponent<SampleTypeFloat>();
      Debug.Log("Ok. We have added a sampletypefloat component to the new gameobject");
      return newFloat;
    }
    Debug.Log("This sample was not a float. The text was " + node.InnerText);
    //if you were to treat other values as special, you would check for them here
    //if it isn't anything specific that we care about, return an abstract sample type
    SampleType newSampleType = go.AddComponent<SampleType>();
    return newSampleType;
  }

  private string GetID(XmlNode node){
    List<string> idAttributes = new List<string> { "id", "dataItemId", "name", "uuid", "componentId", "path" };
    foreach (string attrName in idAttributes)
    {
      if(node.Attributes[attrName]!=null){
        return node.Attributes[attrName].Value;
      }
    }
    return "could not find id";
  }

  //consider cutting this. We don't use it right now.
  private bool AreSameNamedSiblings(string name, AbstractNode parentNode){
    foreach(AbstractNode sibling in parentNode.childNodes){
      if(sibling.nodeName == name){
        return true;
      }
    }//end for
    return false;
  }//end check for same named siblings
}
