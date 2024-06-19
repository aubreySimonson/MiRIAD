using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomDropdownItem : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public bool autotrigger = false;

    void Update(){
        if(autotrigger){
            SetValue();
            autotrigger =false;
        }
    }
    public void SetValue(){
        //figure out what the value of the item is based on where it is in the hierarchy
        Debug.Log("Value is probably..." + (gameObject.transform.GetSiblingIndex()-1));

        //set value to that value
        dropdown.value = gameObject.transform.GetSiblingIndex()-1;
        dropdown.Hide();
    }
}
