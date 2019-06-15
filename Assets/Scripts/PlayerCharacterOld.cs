﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCharacterOLD : MonoBehaviour
{

   /* public float myBaseSpeed;
    public float myJumpSpeed;
    public float myGravity;

    public Vector3 myDirection;

    public bool myShouldStrafe = true;
    public bool myIsTypingInChat = false;

    public float myStunDuration;
    private float myAutoAttackCooldown;
    private float myAutoAttackCooldownReset = 1.0f;

    [SyncVar]
    private string myName;

    private CharacterController myController;
    private Camera myCamera;

    private Animator myAnimator;
    private NetworkAnimator myNetAnimator;
    private Class myClass;
    private Health myHealth;
    private Resource myResource;

    private Stats myStats;

    private List<BuffSpell> myBuffs;

    private Castbar myCastbar;
    private Coroutine myCastingRoutine;

    [SyncVar]
    private bool myIsCasting;
    private bool myShouldAutoAttack;

    [SyncVar]
    private GameObject myTarget;

    private CharacterHUD myCharacterHUD;
    private CharacterHUD myTargetHUD;

    private UIManager myUIManager;

    private bool myIsGrounded;

    private string previousAnimationName;

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        myCamera = Camera.main;
        myCamera.GetComponent<PlayerCamera>().SetTarget(this.transform);

        if (isServer)
            ManualStart();
    }

    private void Start()
    {
        myController = transform.GetComponent<CharacterController>();

        myAnimator = GetComponent<Animator>();
        myNetAnimator = GetComponent<NetworkAnimator>();

        myCastbar = GameObject.Find("Castbar Background").GetComponent<Castbar>();
        myCharacterHUD = GameObject.Find("PlayerHud").GetComponent<CharacterHUD>();
        myTargetHUD = GameObject.Find("TargetHud").GetComponent<CharacterHUD>();

        myUIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        myHealth = GetComponent<Health>();
        myResource = GetComponent<Resource>();

        myStats = GetComponent<Stats>();
        myBuffs = new List<BuffSpell>();

        myClass = GetComponentInChildren<Class>();

        if (hasAuthority)
            ManualStart();
    }

    private void ManualStart()
    {
        myHealth.EventOnHealthChange += ChangeMyHudHealth;
        myHealth.EventOnHealthZero += OnDeath;

        ChangeMyHudHealth(myHealth.GetHealthPercentage(), myHealth.myCurrentHealth.ToString() + "/" + myHealth.myMaxHealth.ToString(), GetComponent<Health>().GetTotalShieldValue());

        myResource.EventOnResourceChange += ChangeMyHudResource;
        ChangeMyHudResource(myResource.GetResourcePercentage(), myResource.myCurrentResource.ToString() + "/" + myResource.MaxResource.ToString());

        myClass.SetupSpellHud(CastSpell, null);

        myCharacterHUD.SetName(myName + " (" + myClass.myClassName + ")");
        myCharacterHUD.Show();
    }

    void Update()
    {
        if (!hasAuthority)
            return;

        string currentAnimation = myAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (currentAnimation != previousAnimationName)
            Debug.Log("Previous: " + previousAnimationName + "    Current: " + currentAnimation);
        previousAnimationName = currentAnimation;

        myDirection.y -= myGravity * Time.deltaTime;
        myController.Move(myDirection * Time.deltaTime);

        //myController.isGrounded unstable further ahead is seems...
        myIsGrounded = myController.isGrounded;
        if (!myAnimator.GetBool("IsGrounded") && myIsGrounded)
            myAnimator.SetBool("IsGrounded", true);

        if (!myIsTypingInChat && myStunDuration <= 0.0f && !GetComponent<Health>().IsDead())
        {
            DetectMovementInput();
            DetectPressedSpell();
            RotatePlayer();
        }
        else if (myStunDuration > 0.0f)
        {
            myStunDuration -= Time.deltaTime;
        }

        DetectMouseClick();

        HandleBuffs();

        if (myShouldAutoAttack)
            AutoAttack();
    }

    private void DetectMovementInput()
    {
        if (!myIsGrounded)
            return;

        if (myShouldStrafe)
        {
            myDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized * myBaseSpeed * myStats.mySpeedMultiplier;
        }
        else
        {
            myDirection = new Vector3(0.0f, 0.0f, Input.GetAxisRaw("Vertical")) * myBaseSpeed * myStats.mySpeedMultiplier;
        }

        myDirection = transform.TransformDirection(myDirection);

        myAnimator.SetBool("IsRunning", IsMoving());

        if (Input.GetButtonDown("Jump"))
        {
            myDirection.y = myJumpSpeed;
            myAnimator.SetBool("IsGrounded", false);
            myAnimator.SetTrigger("Jump");
            myNetAnimator.SetTrigger("Jump");
        }
    }

    private void DetectMouseClick()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastNewTarget();

                if (Input.GetMouseButtonDown(0))
                {
                    myShouldAutoAttack = false;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (myTarget != null && myTarget.tag == "Enemy" && !GetComponent<Health>().IsDead() && !myTarget.GetComponent<Health>().IsDead())
                        myShouldAutoAttack = true;
                }
            }
        }
    }

    private void DetectPressedSpell()
    {
        if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                int keycodeIndex = (int)c;

                if (keycodeIndex > 48 && keycodeIndex < 57)
                {
                    keycodeIndex -= 49;
                    CastSpell(keycodeIndex);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            myShouldAutoAttack = false;
        }
    }

    private void RotatePlayer()
    {
        if (myShouldStrafe)
        {
            if (Mathf.Abs(myDirection.x) > 0.0f || Mathf.Abs(myDirection.y) > 0.0f)
            {
                Vector3 newRotation = transform.eulerAngles;
                newRotation.y = myCamera.transform.eulerAngles.y;
                transform.eulerAngles = newRotation;
            }
        }
        else
        {
            if (Input.GetButton("Horizontal"))
            {
                Vector3 newRotation = transform.eulerAngles;
                newRotation.y += Input.GetAxisRaw("Horizontal") * 2.0f;
                transform.eulerAngles = newRotation;
            }
        }
    }

    private void HandleBuffs()
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
                    AIPostMaster.Instance.PostAIMessage(new AIMessage(AIMessageType.SpellSpawned, new AIMessageData(GetInstanceID(), hot.GetTickValue())));
                    CmdGainHealth(hot.GetTarget(), hot.GetTickValue());
                    hot.ShouldDealTickSpellEffect = false;
                }
            }
            else if (myBuffs[index].GetBuff().mySpellType == SpellType.DOT)
            {
                BuffTickSpell dot = myBuffs[index] as BuffTickSpell;
                if (dot.ShouldDealTickSpellEffect)
                {
                    CmdTakeDamage(dot.GetTarget(), dot.GetTickValue());
                    dot.ShouldDealTickSpellEffect = false;
                }
            }
        }
    }

    private void AutoAttack()
    {
        if (!hasAuthority)
            return;

        if (myAutoAttackCooldown > 0.0f)
        {
            myAutoAttackCooldown -= Time.deltaTime * myStats.myAttackSpeed;
            return;
        }

        GameObject spell = myClass.GetAutoAttack();
        Spell spellScript = spell.GetComponent<Spell>();

        if (!IsAbleToAutoAttack())
        {
            return;
        }

        myNetAnimator.SetTrigger("Attack");
        myAutoAttackCooldown = 1.2f;

        CmdSpawnSpell(-1, GetSpellSpawnPosition(spellScript));
    }

    private void OnDeath()
    {
        myShouldAutoAttack = false;
        while (myBuffs.Count > 0)
        {
            RemoveBuff(myBuffs.Count - 1);
        }

        myAnimator.SetTrigger("Death");
        myNetAnimator.SetTrigger("Death");
        myHealth.EventOnHealthZero -= OnDeath;
    }

    public void CastSpell(int aKeyIndex)
    {
        if (!hasAuthority)
            return;

        if (GetComponent<Health>().IsDead())
            return;

        if (myClass.IsSpellOnCooldown(aKeyIndex))
        {
            myUIManager.CreateErrorMessage("Can't cast that spell yet");
            return;
        }

        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (!IsAbleToCastSpell(spellScript))
            return;

        if (spellScript.myCastTime <= 0.0f)
        {
            CmdSpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            myResource.LoseResource(spellScript.myResourceCost);
            myAnimator.SetTrigger("CastingDone");
            myNetAnimator.SetTrigger("CastingDone");
            return;
        }

        myAnimator.SetBool("IsCasting", true);

        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(0.0f);
        myCastbar.SetSpellName(spellScript.myName);
        myCastbar.SetCastbarColor(spellScript.myCastbarColor);
        myCastbar.SetSpellIcon(spellScript.mySpellIcon);
        myCastbar.SetCastTimeText(spellScript.myCastTime.ToString());

        myCastingRoutine = StartCoroutine(CastbarProgress(aKeyIndex));
    }

    private void CmdSpawnSpell(int aKeyIndex, Vector3 aSpawnPosition)
    {
        GameObject spell;
        if (aKeyIndex == -1)
            spell = myClass.GetAutoAttack();
        else
            spell = myClass.GetSpell(aKeyIndex);

        GameObject instance = Instantiate(spell, aSpawnPosition + new Vector3(0.0f, 0.5f, 0.0f), transform.rotation);

        Spell spellScript = instance.GetComponent<Spell>();
        spellScript.AddDamageIncrease(myStats.myDamageIncrease);
        spellScript.SetParent(transform.gameObject);

        if (spellScript.myIsOnlySelfCast)
            spellScript.SetTarget(transform.gameObject);
        else if (myTarget)
            spellScript.SetTarget(myTarget);
        else
            spellScript.SetTarget(transform.gameObject);

        NetworkServer.Spawn(instance);
    }

    private IEnumerator CastbarProgress(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();


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
            CmdSpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            myResource.LoseResource(spellScript.myResourceCost);
            myAnimator.SetTrigger("CastingDone");
            myNetAnimator.SetTrigger("CastingDone");
        }
    }

    public IEnumerator SpellChannelRoutine(float aDuration, GameObject aChannelGO)
    {
        myStunDuration = aDuration;
        myIsCasting = true;
        float castSpeed = aDuration;
        float rate = 1.0f / castSpeed;
        float progress = 0.0f;


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
                CmdDestroyChannelledSpell(aChannelGO);
                yield break;
            }

            yield return null;
        }

        StopCasting();
        if (aChannelGO != null)
            CmdDestroyChannelledSpell(aChannelGO);
    }

    [Command]
    private void CmdDestroyChannelledSpell(GameObject aChannelGO)
    {
        aChannelGO.GetComponent<ChannelSpell>().RpcSetToDestroy();
    }

    public void StartChannel(float aDuration, Spell aSpellScript, GameObject aChannelGO)
    {
        if (!hasAuthority)
            return;

        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(1.0f);
        myCastbar.SetSpellName(aSpellScript.myName);
        myCastbar.SetCastbarColor(aSpellScript.myCastbarColor);
        myCastbar.SetSpellIcon(aSpellScript.mySpellIcon);
        myCastbar.SetCastTimeText(aDuration.ToString());

        myCastingRoutine = StartCoroutine(SpellChannelRoutine(aDuration, aChannelGO));
    }

    private Vector3 GetSpellSpawnPosition(Spell aSpellScript)
    {
        if (aSpellScript.mySpeed <= 0.0f && !aSpellScript.myIsOnlySelfCast && myTarget != null)
        {
            return myTarget.transform.position;
        }

        return transform.position;
    }

    public void GiveImpulse(Vector3 aVelocity, bool aShouldLookAtDirection)
    {
        myStunDuration = 0.2f;
        myDirection = aVelocity;

        if (aShouldLookAtDirection)
            transform.LookAt(myTarget.transform);
    }

    public void SetPosition(Vector3 aPosition)
    {
        transform.position = aPosition;
    }

    public void Stun(float aStunDuration)
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

    public void AddBuff(BuffSpell aBuffSpell, Sprite aSpellIcon)
    {
        if (!hasAuthority)
            return;

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
        if (!hasAuthority)
            return;

        for (int index = 0; index < myBuffs.Count; index++)
        {
            if (myBuffs[index].GetName() == aName)
            {
                RemoveBuff(index);
                return;
            }
        }
    }

    private void StopCasting()
    {
        if (myCastingRoutine != null)
            StopCoroutine(myCastingRoutine);
        myIsCasting = false;
        myCastbar.FadeOutCastbar();

        myAnimator.SetBool("IsCasting", false);
    }

    private bool CanRaycastToTarget()
    {
        Vector3 hardcodedEyePosition = new Vector3(0.0f, 0.7f, 0.0f);
        Vector3 infrontOfPlayer = (transform.position + hardcodedEyePosition) + transform.forward;
        Vector3 direction = (myTarget.transform.position + hardcodedEyePosition) - infrontOfPlayer;

        Ray ray = new Ray(infrontOfPlayer, direction);
        float distance = Vector3.Distance(infrontOfPlayer, myTarget.transform.position + hardcodedEyePosition);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        if (Physics.Raycast(ray, distance, layerMask))
        {
            return false;
        }

        return true;
    }

    private bool IsAbleToAutoAttack()
    {
        if (myIsCasting)
        {
            myUIManager.CreateErrorMessage("Already casting another spell!");
            return false;
        }

        if (!myTarget)
        {
            return false;
        }

        if (myTarget.GetComponent<Health>().IsDead())
        {
            myUIManager.CreateErrorMessage("That target is dead!");
            myShouldAutoAttack = false;
            return false;
        }

        if (!CanRaycastToTarget())
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, myTarget.transform.position);
        if (distance > 3.0f)
        {
            return false;
        }

        return true;
    }

    private bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            myUIManager.CreateErrorMessage("Already casting another spell!");
            return false;
        }

        if (myResource.myCurrentResource < aSpellScript.myResourceCost)
        {
            myUIManager.CreateErrorMessage("Not enough resource to cast");
            return false;
        }

        if (!aSpellScript.IsCastableWhileMoving() && IsMoving())
        {
            myUIManager.CreateErrorMessage("Can't cast while moving!");
            return false;
        }

        if (aSpellScript.myIsOnlySelfCast)
            return true;

        if (!myTarget)
        {
            if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) != 0)
            {
                myTarget = gameObject;
            }
            else
            {
                myUIManager.CreateErrorMessage("No Target!");
                return false;
            }
        }

        if (myTarget.GetComponent<Health>().IsDead())
        {
            myUIManager.CreateErrorMessage("That target is dead!");
            return false;
        }

        if (!CanRaycastToTarget())
        {
            myUIManager.CreateErrorMessage("Target not in line of sight!");
            return false;
        }

        float distance = Vector3.Distance(transform.position, myTarget.transform.position);
        if (distance > aSpellScript.myRange)
        {
            myUIManager.CreateErrorMessage("Out of range!");
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Enemy) == 0 && myTarget.tag == "Enemy")
        {
            myUIManager.CreateErrorMessage("Can't cast friendly spells on enemies");
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) == 0 && myTarget.tag == "Player")
        {
            myUIManager.CreateErrorMessage("Can't cast hostile spells on friends.");
            return false;
        }

        if (!aSpellScript.myCanCastOnSelf && myTarget == transform.gameObject)
        {
            myUIManager.CreateErrorMessage("Can't be cast on self!");
            return false;
        }

        return true;
    }

    private void RaycastNewTarget()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask targetMask = LayerMask.GetMask("Targetable");

        if (Physics.Raycast(ray, out hit, 100.0f, targetMask, QueryTriggerInteraction.Collide))
        {
            if (myTarget != null)
            {
                myTarget.GetComponentInChildren<Projector>().enabled = false;
                myTarget.GetComponent<Health>().EventOnHealthChange -= ChangeTargetHudHealth;
                if (myTarget.GetComponent<Resource>() != null)
                    myTarget.GetComponent<Resource>().EventOnResourceChange -= ChangeTargetHudResource;
            }

            GameObject target = FindParentWithNetworkIdentity(hit.collider.transform.gameObject);
            myTarget = target;
            CmdSetTarget(target);


            myTarget.GetComponentInChildren<Projector>().enabled = true;
            SetTargetHUD();
        }
        else
        {
            if (myTarget != null)
            {
                myTarget.GetComponentInChildren<Projector>().enabled = false;
                myTarget.GetComponent<Health>().EventOnHealthChange -= ChangeTargetHudHealth;
                if (myTarget.GetComponent<Resource>() != null)
                    myTarget.GetComponent<Resource>().EventOnResourceChange -= ChangeTargetHudResource;
            }

            CmdSetTarget(null);
            myTargetHUD.Hide();
            myShouldAutoAttack = false;
        }
    }

    public void AssignTargetUI(GameObject aTarget)
    {
        if (!hasAuthority)
            return;

        if (myTarget != null)
        {
            myTarget.GetComponentInChildren<Projector>().enabled = false;
            myTarget.GetComponent<Health>().EventOnHealthChange -= ChangeTargetHudHealth;
            if (myTarget.GetComponent<Resource>() != null)
                myTarget.GetComponent<Resource>().EventOnResourceChange -= ChangeTargetHudResource;
        }

        myTarget = aTarget;
        CmdSetTarget(aTarget);


        myTarget.GetComponentInChildren<Projector>().enabled = true;
        SetTargetHUD();
    }

    private bool IsMoving()
    {
        if (myDirection.x != 0 && myDirection.z != 0)
            return true;

        if (!myIsGrounded)
            return true;

        return false;
    }

    [Command]
    private void CmdSetTarget(GameObject aTarget)
    {
        myTarget = aTarget;
    }

    private void SetTargetHUD()
    {
        myTargetHUD.Show();
        myTarget.GetComponent<Health>().EventOnHealthChange += ChangeTargetHudHealth;
        ChangeTargetHudHealth(myTarget.GetComponent<Health>().GetHealthPercentage(),
            myTarget.GetComponent<Health>().myCurrentHealth.ToString() + "/" + myTarget.GetComponent<Health>().MaxHealth,
            myTarget.GetComponent<Health>().GetTotalShieldValue());

        if (myTarget.GetComponent<Resource>() != null)
        {
            myTarget.GetComponent<Resource>().EventOnResourceChange += ChangeTargetHudResource;
            ChangeTargetHudResource(myTarget.GetComponent<Resource>().GetResourcePercentage(),
                myTarget.GetComponent<Resource>().myCurrentResource.ToString() + "/" + myTarget.GetComponent<Resource>().MaxResource);
            myTargetHUD.SetResourceBarColor(myTarget.GetComponent<Resource>().myResourceColor);
        }
        else
        {
            ChangeTargetHudResource(0.0f, "0/0");
        }


        if (myTarget.tag == "Enemy")
        {
            myTargetHUD.SetName(myTarget.name);
            myTargetHUD.SetNameColor(Color.red);
        }
        else if (myTarget.tag == "Player")
        {
            if (myTarget.GetComponent<Player>() != null)
                myTargetHUD.SetName(myTarget.name);
            myTargetHUD.SetNameColor(new Color(120f / 255f, 1.0f, 0.0f));
        }
    }

    private void ChangeTargetHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue)
    {
        myTargetHUD.SetHealthBarFillAmount(aHealthPercentage);
        myTargetHUD.SetHealthText(aHealthText);
        myTargetHUD.SetShieldBar(aShieldValue, myTarget.GetComponent<Health>().myCurrentHealth);
    }

    private void ChangeTargetHudResource(float aResourcePercentage, string aResourceText)
    {
        myTargetHUD.SetResourceBarFillAmount(aResourcePercentage);
        myTargetHUD.SetResourceText(aResourceText);
        if (myTarget.GetComponent<Resource>() != null)
            myTargetHUD.SetResourceBarColor(myTarget.GetComponent<Resource>().myResourceColor);
    }

    private void ChangeMyHudHealth(float aHealthPercentage, string aHealthText, int aShieldValue)
    {
        if (!hasAuthority)
            return;

        myCharacterHUD.SetHealthBarFillAmount(aHealthPercentage);
        myCharacterHUD.SetHealthText(aHealthText);
        myCharacterHUD.SetShieldBar(aShieldValue, myHealth.myCurrentHealth);
    }

    private void ChangeMyHudResource(float aResourcePercentage, string aResourceText)
    {
        if (!hasAuthority)
            return;

        myCharacterHUD.SetResourceBarFillAmount(aResourcePercentage);
        myCharacterHUD.SetResourceText(aResourceText);
        myCharacterHUD.SetResourceBarColor(myResource.myResourceColor);
    }

    private GameObject FindParentWithNetworkIdentity(GameObject aGameObject)
    {
        if (aGameObject.GetComponent<NetworkIdentity>() != null)
            return aGameObject;

        return FindParentWithNetworkIdentity(aGameObject.transform.parent.gameObject);
    }

    public string Name
    {
        get { return myName + " (" + myClass.myClassName + ")"; }
        set
        {
            myName = value;

            if (hasAuthority && myCharacterHUD != null && myClass != null)
                myCharacterHUD.SetName(value + " (" + myClass.myClassName + ")");
        }
    }

    public bool IsCasting(bool aNonAuthorityReturn)
    {
        if (!hasAuthority)
            return aNonAuthorityReturn;

        return myIsCasting;
    }

    public void WriteErrorMessage(string aText)
    {
        if (!hasAuthority)
            return;

        myUIManager.CreateErrorMessage(aText);
    }

    [Command]
    public void CmdGainHealth(GameObject aTarget, int aValue)
    {
        aTarget.GetComponent<Health>().GainHealth(aValue);
    }

    [Command]
    public void CmdTakeDamage(GameObject aTarget, int aValue)
    {
        aTarget.GetComponent<Health>().TakeDamage(aValue);
    }*/
}
