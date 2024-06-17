using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sub-type of sampletype for containing additional information about floats.
///
/// ??--> simonson.au@northeastern.edu
/// </summary>

public class SampleTypeFloat : SampleType
{
    public float minVal;
    public float maxVal;
    public float meanVal;
    public float total;//mostly useful for calculating the average

    public float lastSampleValue;

    //incorporate more data related to this node
    public void AddSample(float newSample){
        if(newSample<minVal){
            minVal = newSample;
        }
        if(newSample>maxVal){
            maxVal = newSample;
        }

        total += newSample;
        numberOfSamples++;
        meanVal = total/numberOfSamples;

        lastSampleValue = newSample;
    }
}
