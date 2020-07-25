using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ClassRole
{
    MeleeDps,
    RangedDps,
    Healer,
    Tank
}

[CreateAssetMenu(fileName = "Class", menuName = "Scriptables/Class")]
public class ClassData : ScriptableObject
{
    public List<GameObject> mySpells = new List<GameObject>(4);
    [Header("Details")]
    public GameObject myRightItem;
    public GameObject myLeftItem;

    [Space(2)]
    public GameObject myGnome;
    public ClassRole myClassRole;
    public Sprite myIconSprite;
    public Color myClassColor;
    public string myName;
    public string myDescription;
}
