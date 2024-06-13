using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// This is the abstract class that all other Nodes should use it or inherit from.
/// ???-->simonson.au@northeastern.edu
/// </summary>

public class AbstractNode : MonoBehaviour
{
  public List<AbstractNode> childNodes;
  public AbstractNode parentNode;//the choice to make this a single node or a list... is really important
  public string nodeName;
  public string nodeID;
  //public AbstractValue value;--actually, it seems like very few nodes actually have values
  //do you also add a dictionary of other values?
  public GameObject physicalRepresentation;

}
