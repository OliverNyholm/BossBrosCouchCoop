using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int myControllerIndex;
    public float myBaseSpeed;
    public float myJumpSpeed;
    public float myGravity;

    private CharacterController myController;
    private Animator myAnimator;
    private Class myClass;

    private TargetHandler myTargetHandler;
    private GameObject myTarget;

    private CharacterHUD myCharacterHUD;
    private CharacterHUD myTargetHUD;
    private Castbar myCastbar;
    private UIManager myUIManager;

    private Coroutine myCastingRoutine;

    private List<BuffSpell> myBuffs;

    private Vector3 myDirection;
    private CameraXZTransform myCameraXZTransform;
    private DpadInput myDpadInput;

    private float myStunDuration;
    private float myAutoAttackCooldown;
    private float myAutoAttackCooldownReset = 1.0f;

    private bool myIsGrounded;
    private bool myIsCasting;

    void Start()
    {
        myCameraXZTransform.myForwards = Camera.main.transform.forward;
        myCameraXZTransform.myForwards.y = 0.0f;
        myCameraXZTransform.myForwards.Normalize();

        myCameraXZTransform.myRight = Camera.main.transform.right;
        myCameraXZTransform.myRight.y = 0.0f;
        myCameraXZTransform.myRight.Normalize();

        myDpadInput = new DpadInput(myControllerIndex);

        myDirection = Vector3.zero;

        myTargetHandler = GameObject.Find("GameManager").GetComponent<TargetHandler>();
        myUIManager = GameObject.Find("GameManager").GetComponent<UIManager>();

        myController = GetComponent<CharacterController>();
        myAnimator = GetComponent<Animator>();
        myClass = GetComponent<Class>();

        SetupHud(GameObject.Find("PlayerUI" + myControllerIndex).transform);

        myBuffs = new List<BuffSpell>();

        myTarget = null;
        myIsCasting = false;
    }

    private void SetupHud(Transform aUIParent)
    {
        aUIParent.GetComponent<CanvasGroup>().alpha = 1.0f;

        myCharacterHUD = aUIParent.transform.Find("PlayerHud").GetComponent<CharacterHUD>();
        myTargetHUD = aUIParent.transform.Find("TargetHud").GetComponent<CharacterHUD>();
        myCastbar = aUIParent.transform.Find("Castbar Background").GetComponent<Castbar>();

        myCharacterHUD.SetName(transform.name);
        myClass.SetupSpellHud(CastSpell, aUIParent);


        Health health = GetComponent<Health>();
        health.EventOnHealthChange += ChangeMyHudHealth;
        health.EventOnHealthZero += OnDeath;

        ChangeMyHudHealth(health.GetHealthPercentage(), health.myCurrentHealth.ToString() + "/" + health.myMaxHealth.ToString(), GetComponent<Health>().GetTotalShieldValue());

        Resource resource = GetComponent<Resource>();
        resource.EventOnResourceChange += ChangeMyHudResource;
        ChangeMyHudResource(resource.GetResourcePercentage(), resource.myCurrentResource.ToString() + "/" + resource.MaxResource.ToString());
    }

    void Update()
    {
        myDpadInput.Update();

        myDirection.y -= myGravity * Time.deltaTime;
        myController.Move(myDirection * Time.deltaTime);

        //myController.isGrounded unstable further ahead is seems...
        myIsGrounded = myController.isGrounded;
        if (!myAnimator.GetBool("IsGrounded") && myIsGrounded)
            myAnimator.SetBool("IsGrounded", true);

        DetectTargetingInput();
        DetectMovementInput();
        DetectSpellInput();
    }
    private void DetectMovementInput()
    {
        if (!myIsGrounded)
            return;

        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("LeftHorizontal" + myControllerIndex), 0.0f, Input.GetAxisRaw("LeftVertical" + myControllerIndex));

        myDirection = (inputDirection.x * myCameraXZTransform.myRight + inputDirection.z * myCameraXZTransform.myForwards).normalized;
        myDirection *= myBaseSpeed * GetComponent<Stats>().mySpeedMultiplier;

        bool isMoving = IsMoving();
        if (isMoving)
            RotatePlayer();

        myAnimator.SetBool("IsRunning", isMoving);

        if (Input.GetButton("RightBumper" + myControllerIndex))
        {
            myDirection.y = myJumpSpeed;
            myAnimator.SetBool("IsGrounded", false);
            myAnimator.SetTrigger("Jump");
        }
    }

    private void DetectSpellInput()
    {
        if (Input.GetButtonDown("A" + myControllerIndex))
            CastSpell(0);
        else if (Input.GetButtonDown("B" + myControllerIndex))
            CastSpell(1);
        else if (Input.GetButtonDown("X" + myControllerIndex))
            CastSpell(2);
        else if (Input.GetButtonDown("Y" + myControllerIndex))
            CastSpell(3);
        else if (myDpadInput.IsDPADPressed(DpadInput.DPADButton.Down))
            CastSpell(4);
        else if (myDpadInput.IsDPADPressed(DpadInput.DPADButton.Right))
            CastSpell(5);
        else if (myDpadInput.IsDPADPressed(DpadInput.DPADButton.Left))
            CastSpell(6);
        else if (myDpadInput.IsDPADPressed(DpadInput.DPADButton.Up))
            CastSpell(7);

    }
    private void RotatePlayer()
    {
        transform.rotation = Quaternion.LookRotation(myDirection, Vector3.up);
    }
    private bool IsMoving()
    {
        if (myDirection.x != 0 || myDirection.z != 0)
            return true;

        if (!myIsGrounded)
            return true;

        return false;
    }

    public void CastSpell(int aKeyIndex)
    {
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
            SpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            myAnimator.SetTrigger("CastingDone");
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

    private IEnumerator CastbarProgress(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();


        myIsCasting = true;
        float castSpeed = spellScript.myCastTime / GetComponent<Stats>().myAttackSpeed;
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
                Destroy(aChannelGO);
                yield break;
            }

            yield return null;
        }

        StopCasting();
        if (aChannelGO != null)
            Destroy(aChannelGO);
    }

    public void StartChannel(float aDuration, Spell aSpellScript, GameObject aChannelGO)
    {
        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(1.0f);
        myCastbar.SetSpellName(aSpellScript.myName);
        myCastbar.SetCastbarColor(aSpellScript.myCastbarColor);
        myCastbar.SetSpellIcon(aSpellScript.mySpellIcon);
        myCastbar.SetCastTimeText(aDuration.ToString());

        myCastingRoutine = StartCoroutine(SpellChannelRoutine(aDuration, aChannelGO));
    }

    private void SpawnSpell(int aKeyIndex, Vector3 aSpawnPosition)
    {
        GameObject spell;
        if (aKeyIndex == -1)
            spell = myClass.GetAutoAttack();
        else
            spell = myClass.GetSpell(aKeyIndex);

        GameObject instance = Instantiate(spell, aSpawnPosition + new Vector3(0.0f, 0.5f, 0.0f), transform.rotation);

        Spell spellScript = instance.GetComponent<Spell>();
        spellScript.SetParent(transform.gameObject);
        spellScript.AddDamageIncrease(GetComponent<Stats>().myDamageIncrease);

        if (spellScript.myIsOnlySelfCast)
            spellScript.SetTarget(transform.gameObject);
        else if (myTarget)
            spellScript.SetTarget(myTarget);
        else
            spellScript.SetTarget(transform.gameObject);
    }

    private bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            myUIManager.CreateErrorMessage("Already casting another spell!");
            return false;
        }

        if (GetComponent<Resource>().myCurrentResource < aSpellScript.myResourceCost)
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


    private Vector3 GetSpellSpawnPosition(Spell aSpellScript)
    {
        if (aSpellScript.mySpeed <= 0.0f && !aSpellScript.myIsOnlySelfCast && myTarget != null)
        {
            return myTarget.transform.position;
        }

        return transform.position;
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


    private void StopCasting()
    {
        if (myCastingRoutine != null)
            StopCoroutine(myCastingRoutine);
        myIsCasting = false;
        myCastbar.FadeOutCastbar();

        myAnimator.SetBool("IsCasting", false);
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

    private void DetectTargetingInput()
    {
        int targetIndex = -1;
        int horizontalAxis = (int)Input.GetAxisRaw("RightHorizontal" + myControllerIndex);
        int verticalAxis = (int)Input.GetAxisRaw("RightVertical" + myControllerIndex);

        if (horizontalAxis != 0)
            targetIndex = horizontalAxis > 0 ? 1 : 3;
        else if (verticalAxis != 0)
            targetIndex = verticalAxis > 0 ? 0 : 2;

        if (targetIndex != -1)
        {
            SetTarget(GameObject.Find("GameManager").GetComponent<TargetHandler>().GetPlayer(targetIndex));
            return;
        }

        if (Input.GetButtonDown("LeftBumper" + myControllerIndex))
            SetTarget(GameObject.Find("GameManager").GetComponent<TargetHandler>().GetEnemy(myControllerIndex));
    }

    private void SetTarget(GameObject aTarget)
    {
        if (myTarget != null)
        {
            myTarget.GetComponentInChildren<Projector>().enabled = false;
            myTarget.GetComponent<Health>().EventOnHealthChange -= ChangeTargetHudHealth;
            if (myTarget.GetComponent<Resource>() != null)
                myTarget.GetComponent<Resource>().EventOnResourceChange -= ChangeTargetHudResource;
        }

        myTarget = aTarget;
        if (myTarget)
            SetTargetHUD();
        else
            myTargetHUD.Hide();
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
            myTargetHUD.SetName(myTarget.GetComponent<Enemy>().myName);
            myTargetHUD.SetNameColor(Color.red);
        }
        else if (myTarget.tag == "Player")
        {
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
        myCharacterHUD.SetHealthBarFillAmount(aHealthPercentage);
        myCharacterHUD.SetHealthText(aHealthText);
        myCharacterHUD.SetShieldBar(aShieldValue, GetComponent<Health>().myCurrentHealth);
    }

    private void ChangeMyHudResource(float aResourcePercentage, string aResourceText)
    {
        myCharacterHUD.SetResourceBarFillAmount(aResourcePercentage);
        myCharacterHUD.SetResourceText(aResourceText);
        myCharacterHUD.SetResourceBarColor(GetComponent<Resource>().myResourceColor);
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

        Stats stats = GetComponent<Stats>();

        myBuffs.Add(aBuffSpell);
        aBuffSpell.GetBuff().ApplyBuff(ref stats);
        myCharacterHUD.AddBuff(aSpellIcon);

        if (aBuffSpell.GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / stats.myAttackSpeed;

            float currentAnimationSpeed = 1.0f;
            if (currentAnimationSpeed > attackspeed)
            {
                myAnimator.SetFloat("AutoAttackSpeed", myAnimator.GetFloat("AutoAttackSpeed") + attackspeed / currentAnimationSpeed);
            }
        }
        else if (aBuffSpell.GetBuff().mySpeedMultiplier != 0.0f)
        {
            myAnimator.SetFloat("RunSpeed", stats.mySpeedMultiplier);
        }
    }

    private void RemoveBuff(int anIndex)
    {
        Stats stats = GetComponent<Stats>();
        myBuffs[anIndex].GetBuff().EndBuff(ref stats);
        if (myBuffs[anIndex].GetBuff().mySpellType == SpellType.Shield)
        {
            //Ugly hack to set shield to 0, which will remove it at myHealth.RemoveShield()
            (myBuffs[anIndex] as BuffShieldSpell).SoakDamage((myBuffs[anIndex] as BuffShieldSpell).GetRemainingShieldHealth());
            GetComponent<Health>().RemoveShield();
        }

        if (myBuffs[anIndex].GetBuff().myAttackSpeed != 0.0f)
        {
            float attackspeed = myAutoAttackCooldownReset / stats.myAttackSpeed;

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
            myAnimator.SetFloat("RunSpeed", stats.mySpeedMultiplier);
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

    private void OnDeath()
    {
        // myShouldAutoAttack = false;
        while (myBuffs.Count > 0)
        {
            RemoveBuff(myBuffs.Count - 1);
        }

        myAnimator.SetTrigger("Death");
        GetComponent<Health>().EventOnHealthZero -= OnDeath;
    }

    public int GetControllerIndex()
    {
        return myControllerIndex;
    }
}