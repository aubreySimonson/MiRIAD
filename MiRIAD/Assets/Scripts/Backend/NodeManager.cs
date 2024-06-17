using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine.UI;

///<summary>
///Part of MiRIAD, an authoring tool for digital twins
/// This is the high-level brain of MiRIAD
/// It controls all of the other stuff.
/// 
/// Architecture question-- should refreshing / recording check for new nodes which aren't in the node tree?
/// Much harder that way. Maybe a change to make to the architecture later. 
/// It will be harder then, but you'll also be more experienced
/// 
///???-->followspotfour@gmail.com
///</summary>

public class NodeManager : MonoBehaviour
{
    public MTConnectParser mTConnectParser;
    public ServerTalker serverTalker;
    public AbstractNode rootNode;
    private bool isRecording = false;
    public Text debugText;

    public XML_Sandbox xML_Sandbox;
    public float refreshRate;//in seconds
    private bool updatingDisplays = false;//don't start making repeated calls to the server until we're actually trying to display data
     

    private int nodesChecked;//for making sure that we actually check all of the nodes when we do things recursively

    public List<AbstractRepresentation> representations;

    void Start(){
        //make sure that we start with some data
        StartCoroutine(xML_Sandbox.GetWebData("https://demo.metalogi.io/current"));
    }

    void Update(){
        if(!updatingDisplays && representations.Count >0){
            debugText.text = "refresh displays started by " + representations[0].name;
            InvokeRepeating("RefreshDisplays", 1.0f, 4.0f);
            updatingDisplays=true;
        }
    }

    //go and check data only for nodes being used in displays
    public void RefreshDisplays(){
        debugText.text += "refreshing display";
        //get data from remote-- the syncing on these is definitely going to be kind of weird
         StartCoroutine(xML_Sandbox.GetWebData("https://demo.metalogi.io/current"));
        //iterate over XML and find everything in the displayed IDs list
        foreach(AbstractRepresentation rep in representations){
            debugText.text += xML_Sandbox.GetNodeInnerText(rep.GetIdInNodeParent());

            //the following is not the most elegant thing you've ever written
            if(rep is FloatRepresentation){
                FloatRepresentation repButAFloat = (FloatRepresentation) rep;
                repButAFloat.RefreshDisplay(xML_Sandbox.GetNodeInnerText(rep.GetIdInNodeParent()));
            }
        }
    }


    /// functions below this lines are sort of in the ideas phase, and aren't really doing anything yet 


    //start initialization of everything
    public void BuildTree(){
        mTConnectParser.ReadSampleData();
    }

    public void StartRecording(){
        isRecording = true;
    }

    public void StopRecording(){
        isRecording = false;
        CollapseAllSamples();
    }

    public void CollapseAllSamples(){

    }

    
    //go and check remote for new data for all nodes
    public void RefreshAll(){
        //successfully walks the tree, though it doesn't get anything in particular of value from it yet
        CheckMoreNodesRecursively(rootNode);
    }

    private void CheckMoreNodesRecursively(AbstractNode nextNode){
        //update data for this node
        Debug.Log("for now, we're just checking that this happens at all");
        //string word = (string)xdoc.XPathSelectElement("//word[@id='10001']");
        //then, do the recursion...
        //if list of length 0, return
        if(nextNode == null || nextNode.childNodes == null || nextNode.childNodes.Count == 0){
            return;
        }
        //else, call CheckMoreNodes on all children
        else{
            foreach(AbstractNode node in nextNode.childNodes){
                CheckMoreNodesRecursively(node);
            }
        }
    }
}
