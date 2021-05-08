using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTag : MonoBehaviour
{
    [SerializeField]
    protected SpellTargetType myTargetTag;
    public SpellTargetType GetTargetTag() { return myTargetTag; }

    public bool IsTargetType(SpellTargetType aTargetType)
    {
        return UtilityFunctions.HasSpellTarget(aTargetType, myTargetTag);
    }
}
