using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static Transform FindInChildren(this Transform self, string name)
    {
        int count = self.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = self.GetChild(i);
            if (child.name == name) return child;
            Transform subChild = child.FindInChildren(name);
            if (subChild != null) return subChild;
        }
        return null;
    }

    public static GameObject FindInChildren(this GameObject self, string name)
    {
        Transform transform = self.transform;
        Transform child = transform.FindInChildren(name);
        return child != null ? child.gameObject : null;
    }
}

public static class UtilityFunctions
{
    public static bool HasSpellType(SpellType anAttacktype, SpellType aHasType)
    {
        return (anAttacktype & aHasType) != 0;
    }

    public static bool HasSpellType(SpellOverTimeType anAttacktype, SpellOverTimeType aHasType)
    {
        return (anAttacktype & aHasType) != 0;
    }
}
