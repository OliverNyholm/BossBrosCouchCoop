using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Class", menuName = "Scriptables/Class")]
public class ClassData : ScriptableObject
{
    public GameObject myClass;
    public Sprite myIconSprite;
    public string myName;
    public string myDescription;
}
