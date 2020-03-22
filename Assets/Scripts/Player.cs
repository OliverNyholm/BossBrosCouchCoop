using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Player : Character
{
    private CharacterController myController;

    private TargetHandler myTargetHandler;

    private SpellErrorHandler mySpellErrorHandler;

    private PlayerControls myPlayerControls;

    private Vector3 myVelocity;
    private CameraXZTransform myCameraXZTransform;

    private bool myIsGrounded;
    private bool myShouldAutoAttack;

    private float myStartTimeOfHoldingKeyDown;
    private float myStartTimeOfReleasingHealingKeyDown;
    private int myFriendlySpellKeyHeldDownIndex;
    private bool myIsFriendlySpellKeyHeldDown;
    private bool myIsHealTargetingEnabled;

    [Header("The duration of holding down a spell button before enabling targeting system")]
    [SerializeField]
    private float mySmartTargetHoldDownMaxDuration = 0.35f;

    public int PlayerIndex { get; set; }

    protected override void Start()
    {
        base.Start();

        myCameraXZTransform.myForwards = Camera.main.transform.forward;
        myCameraXZTransform.myForwards.y = 0.0f;
        myCameraXZTransform.myForwards.Normalize();

        myCameraXZTransform.myRight = Camera.main.transform.right;
        myCameraXZTransform.myRight.y = 0.0f;
        myCameraXZTransform.myRight.Normalize();

        myVelocity = Vector3.zero;

        myTargetHandler = GameObject.Find("GameManager").GetComponent<TargetHandler>();

        myController = GetComponent<CharacterController>();

        Transform uiHud = GameObject.Find("PlayerHud" + PlayerIndex).transform;
        SetupHud(uiHud);

        mySpellErrorHandler = uiHud.GetComponentInChildren<SpellErrorHandler>();
        myClass.SetupSpellHud(CastSpell, uiHud);
    }

    protected override void Update()
    {
        if (PauseMenu.ourIsGamePaused)
            return;

        base.Update();

        myVelocity.y -= myGravity * Time.deltaTime;
        myController.Move(myVelocity * Time.deltaTime);

        //myController.isGrounded unstable further ahead is seems...
        myIsGrounded = IsGrounded();
        if (!myAnimator.GetBool("IsGrounded") && myIsGrounded)
            myAnimator.SetBool("IsGrounded", true);

        DetectTargetingInput();
        if (GetComponent<Health>().IsDead())
            return;

        if (IsStunned())
            return;

        SlideOnAngledSurface();
        DetectStartHealTargeting();
        DetectLanding();
        DetectInput();

        if (myShouldAutoAttack)
            AutoAttack();
    }

    bool IsGrounded()
    {
        if (myVelocity.y > 0.0f)
            return false;

        const float offsetLength = 0.3f;
        Vector3 offset = new Vector3(0.0f, offsetLength, 0.0f);

        float distance = 0.3f + offsetLength;
        Ray ray = new Ray(transform.position + offset, Vector3.down);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            if (Vector3.Dot(hitInfo.normal, Vector3.down) < -0.5f)
                return true;
        }

        return false;
    }

    private void SlideOnAngledSurface()
    {
        if (myIsGrounded)
            return;

        const float offsetLength = 0.3f;
        Vector3 offset = new Vector3(0.0f, offsetLength, 0.0f);

        float distance = 0.3f + offsetLength;
        Ray ray = new Ray(transform.position + offset, Vector3.down);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        Vector3 pushDirection = Vector3.zero;
        Vector3 horizontalSlopeNormal = Vector3.zero;
        if (DoesRayHitSlope(ray, distance, layerMask, ref horizontalSlopeNormal))
        {
            pushDirection += horizontalSlopeNormal;
        }

        const int circlePoints = 7;
        float radius = myController.radius;
        for (int index = 0; index < circlePoints; index++)
        {
            float angle = (Mathf.PI * 2.0f) / circlePoints * index;
            ray.origin = transform.position + new Vector3(radius * Mathf.Cos(angle), offsetLength, radius * Mathf.Sin(angle));
            if (DoesRayHitSlope(ray, distance, layerMask, ref horizontalSlopeNormal))
            {
                pushDirection += horizontalSlopeNormal;
            }
        }

        myVelocity += pushDirection.normalized;
    }

    public bool DoesRayHitSlope(Ray aRay, float aDistance, LayerMask aLayermask, ref Vector3 outNormal)
    {
        Debug.DrawLine(aRay.origin, aRay.origin + Vector3.down * aDistance);

        RaycastHit hitInfo;
        if (Physics.Raycast(aRay, out hitInfo, aDistance, (int)aLayermask))
        {
            if (Vector3.Dot(hitInfo.normal, Vector3.up) < 0.5f)
            {
                outNormal = hitInfo.normal;
                outNormal.y = 0.0f;
                return true;
            }
        }

        return false;
    }

    private void DetectMovementInput()
    {
        if (!myIsGrounded)
            return;

        if (Time.time - myStartTimeOfReleasingHealingKeyDown < 0.1f)
            return;

        Vector2 leftStickAxis = myPlayerControls.Movement;

        myVelocity = (leftStickAxis.x * myCameraXZTransform.myRight + leftStickAxis.y * myCameraXZTransform.myForwards).normalized;
        myVelocity *= myBaseSpeed * GetComponent<Stats>().mySpeedMultiplier;

        bool isMoving = IsMoving();
        if (isMoving)
            RotatePlayer();

        if (myIsHealTargetingEnabled)
        {
            DetectFriendlyTargetInput(leftStickAxis != Vector2.zero);
            myVelocity = Vector2.zero;
            return;
        }

        myAnimator.SetBool("IsRunning", isMoving);

        if (myPlayerControls.Jump.WasPressed)
        {
            myVelocity.y = myJumpSpeed;
            myAnimator.ResetTrigger("Land");
            myAnimator.SetBool("IsGrounded", false);
            myAnimator.SetTrigger("Jump");
        }
    }

    private void DetectInput()
    {
        DetectMovementInput();
        DetectSpellInput();

        if (myPlayerControls.ToggleInfo.WasPressed)
            myClass.ToggleSpellInfo();

        if (myPlayerControls.ToggleUIText.WasPressed)
            myCharacterHUD.ToggleUIText(gameObject.GetInstanceID());

        if (myPlayerControls.Restart.WasPressed)
            FindObjectOfType<GameManager>().RestartLevel();
    }

    private void DetectFriendlyTargetInput(bool hasJoystickMoved)
    {
        if (!hasJoystickMoved)
            return;

        int indexOfFriendWithinClosestLookingDirection = 0;
        float closestDotAngle = -1f;

        List<GameObject> players = myTargetHandler.GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            if (index == (PlayerIndex - 1))
                continue;

            Vector3 toFriend = (players[index].transform.position - transform.position).normalized;
            float dotAngle = Vector3.Dot(transform.forward, toFriend);
            if (dotAngle > closestDotAngle)
            {
                closestDotAngle = dotAngle;
                indexOfFriendWithinClosestLookingDirection = index;
            }
        }

        GameObject bestTarget = myTargetHandler.GetPlayer(indexOfFriendWithinClosestLookingDirection);
        if (Target != bestTarget)
            SetTarget(bestTarget);
    }

    private void DetectSpellInput()
    {
        if (myPlayerControls.Action1.WasPressed)
            CheckSpellToCast(0);
        else if (myPlayerControls.Action2.WasPressed)
            CheckSpellToCast(1);
        else if (myPlayerControls.Action3.WasPressed)
            CheckSpellToCast(2);
        else if (myPlayerControls.Action4.WasPressed)
            CheckSpellToCast(3);

        if (!myIsFriendlySpellKeyHeldDown)
            return;

        if (myPlayerControls.Action1.WasReleased)
            CastFriendlySpell(0);
        else if (myPlayerControls.Action2.WasReleased)
            CastFriendlySpell(1);
        else if (myPlayerControls.Action3.WasReleased)
            CastFriendlySpell(2);
        else if (myPlayerControls.Action4.WasReleased)
            CastFriendlySpell(3);
    }

    private void DetectTargetingInput()
    {
        if (myPlayerControls.TargetEnemy.WasPressed && !myIsFriendlySpellKeyHeldDown)
            SetTarget(GameObject.Find("GameManager").GetComponent<TargetHandler>().GetEnemy(PlayerIndex));
    }

    private void DetectStartHealTargeting()
    {
        bool wasHealTargetingEnabled = myIsHealTargetingEnabled;
        myIsHealTargetingEnabled = IsHealTargetingOngoing();

        if (!wasHealTargetingEnabled && myIsHealTargetingEnabled)
        {
            SetTarget(myTargetHandler.GetPlayer(PlayerIndex - 1));
            myAnimator.SetBool("IsRunning", false);
            EnableManualHealTargeting();
        }
    }

    private void DetectLanding()
    {
        if (myIsGrounded)
            return;

        if (myVelocity.y > 0.0f)
            return;

        Ray ray = new Ray(transform.position, Vector3.down);
        float distance = Mathf.Abs(myVelocity.y) * 0.15f;
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        //If needed for future - predict actual landing position with curves and speed.

        //Debug.DrawLine(transform.position, transform.position + Vector3.down * distance);

        if (Physics.Raycast(ray, distance, layerMask))
        {
            myAnimator.SetTrigger("Land");
        }
    }

    private void RotatePlayer()
    {
        transform.rotation = Quaternion.LookRotation(myVelocity, Vector3.up);
    }
    protected override bool IsMoving()
    {
        if (myVelocity.x != 0 || myVelocity.z != 0)
            return true;

        if (!myIsGrounded)
            return true;

        return false;
    }

    private bool IsStunned()
    {
        myStunDuration -= Time.deltaTime;
        if (myStunDuration > 0.0f)
            return true;

        myStunDuration = 0.0f;

        return false;
    }

    private void AutoAttack()
    {
        if (myAutoAttackCooldown > 0.0f)
        {
            myAutoAttackCooldown -= Time.deltaTime * GetComponent<Stats>().myAttackSpeed;
            return;
        }

        GameObject spell = myClass.GetAutoAttack();
        Spell spellScript = spell.GetComponent<Spell>();

        if (!IsAbleToAutoAttack())
        {
            return;
        }

        myAnimator.SetTrigger(GetAnimationHash(SpellAnimationType.AutoAttack));
        myAutoAttackCooldown = 1.2f;

        SpawnSpell(-1, GetSpellSpawnPosition(spellScript));
    }

    public void CheckSpellToCast(int aKeyIndex)
    {
        if (!myClass.IsSpellCastOnFriends(aKeyIndex))
        {
            CastSpell(aKeyIndex, true);
            return;
        }

        myClass.SpellHeldDown(aKeyIndex);
        myStartTimeOfHoldingKeyDown = Time.time;
        myIsFriendlySpellKeyHeldDown = true;
        myFriendlySpellKeyHeldDownIndex = aKeyIndex;
    }

    public void CastFriendlySpell(int aKeyIndex)
    {
        if (Time.time - myStartTimeOfHoldingKeyDown < mySmartTargetHoldDownMaxDuration)
        {
            SetTargetWithSmartTargeting(aKeyIndex);
            CastSpell(aKeyIndex, true);
        }
        else
        {
            CastSpell(aKeyIndex, false);
            myStartTimeOfReleasingHealingKeyDown = Time.time;
        }

        DisableManualHealTargeting();
    }

    public void SetTargetWithSmartTargeting(int aKeyIndex)
    {
        float bestScore = 0.0f;
        int bestPlayerTarget = PlayerIndex - 1;
        List<GameObject> players = myTargetHandler.GetAllPlayers();
        List<GameObject> enemies = myTargetHandler.GetAllEnemies();

        Spell aSpell = myClass.GetSpell(aKeyIndex).GetComponent<Spell>();

        for (int index = 0; index < players.Count; index++)
        {
            float score = 0.0f;
            GameObject playerGO = players[index];

            if (index == PlayerIndex - 1 && !aSpell.myCanCastOnSelf)
            {
                //If there has been no valid target yet, and the current target is the player whom can't cast on self -> put best target to player one or two.
                if (players.Count > 1 && bestPlayerTarget == PlayerIndex - 1 && index == 0)
                    bestPlayerTarget = 1;
                else if (players.Count > 1 && bestPlayerTarget == PlayerIndex - 1 && index > 0)
                    bestPlayerTarget = 0;

                continue;
            }

            Player player = playerGO.GetComponent<Player>();
            if (aSpell.myBuff != null)
            {
                if (player.CheckAlreadyHasThatBuff(aSpell.myBuff.InitializeBuff(gameObject))) //SO BAD, redo buff system from networking legacy
                    continue;
                else
                    score += 1.0f;
            }

            float distance = Vector3.Distance(transform.position, playerGO.transform.position);
            if (distance > aSpell.myRange || !CanRaycastToObject(playerGO))
                continue;

            float healthPercentage = playerGO.GetComponent<Health>().GetHealthPercentage();
            if (playerGO.GetComponent<Class>().myClassRole == Class.ClassRole.Tank)
            {
                score += 0.2f;
                healthPercentage -= 0.15f;
            }

            score += (1.0f - healthPercentage) * 10.0f;

            score += playerGO.GetComponent<Player>().CalculateBuffSmartDamage();

            foreach (GameObject enemyGO in enemies)
            {
                Enemy enemy = enemyGO.GetComponent<Enemy>();
                if (enemy.Target == playerGO)
                    score += 3f;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestPlayerTarget = index;
            }
        }

        GameObject bestTarget = myTargetHandler.GetPlayer(bestPlayerTarget);
        if (Target != bestTarget)
            SetTarget(bestTarget);
    }

    protected float CalculateBuffSmartDamage()
    {
        int damageBuffCount = 0;
        float score = 0.0f;
        foreach (BuffSpell buff in myBuffs)
        {
            if (buff.GetBuff().mySpellType != SpellType.DOT)
                continue;

            BuffTickSpell dot = buff as BuffTickSpell;
            score += dot.CalculateRemainingDamage() * 0.05f;
            const float madeUpMaxTime = 5.0f;
            score += Mathf.Abs(madeUpMaxTime - dot.TimeUntilNextTick()) + dot.GetTickValue() * 0.1f;

            damageBuffCount++;
        }

        return score + damageBuffCount;
    }

    public void CastSpell(int aKeyIndex, bool isPressed)
    {
        if (isPressed)
            myClass.SpellPressed(aKeyIndex);
        else
            myClass.SpellReleased(aKeyIndex);

        if (myClass.IsSpellOnCooldown(aKeyIndex))
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.Cooldown);
            return;
        }

        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (GetComponent<Health>().IsDead())
            return;

        if (!IsAbleToCastSpell(spellScript))
            return;

        myAnimator.SetTrigger(GetAnimationHash(spellScript.GetAnimationType()));

        if (spellScript.myCastTime <= 0.0f)
        {
            SpawnSpell(aKeyIndex, GetSpellSpawnPosition(spellScript));
            myClass.SetSpellOnCooldown(aKeyIndex);
            GetComponent<Resource>().LoseResource(spellScript.myResourceCost);
            //myAnimator.SetTrigger("CastingDone");
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

    private bool IsAbleToAutoAttack()
    {
        if (myIsCasting)
        {
            return false;
        }

        if (!Target)
        {
            return false;
        }

        if (Target.GetComponent<Health>().IsDead())
        {
            myShouldAutoAttack = false;
            return false;
        }

        if (!CanRaycastToObject(Target))
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance > GetComponent<Stats>().myAutoAttackRange)
        {
            return false;
        }

        return true;
    }

    protected override bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (myIsCasting)
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.AlreadyCasting);
            return false;
        }

        if (GetComponent<Resource>().myCurrentResource < aSpellScript.myResourceCost)
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.OutOfResources);
            return false;
        }

        if (!aSpellScript.IsCastableWhileMoving() && IsMoving())
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.CantMoveWhileCasting);
            return false;
        }

        if (aSpellScript.myIsOnlySelfCast)
            return true;

        if (!Target)
        {
            if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) != 0)
            {
                Target = gameObject;
            }
            else
            {
                mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.NoTarget);
                return false;
            }
        }

        bool isDead = Target.GetComponent<Health>().IsDead();
        if (!isDead && aSpellScript.mySpellType == SpellType.Ressurect)
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.NotDead);
            return false;
        }
        if (isDead && aSpellScript.mySpellType != SpellType.Ressurect)
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.IsDead);
            return false;
        }

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        if (distance > aSpellScript.myRange)
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.OutOfRange);
            return false;
        }

        if (!CanRaycastToObject(Target))
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.NoVision);
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Enemy) == 0 && Target.tag == "Enemy")
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.WrongTargetEnemy);
            return false;
        }

        if ((aSpellScript.GetSpellTarget() & SpellTarget.Friend) == 0 && Target.tag == "Player")
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.WrongTargetPlayer);
            return false;
        }

        if (!aSpellScript.myCanCastOnSelf && Target == transform.gameObject)
        {
            mySpellErrorHandler.HighLightError(SpellErrorHandler.SpellError.NotSelfCast);
            return false;
        }

        return true;
    }

    protected override void SetupHud(Transform aUIParent)
    {
        base.SetupHud(aUIParent);
        GetComponentInChildren<TargetProjector>().SetPlayerColor(myCharacterColor);
    }

    public override void Stun(float aDuration)
    {
        base.Stun(aDuration);

        myVelocity.x = 0.0f;
        myVelocity.z = 0.0f;
    }

    public void GiveImpulse(Vector3 aVelocity, bool aShouldLookAtDirection)
    {
        myStunDuration = 0.2f;
        myVelocity = aVelocity;

        if (aShouldLookAtDirection && Target)
            transform.LookAt(Target.transform);
    }

    public void SetPosition(Vector3 aPosition)
    {
        transform.position = aPosition;
    }

    public override void SetTarget(GameObject aTarget)
    {
        if (Target)
            Target.GetComponentInChildren<TargetProjector>().DropTargetProjection(PlayerIndex);

        base.SetTarget(aTarget);

        if (Target)
            Target.GetComponentInChildren<TargetProjector>().AddTargetProjection(myCharacterColor, PlayerIndex);

        myShouldAutoAttack = false;
        if (Target && Target.tag == "Enemy")
            myShouldAutoAttack = true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        myVelocity.x = 0.0f;
        myVelocity.z = 0.0f;
        myShouldAutoAttack = false;
        PostMaster.Instance.PostMessage(new Message(MessageCategory.PlayerDied, gameObject.GetInstanceID()));
    }

    public void OnRevive()
    {
        myAnimator.SetTrigger(GetAnimationHash(SpellAnimationType.AutoAttack));
    }

    public int GetControllerIndex()
    {
        return PlayerIndex;
    }

    public void SetPlayerControls(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }

    private bool IsHealTargetingOngoing()
    {
        return myIsFriendlySpellKeyHeldDown && Time.time - myStartTimeOfHoldingKeyDown > mySmartTargetHoldDownMaxDuration;
    }

    private void EnableManualHealTargeting()
    {
        myClass.HightlightHealTargeting(myFriendlySpellKeyHeldDownIndex, true);
        GetComponentInChildren<HealTargetArrow>().EnableHealTarget(myCharacterColor);
    }

    private void DisableManualHealTargeting()
    {
        myClass.HightlightHealTargeting(myFriendlySpellKeyHeldDownIndex, false);
        GetComponentInChildren<HealTargetArrow>().DisableHealTarget();
        myIsFriendlySpellKeyHeldDown = false;
        myIsHealTargetingEnabled = false;
    }
}