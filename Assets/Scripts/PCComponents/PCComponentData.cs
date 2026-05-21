using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewComponentData", menuName = "PC/Component Data")]
public class PCComponentData : ScriptableObject
{
    public string componentName;
    public string componentType;
    [TextArea]
    public string description;
    
    [Header("Specifications")]
    public List<string> specifications = new List<string>();
}
