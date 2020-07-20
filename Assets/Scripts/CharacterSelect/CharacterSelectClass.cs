using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectClass : Class
{
    private CharacterSelector myCharacterSelector;

    protected override void Start() { }

    public void SetCharacterSelector(CharacterSelector aCharacterSelector)
    {
        myCharacterSelector = aCharacterSelector;
        myCharacterSelector.EventOnClassChanged += OnClassChanged;

        SetClassDetail(myCharacterSelector.GetCurrentClassData());
    }

    void OnClassChanged()
    {
        ClassData classData = myCharacterSelector.GetCurrentClassData();
        SetClassDetail(classData);
    }

    public void SetClassDetail(ClassData aClassData)
    {
        PoolManager poolManager = PoolManager.Instance;

        mySpells = aClassData.mySpells.ToArray();

        for (int index = 0; index < mySpells.Length; index++)
        {
            if (mySpells[index] == null)
            {
                myUIComponent.SetSpellHud(null, index);
                myCooldownTimers[index] = 0.01f;
                continue;
            }

            Spell spell = mySpells[index].GetComponent<Spell>();
            spell.CreatePooledObjects(poolManager, 3);

            myUIComponent.SetSpellHud(spell, index);
            myCooldownTimers[index] = 0.01f;
        }
    }
}
