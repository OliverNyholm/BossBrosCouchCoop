using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectTargetingComponent : PlayerTargetingComponent
{
    private int myPlayerIndex = 0;

    private void Update()
    {
        
    }

    public void SetPlayerIndex(int anIndex)
    {
        myPlayerIndex = anIndex;
    }

    public override void SetTarget(GameObject aTarget)
    {
        Target = aTarget;

        if (Target)
        {
            Target.GetComponentInChildren<TargetProjector>().DropTargetProjection(myPlayerIndex);
        }

        if (Target)
        {
            Target.GetComponentInChildren<TargetProjector>().AddTargetProjection(GetComponent<UIComponent>().myCharacterColor, myPlayerIndex);
        }
    }

    public override void SetSpellTarget(GameObject aTarget)
    {
        SpellTarget = aTarget;
    }
}
