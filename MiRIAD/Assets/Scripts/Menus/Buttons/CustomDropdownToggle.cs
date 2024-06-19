using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CustomDropdownToggle : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public void Toggle(){
        if(dropdown.IsActive()){
            dropdown.Show();
        }
        else{
            dropdown.Hide();
        }
    }
}
