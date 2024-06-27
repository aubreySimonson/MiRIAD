using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Part of MiRIAD, an authoring tool for digital twins
/// Knows about and controls which URL we're reading from. 
/// Node Manager and MTConnect parser get that info from here.
/// Hypothetically multiple systems of parser, node manager, and url manager 
/// could run in the same scene at the same time, but that isn't tested yet.
/// 
///???-->followspotfour@gmail.com
///</summary>

public class URLManager : MonoBehaviour
{
    public List<string> urls = new List<string>(){"https://demo.metalogi.io/current", "https://smstestbed.nist.gov/vds/current", "http://192.168.50.8:5000/current"};
    public int urlIndex;
}
