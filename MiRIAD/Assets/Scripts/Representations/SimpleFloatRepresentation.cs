using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SimpleFloatRepresentation : FloatRepresentation
{

    public TextMeshPro display;

    public Text debugText;

    //Spaghetti, but might be useful when all of the parts do actually have to talk to eachother
    public SampleTypeFloat underlyingNode;

    //menus should call this after instantiating the relevant prefab. 
    public override void Initialize(SampleTypeFloat associatedNode){
        debugText = GameObject.Find("Debug").GetComponent<Text>();
        SetUnderlyingNode(associatedNode);
        SetDisplayValue(associatedNode.lastSampleValue);
        gameObject.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
        nodeManager = GameObject.Find("NodeManager").GetComponent<NodeManager>();//a bit fragile
        nodeManager.representations.Add(this);
    }

    public override void RefreshDisplay(float newValue){
        SetDisplayValue(newValue);
    }

    public override void RefreshDisplay(string newValue){
        SetDisplayValue(newValue);
    }

    public void SetDisplayValue(string newValue){
        display.text = newValue;
    }

    public void SetDisplayValue(float newValue){
        display.text = newValue.ToString();
    }

    public void SetUnderlyingNode(SampleTypeFloat node){
        underlyingNode = node;//this one is a sampletypefloat
        associatedNode = node;//samething, but its an abstract node
    }
}
