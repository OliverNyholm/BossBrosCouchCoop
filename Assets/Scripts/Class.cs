using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType
{
    Damage,
    DOT,
    Heal,
    HOT,
    Shield,
    Interrupt,
    Silence,
    Slow,
    Buff,
    Ressurect
}


public class Class : MonoBehaviour
{

    public string myClassName;
    public GameObject[] mySpells;

    private float myResource;

    public Class()
    {
        mySpells = new GameObject[8];
    }

    public GameObject GetSpell(int anIndex)
    {
        return mySpells[anIndex];
    }
}
