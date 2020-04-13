using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class TutorialSpellCheck : Conditional
{
    public SpellTypeToBeChanged aSpellType;
    public SharedInt myPlayerIndexWithSpell;

    private List<Spell> mySpellsToCheck;
    private TutorialPanel myTutorialPanel;

    public override void OnAwake()
    {
        myTutorialPanel = Object.FindObjectOfType<TutorialPanel>();
    }

    public override void OnStart()
    {
        mySpellsToCheck = new List<Spell>(myTutorialPanel.GetTutorialSpells());
    }

    public override TaskStatus OnUpdate()
    {
        for (int index = 0; index < mySpellsToCheck.Count; index++)
        {
            if (mySpellsToCheck[index].mySpellType == aSpellType)
            {
                myPlayerIndexWithSpell.Value = index;
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }
}
