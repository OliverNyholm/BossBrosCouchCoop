using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectClass : Class
{
    private CharacterSelector myCharacterSelector;
    private CharacterSelectCastingComponent myCastingComponent;

    public override void Awake()
    {
        base.Awake();
        myCastingComponent = GetComponent<CharacterSelectCastingComponent>();
    }

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

        myCastingComponent.OnClassChanged();
        mySpells = aClassData.mySpells.ToArray();

        aClassData.AddRequiredComponents(gameObject);

        CharacterSelectUIComponent csUIComponent = myUIComponent as CharacterSelectUIComponent;

        for (int index = 0; index < mySpells.Length; index++)
        {
            if (mySpells[index] == null)
            {
                csUIComponent.SetSpellHud(null, aClassData.myClassColor,  index);
                continue;
            }


            Spell spell = mySpells[index].GetComponent<Spell>();
            spell.CreatePooledObjects(poolManager, spell.myPoolSize * 2, gameObject);

            csUIComponent.SetSpellHud(spell, aClassData.myClassColor, index);
            myCooldownTimers[index] = 0.0f;
            myUIComponent.SetSpellCooldownText(index, myCooldownTimers[index]);
        }
    }
}
