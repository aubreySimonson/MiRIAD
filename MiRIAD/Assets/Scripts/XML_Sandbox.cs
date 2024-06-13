using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

//a script for safely playing around with MTConnect data without wrecking our parser
//this may eventually grow into a better parser

public class XML_Sandbox : MonoBehaviour
{
    XDocument xDoc;
    
    void Start()
    {
        StartCoroutine(GetWebData("https://demo.metalogi.io/current"));//<--this works.
    }

    public IEnumerator GetWebData(string address){
        Debug.Log("Getting web data");
        UnityWebRequest www = UnityWebRequest.Get(address);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success){
            Debug.LogError("That wasn't supposed to happen: " + www.error);
        }
        else{
            Debug.Log("Web data is: " + www.downloadHandler.text);
            //SetAndReadWebData(www.downloadHandler.text);
            //GetAllIds(www.downloadHandler.text);
            xDoc = XDocument.Parse(www.downloadHandler.text);
            //Debug.Log(GetNodeInnerText("_127.0.0.1_7878_observation_update_rate"));


            Debug.Log("finished");
        }
    }



    public string GetNodeInnerText(string nodeId)
    {
        // Define the list of attribute names to search for
        string[] attributeNames = { "id", "dataItemId", "name", "uuid", "componentId", "path" };

        // Find the node with the matching attribute
        XElement matchingNode = xDoc.Descendants()
            .FirstOrDefault(node =>
                attributeNames.Any(attr => 
                    (string)node.Attribute(attr) == nodeId));

        // Return the inner text of the matching node, or null if not found
        return matchingNode?.Value;
    }

    //prints all ids in remote data as strings
    static void GetAllIds(string xmlData){
        XDocument xDoc = XDocument.Parse(xmlData);
        List<string> idAttributes = new List<string> { "id", "dataItemId", "name", "uuid", "componentId", "path" };
        List<string> ids = new List<string>();

        foreach (string attrName in idAttributes)
        {
            ids.AddRange(xDoc.Descendants()
                             .Attributes(attrName)
                             .Select(attr => attr.Value));
        }

        foreach (string id in ids)
        {
            Debug.Log(id);
        }
    }
}
