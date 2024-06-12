using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Abstract class which all representations should inherit from.
/// Mostly for being able to put them all in a list of the same type--
/// They should be able to have basically nothing in common.
///
/// ??--> simonson.au@northeastern.edu
/// </summary>

public class AbstractRepresentation : MonoBehaviour
{
    public string name;//what name should we display in the menu?
    public string id;
    //representations should generally be able to set themselves up from just this information
    public NodeManager nodeManager;
    public AbstractNode associatedNode;
    public void Initialize(SampleType associatedNode){

    }  

    //not every node has an id-- this is how we're trying to deal with that    
    public string GetIdInNodeParent(){
        //return "testing what part breaks things";//runs
        try{
            if(associatedNode == null){
                return "no associated node!";//this happens? how?
            }
            if(associatedNode.nodeID!=null && associatedNode.nodeID!= "could not find id"){
                return associatedNode.nodeID;
            }
            if(associatedNode.parentNode == null){
                return "associated node has no id or parent";
            }
            else if(associatedNode.parentNode.nodeID != null && associatedNode.parentNode.nodeID!="could not find id"){
                return associatedNode.parentNode.nodeID;
            }
            else if(associatedNode.parentNode.parentNode.nodeID != null && associatedNode.parentNode.parentNode.nodeID != "could not find id"){
                return associatedNode.parentNode.parentNode.nodeID;
            }
            else{
                return "could not find id in node, parent, or grandparent";
            }
        }
        catch(Exception e){
            return "something went wrong in trying to find an ID" + e;//runs
        }
    }

}
