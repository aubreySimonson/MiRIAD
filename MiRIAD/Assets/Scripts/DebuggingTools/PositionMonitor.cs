using UnityEngine;
using System.Collections;

/// <summary>
/// Position of some objects are being mysteriously moved. 
/// Rather than tracking down what is moving them, this script just puts them back where they go. 
/// </summary>

public class PositionMonitor : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    public bool lockPosition = false;

    public void SetCorrectPosition()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lockPosition = true;
        StartCoroutine(UnlockAfterDelay());
    }

    void Update()
    {
        if (lockPosition && transform.position != lastPosition)
        {
            Debug.Log("Position changed from " + lastPosition + " to " + transform.position + ". Putting it back now.", this);
            transform.position = lastPosition;
        }
        if(lockPosition && transform.rotation != lastRotation){
            Debug.Log("Rotation changed from " + lastRotation + " to " + transform.rotation + ". Putting it back now.", this);
            transform.rotation = lastRotation;
        }
    }

    IEnumerator UnlockAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);//10 frames//should be 0.2 but we're fucking around
        lockPosition = false;
    }
}