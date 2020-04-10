using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpellType
{
    Damage,
    DOT,
    Heal,
    HOT,
    Shield,
    Interrupt,
    Taunt,
    Slow,
    Buff,
    Ressurect,
    Special
}

[System.Flags]
public enum SpellTarget
{
    Friend = 1 << 1,
    Enemy = 1 << 2,
    Anyone = Friend | Enemy
}

public class Class : MonoBehaviour
{
    public string myClassName;
    public ClassRole myClassRole;

    [Header("Image to show on hud")]
    public Sprite mySprite;

    [SerializeField]
    private GameObject myAutoAttack = null;
    public GameObject[] mySpells;
    public float[] myCooldownTimers;

    [HideInInspector]
    public int mySpellSize = 4;

    private PlayerUIComponent myUIComponent;

    public enum ClassRole
    {
        MeleeDps,
        RangedDps,
        Healer,
        Tank
    }

    public void Awake()
    {
        myUIComponent = GetComponent<PlayerUIComponent>();
    }

    public void Start()
    {
        PoolManager poolManager = PoolManager.Instance;

        for (int index = 0; index < mySpells.Length; index++)
        {
            if (mySpells[index] == null)
                continue;

            Spell spell = mySpells[index].GetComponent<Spell>();
            spell.CreatePooledObjects(poolManager, 3);

            myUIComponent.SetSpellHud(spell, index);
        }

        myAutoAttack.GetComponent<Spell>().CreatePooledObjects(poolManager, 4);
    }

    private void Update()
    {
        for (int index = 0; index < myCooldownTimers.Length; index++)
        {
            if (myCooldownTimers[index] > 0.0f)
            {
                myCooldownTimers[index] -= Time.deltaTime;
                myUIComponent.SetSpellCooldownText(index, myCooldownTimers[index]);
            }
        }
    }

    public bool IsSpellCastOnFriends(int anIndex)
    {
        return mySpells[anIndex].GetComponent<Spell>().IsCastOnFriends();
    }

    public bool IsSpellOnCooldown(int anIndex)
    {
        if (myCooldownTimers[anIndex] > 0.0f)
            return true;

        return false;
    }

    public GameObject GetAutoAttack()
    {
        return myAutoAttack;
    }

    public GameObject GetSpell(int anIndex)
    {
        return mySpells[anIndex];
    }

    public void SetSpellOnCooldown(int anIndex)
    {
        myCooldownTimers[anIndex] = mySpells[anIndex].GetComponent<Spell>().myCooldown;
        myUIComponent.SetSpellCooldownText(anIndex, myCooldownTimers[anIndex]);
    }
}
