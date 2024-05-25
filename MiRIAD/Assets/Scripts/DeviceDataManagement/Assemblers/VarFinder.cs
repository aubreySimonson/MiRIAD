using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// This script is not part of Theo's original repo.
///
/// It's a litte silly for this script to exist--
/// It does nothing but go on the device prefab,
/// and make it easier to find variables and components on that prefab.
///
/// </summary>

public class VarFinder : MonoBehaviour
{
  public TextMeshPro TMPlabel;//where the name goes
  public Text label;
  public bool usingTMP;//all of this sort of annoying workaround is becuase XRTK hands demo scene doesn't use TMP.
}
