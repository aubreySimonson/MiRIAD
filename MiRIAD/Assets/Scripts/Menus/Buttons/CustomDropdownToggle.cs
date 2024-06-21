using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class CustomDropdownToggle : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public bool isOpen = false;
    public Text debugText;



    void Start(){
        debugText = GameObject.Find("Debug").GetComponent<Text>();
    }


    public void Toggle(){
        debugText.text = "toggling dropdown. ";
        if(isOpen){
            debugText.text += "Dropdown is active. Hiding it now";
            isOpen = false;
            dropdown.Hide();
        }
        else{
            debugText.text += "Dropdown is not active. Showing it now";
            isOpen = true;
            dropdown.Show();
        }
    }
}
