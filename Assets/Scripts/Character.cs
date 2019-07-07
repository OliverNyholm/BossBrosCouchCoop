using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Avatar used by character")]
    [SerializeField]
    private Sprite myAvatarSprite;

    public float myBaseSpeed;
    public float myJumpSpeed;
    public float myGravity;

    public string myName;
    public Color myCharacterColor;

    protected Animator myAnimator;

    public GameObject Target { get; protected set; }

    protected CharacterHUD myCharacterHUD;
    protected CharacterHUD myTargetHUD;
    protected Castbar myCastbar;

    protected Coroutine myCastingRoutine;

    protected Health myHealth;
    protected Resource myResource;
    protected Stats myStats;
    protected Class myClass;


    protected List<BuffSpell> myBuffs;

    protected float myStunDuration;
    protected float myAutoAttackCooldown;
    protected float myAutoAttackCooldownReset = 1.0f;

    protected bool myIsCasting;

    protected virtual void Start()
    {
        myAnimator = GetComponent<Animator>();

        myHealth = GetComponent<Health>();
        myResource = GetComponent<Resource>();
        myStats = GetComponent<Stats>();
        myClass = GetComponent<Class>();

        myBuffs = new List<BuffSpell>();

        Target = null;
        myIsCasting = false;
    }

    protected virtual void SetupHud(Transform aUIParent)
    {
        aUIParent.GetComponent<CanvasGroup>().alpha = 1.0f;

        myCharacterHUD = aUIParent.transform.Find("CharacterHud").GetComponent<CharacterHUD>();
        myTargetHUD = aUIParent.transform.Find("TargetHud").GetComponent<CharacterHUD>();
        myCastbar = aUIParent.transform.Find("Castbar Background").GetComponent<Castbar>();

        myCharacterHUD.SetName(myName);
        myCharacterHUD.SetClassSprite(myClass.mySprite);
        myCharacterHUD.SetAvatarSprite(myAvatarSprite);
        myCharacterHUD.SetHudColor(myCharacterColor);

        myHealth.EventOnHealthChange += ChangeMyHudHealth;
        myHealth.EventOnHealthZero += OnDeath;

        ChangeMyHudHealth(myHealth.GetHealthPercentage(), myHealth.myCurrentHealth.ToString() + "/" + myHealth.myMaxHealth.ToString(), GetComponent<Health>().GetTotalShieldValue());

        if (myResource)
        {
            myResource.EventOnResourceChange += ChangeMyHudResource;
            ChangeMyHudResource(myResource.GetResourcePercentage(), myResource.myCurrentResource.ToString() + "/" + myResource.MaxResource.ToString());
        }
    }

    protected abstract bool IsMoving();

    protected virtual void Update()
    {
        HandleBuffs();

        //Gör en kommentar ändå, puss på dig Oliver <3. Saknar dig.
        myAnimator.SetLayerWeight(1, 1);
    }

    protected IEnumerator CastbarProgress(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        GetComponent<AudioSource>().clip = spellScript.GetSpellSFX().myCastSound;
        GetComponent<AudioSource>().Play();


        myIsCasting = true;
        float castSpeed = spellScript.myCastTime / myStats.myAttackSpeed;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        while (progress <= 1.0f)
        {
            myCastbar.SetCastbarFillAmount(Mathf.Lerp(0, 1, progress));
            myCastbar.SetCastTimeText((castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            if (!spellScript.IsCastableWhileMoving() && IsMoving() || Input.GetKeyDown(KeyCode.Escape))
            {
                myCastbar.SetCastTimeText("Cancelled");
                StopCasting();
                yield break;
            }

            yield return null;
        }

        StopCasting();

        if (IsAbleToCastSpell(spellScript))
        {
            SpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            myAnimator.SetTrigger("CastingDone");
        }
    }

    public IEnumerator SpellChannelRoutine(float aDuration, GameObject aChannelGO, float aStunDuration)
    {
        myStunDuration = aStunDuration;
        myIsCasting = true;
        float castSpeed = aDuration;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;

        myAnimator.SetBool("IsCasting", true);

        while (progress <= 1.0f)
        {
            myCastbar.SetCastbarFillAmount(Mathf.Lerp(1, 0, progress));
            myCastbar.SetCastTimeText((castSpeed - (progress * castSpeed)).ToString("0.0"));

            progress += rate * Time.deltaTime;

            if (IsMoving() || (Input.GetKeyDown(KeyCode.Escape) && aChannelGO != null))
            {
                myCastbar.SetCastTimeText("Cancelled");
                myStunDuration = 0.0f;
                StopCasting();
                Destroy(aChannelGO);
                yield break;
            }

            yield return null;
        }

        StopCasting();
        if (aChannelGO != null)
            Destroy(aChannelGO);
    }

    public void StartChannel(float aDuration, Spell aSpellScript, GameObject aChannelGO, float aStunDuration = 1.0f)
    {
        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(1.0f);
        myCastbar.SetSpellName(aSpellScript.myName);
        myCastbar.SetCastbarColor(aSpellScript.myCastbarColor);
        myCastbar.SetSpellIcon(aSpellScript.mySpellIcon);
        myCastbar.SetCastTimeText(aDuration.ToString());

        GetComponent<AudioSource>().clip = aSpellScript.GetSpellSFX().myCastSound;
        GetComponent<AudioSource>().Play();

        myCastingRoutine = StartCoroutine(SpellChannelRoutine(aDuration, aChannelGO, aStunDuration));
    }

    protected void SpawnSpell(int aKeyIndex, Vector3 aSpawnPosition)
    {
        GameObject spell;
        if (aKeyIndex == -1)
            spell = myClass.GetAutoAttack();
        else
            spell = myClass.GetSpell(aKeyIndex);

        GameObject instance = Instantiate(spell, aSpawnPosition + new Vector3(0.0f, 0.5f, 0.0f), transform.rotation);

        Spell spellScript = instance.GetComponent<Spell>();
        spellScript.SetParent(transform.gameObject);
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);

        if (spellScript.myIsOnlySelfCast)
            spellScript.SetTarget(transform.gameObject);
        else if (Target)
            spellScript.SetTarget(Target);
        else
            spellScript.SetTarget(transform.gameObject);

        if (aSpawnPosition == transform.position)
            GetComponent<AudioSource>().PlayOneShot(spellScript.GetSpellSFX().mySpawnSound);
    }

    protected abstract bool IsAbleToCastSpell(Spell aSpellScript);

    protected Vector3 GetSpellSpawnPosition(Spell aSpellScript)
    {
        if (aSpellScript.mySpeed <= 0.0f && !aSpellScript.myIsOnlySelfCast && Target != null)
        {
            return Target.transform.position;
        }

        return transform.position;
    }

    protected bool CanRaycastToTarget()
    {
        Vector3 hardcodedEyePosition = new Vector3(0.0f, 0.7f, 0.0f);
        Vector3 infrontOfPlayer = (transform.position + hardcodedEyePosition) + transform.forward;
        Vector3 direction = (Target.transform.position + hardcodedEyePosition) - infrontOfPlayer;

        Ray ray = new Ray(infrontOfPlayer, direction);
        float distance = Vector3.Distance(infrontOfPlayer, Target.transform.position + hardcodedEyePosition);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, distance, layerMask))
        {
            return false;
        }

        return true;
    }


    protected void StopCasting()
    {
        if (myCastingRoutine != null)
            StopCoroutine(myCastingRoutine);
        myIsCasting = false;
        myCastbar.FadeOutCastbar();
        GetComponent<AudioSource>().Stop();
        myAnimator.SetBool("IsCasting", false);
    }

    public virtual void Stun(float aStunDuration)
    {
        myStunDuration = aStunDuration;
    }

    public void InterruptSpellCast()
    {
        if (myIsCasting)
        {
            StopCasting();
        }
    }

    protected virtual void SetTarget(GameObject aTarget)
    {
        if (Target != null)
        {
            Target.GetComponent<Health>().EventOnHealthChange -= ChangeTargetHudHealth;
            if (Target.GetComponent<Resource>() != null)
                Target.GetComponent<Resource>().EventOnResourceChange -= ChangeTargetHudResource;
        }

        Target = aTarget;
        if (Target)
        {
            SetTargetHUD();
        }
        else
            myTargetHUD.Hide();
    }

    private void SetTargetHUD()
    {
        myTargetHUD.Show();
        Target.GetComponent<Health>().EventOnHealthChange += ChangeTargetHudHealth;
        ChangeTargetHudHealth(Target.GetComponent<Health>().GetHealthPercentage(),
            Target.GetComponent<Health>().myCurrentHealth.ToString() + "/" + Target.GetComponent<Health>().MaxHealth,
            Target.GetComponent<Health>().GetTotalShieldValue());

        if (Target.GetComponent<Resource>() != null)
        {
            Target.GetComponent<Resource>().EventOnResourceChange += ChangeTargetHudResource;
            ChangeTargetHudResource(Target.GetComponent<Resource>().GetResourcePercentage(),
                Target.GetComponent<Resource>().myCurrentResource.ToString() + "/" + Target.GetComponent<Resource>().MaxResource);
            myTargetHUD.SetResourceBarColor(Target.GetComponent<Resource>().myResourceColor);
        }
        else
        {
            ChangeTargetHudResource(0.0f, "0/0");
        }

        myTargetHUD.SetAvatarSprite(Target.GetComponent<Character>().GetAvatar());
        myTargetHUD.SetClassSprite(Target.GetComponent<Class>().mySprite);
        myTargetHUD.SetHudColor(Target.GetComponent<Character>().myCharacterColor);
        myTargetHUD.SetName(Target.GetComponent<Character>().myName);
    }
    private void ChangeTargetHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue)
    {
        myTargetHUD.SetHealthBarFillAmount(aHealthPercentage);
        myTargetHUD.SetHealthText(aHealthText);
        if (Target && Target.GetComponent<Health>() != null)
            myTargetHUD.SetShieldBar(aShieldValue, Target.GetComponent<Health>().myCurrentHealth);
    }
    private void ChangeTargetHudResource(float aResourcePercentage, string aResourceText)
    {
        myTargetHUD.SetResourceBarFillAmount(aResourcePercentage);
        myTargetHUD.SetResourceText(aResourceText);
        if (Target && Target.GetComponent<Resource>() != null)
            myTargetHUD.SetResourceBarColor(Target.GetComponent<Resource>().myResourceColor);
    }

    protected virtual void ChangeMyHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue)
    {
        myCharacterHUD.SetHealthBarFillAmount(aHealthPercentage);
        myCharacterHUD.SetHealthText(aHealthText);
        myCharacterHUD.SetShieldBar(aShieldValue, myHealth.myCurrentHealth);
    }

    public GameObject GetTarget()
    {
        return Target;
    }

    private void ChangeMyHudResource(float aResourcePercentage, string aResourceText)
    {
        myCharacterHUD.SetResourceBarFillAmount(aResourcePercentage);
        myCharacterHUD.SetResourceText(aResourceText);
        myCharacterHUD.SetResourceBarColor(myResource.myResourceColor);
    }
    protected void HandleBuffs()
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            myBuffs[index].Tick();
            if (myBuffs[index].IsFinished())
            {
                RemoveBuff(index);
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.HOT)
            {
                BuffTickSpell hot = myBuffs[index] as BuffTickSpell;
                if (hot.ShouldDealTickSpellEffect)
                {
                    myHealth.GainHealth(hot.GetTickValue());
                    hot.ShouldDealTickSpellEffect = false;
                }
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.DOT)
            {
                BuffTickSpell dot = myBuffs[index] as BuffTickSpell;
                if (dot.ShouldDealTickSpellEffect)
                {
                    myHealth.TakeDamage(dot.GetTickValue(), dot.GetParent().GetComponent<Character>().myCharacterColor);
                    if (dot.GetParent().tag == "Player")
                        PostMaster.Instance.PostMessage(new Message(MessageType.DamageDealt, new Vector2(dot.GetParent().GetInstanceID(), dot.GetTickValue())));

                    dot.ShouldDealTickSpellEffect = false;
                }
            }
        }
    }

    public void AddBuff(BuffSpell aBuffSpell, Sprite aSpellIcon)
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetParent() == aBuffSpell.GetParent() &&
                myBuffs[index].GetBuff() == aBuffSpell.GetBuff())
            {
                RemoveBuff(index);
                break;
            }
        }

        myBuffs.Add(aBuffSpell);
        aBuffSpell.GetBuff().ApplyBuff(ref myStats);
        myCharacterHUD.AddBuff(aSpellIcon);

        if (aBuffSpell.GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
        }
        else if (aBuffSpell.GetBuff().mySpeedMultiplier != 0.0f)
        {
            myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }
    }

    private void RemoveBuff(int anIndex)
    {
        myBuffs[anIndex].GetBuff().EndBuff(ref myStats);
        if (myBuffs[anIndex].GetBuff().mySpellType == SpellType.Shield)
        {
            //Ugly hack to set shield to 0, which will remove it at myHealth.RemoveShield()
            (myBuffs[anIndex] as BuffShieldSpell).SoakDamage((myBuffs[anIndex] as BuffShieldSpell).GetRemainingShieldHealth());
            myHealth.RemoveShield();
        }

        if (myBuffs[anIndex].GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / myStats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
            else
            {
                myAnimator.SetFloat("AutoAttackSpeed", 1.0f);
            }
        }
        else if (myBuffs[anIndex].GetBuff().mySpeedMultiplier != 0.0f)
        {
            myAnimator.SetFloat("RunSpeed", myStats.mySpeedMultiplier);
        }

        myBuffs.RemoveAt(anIndex);
        myCharacterHUD.RemoveBuff(anIndex);
    }

    public void RemoveBuffByName(string aName)
    {
        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetName() == aName)
            {
                RemoveBuff(index);
                return;
            }
        }
    }

    protected virtual void OnDeath()
    {
        while (myBuffs.Count > 0)
        {
            RemoveBuff(myBuffs.Count - 1);
        }

        StopAllCoroutines();

        GetComponent<AudioSource>().Stop();
        myAnimator.SetTrigger("Death");
    }

    public void SetAvatar(Sprite aSprite)
    {
        myAvatarSprite = aSprite;
    }

    public Sprite GetAvatar()
    {
        return myAvatarSprite;
    }
}