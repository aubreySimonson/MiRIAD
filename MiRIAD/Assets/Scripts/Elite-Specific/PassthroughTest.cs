using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIVE.OpenXR.CompositionLayer.Passthrough;
using VIVE.OpenXR.CompositionLayer;

public class PassthroughTest : MonoBehaviour
{
    int ID;
    void Start()
    {
        ID = CompositionLayerPassthroughAPI.CreatePlanarPassthrough(LayerType.Underlay);
    }
}
