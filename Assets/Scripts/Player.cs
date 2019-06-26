using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Player : Character
{
    private CharacterController myController;

    private TargetHandler myTargetHandler;

    private UIManager myUIManager;

    private PlayerControls myPlayerControls;

    private Vector3 myDirection;
    private CameraXZTransform myCameraXZTransform;

    private bool myIsGrounded;
    private bool myShouldAutoAttack;

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

        myDirection = Vector3.zero;

        myTargetHandler = GameObject.Find("GameManager").GetComponent<TargetHandler>();
        myUIManager = GameObject.Find("GameManager").GetComponent<UIManager>();

        myController = GetComponent<CharacterController>();

        Transform uiHud = GameObject.Find("PlayerUI" + PlayerIndex).transform;
        SetupHud(uiHud);
        myClass.SetupSpellHud(CastSpell, uiHud);
    }

    protected override void Update()
    {
        base.Update();

        myDirection.y -= myGravity * Time.deltaTime;
        myController.Move(myDirection * Time.deltaTime);

        //myController.isGrounded unstable further ahead is seems...
        myIsGrounded = IsGrounded();
        if (!myAnimator.GetBool("IsGrounded") && myIsGrounded)
            myAnimator.SetBool("IsGrounded", true);

        DetectTargetingInput();
        if (GetComponent<Health>().IsDead())
            return;

        if (IsStunned())
            return;

        DetectMovementInput();
        DetectSpellInput();

        if (myShouldAutoAttack)
            AutoAttack();
    }

    bool IsGrounded()
    {
        if (myDirection.y > 0.0f)
            return false;

        Ray ray = new Ray(transform.position, Vector3.down);
        float distance = 0.2f;
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        //Debug.DrawLine(transform.position, transform.position + Vector3.down * distance);

        if (Physics.Raycast(ray, distance, layerMask))
            return true;


        return false;
    }

    private void DetectMovementInput()
    {
        if (!myIsGrounded)
            return;

        Vector2 leftStickAxis = myPlayerControls.Movement;

        myDirection = (leftStickAxis.x * myCameraXZTransform.myRight + leftStickAxis.y * myCameraXZTransform.myForwards).normalized;
        myDirection *= myBaseSpeed * GetComponent<Stats>().mySpeedMultiplier;

        bool isMoving = IsMoving();
        if (isMoving)
            RotatePlayer();

        myAnimator.SetBool("IsRunning", isMoving);

        if (myPlayerControls.Jump.WasPressed)
        {
            myDirection.y = myJumpSpeed;
            myAnimator.SetBool("IsGrounded", false);
            myAnimator.SetTrigger("Jump");
        }
    }
    private void DetectSpellInput()
    {
        if (myPlayerControls.Shift.WasPressed)
            myClass.ShiftInteracted(true);
        if (myPlayerControls.Shift.WasReleased)
            myClass.ShiftInteracted(false);

        if (myPlayerControls.ToggleSpellInfo.WasPressed)
            myClass.ToggleSpellInfo();

        bool isShiftDown = myPlayerControls.Shift.RawValue > 0.0f;

        if (isShiftDown && myPlayerControls.Action1.WasPressed)
            CastSpell(4);
        else if (isShiftDown && myPlayerControls.Action2.WasPressed)
            CastSpell(5);
        else if (isShiftDown && myPlayerControls.Action3.WasPressed)
            CastSpell(6);
        else if (isShiftDown && myPlayerControls.Action4.WasPressed)
            CastSpell(7);
        else if (myPlayerControls.Action1.WasPressed)
            CastSpell(0);
        else if (myPlayerControls.Action2.WasPressed)
            CastSpell(1);
        else if (myPlayerControls.Action3.WasPressed)
            CastSpell(2);
        else if (myPlayerControls.Action4.WasPressed)
            CastSpell(3);
    }

    private void DetectTargetingInput()
    {
        int targetIndex = -1;
        if (myPlayerControls.PlayerOne.RawValue > 0.5f)
            targetIndex = 0;
        if (myPlayerControls.PlayerTwo.RawValue > 0.5f)
            targetIndex = 1;
        if (myPlayerControls.PlayerThree.RawValue > 0.5f)
            targetIndex = 2;
        if (myPlayerControls.PlayerFour.RawValue > 0.5f)
            targetIndex = 3;

        if (myPlayerControls.TargetSelf.WasPressed)
            targetIndex = PlayerIndex - 1;

        if (targetIndex != -1)
        {
            SetTarget(GameObject.Find("GameManager").GetComponent<TargetHandler>().GetPlayer(targetIndex));
            return;
        }

        if (myPlayerControls.TargetEnemy.WasPressed)
            SetTarget(GameObject.Find("GameManager").GetComponent<TargetHandler>().GetEnemy(PlayerIndex));
    }

    private void RotatePlayer()
    {
        transform.rotation = Quaternion.LookRotation(myDirection, Vector3.up);
    }
    protected override bool IsMoving()
    {
        if (myDirection.x != 0 || myDirection.z != 0)
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

        myAnimator.SetTrigger("Attack");
        myAutoAttackCooldown = 1.2f;

        SpawnSpell(-1, GetSpellSpawnPosition(spellScript));
    }

    public void CastSpell(int aKeyIndex)
    {
        myClass.SpellPressed(aKeyIndex);

        if (myClass.IsSpellOnCooldown(aKeyIndex))
        {
            myUIManager.CreateErrorMessage("Can't cast that spell yet");
            return;
        }

        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (GetComponent<Health>().IsDead() && spellScript.mySpellType != SpellType.Ressurect)
            return;

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

    private bool IsAbleToAutoAttack()
    {
        if (myIsCasting)
        {
            return false;
        }

        if (!myTarget)
        {
            return false;
        }

        if (myTarget.GetComponent<Health>().IsDead())
        {
            myShouldAutoAttack = false;
            return false;
        }

        if (!CanRaycastToTarget())
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, myTarget.transform.position);
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

        if (!myTarget.GetComponent<Health>().IsDead() && aSpellScript.mySpellType == SpellType.Ressurect)
        {
            myUIManager.CreateErrorMessage("That target is not dead!");
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

    protected override void SetupHud(Transform aUIParent)
    {
        base.SetupHud(aUIParent);
        GetComponentInChildren<TargetProjector>().SetPlayerColor(myCharacterColor);
    }

    public override void Stun(float aDuration)
    {
        base.Stun(aDuration);

        myDirection.x = 0.0f;
        myDirection.z = 0.0f;
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

    protected override void SetTarget(GameObject aTarget)
    {
        if(myTarget)
            myTarget.GetComponentInChildren<TargetProjector>().DropTargetProjection(PlayerIndex);

        base.SetTarget(aTarget);

        if (myTarget)
            myTarget.GetComponentInChildren<TargetProjector>().AddTargetProjection(myCharacterColor, PlayerIndex);

        myShouldAutoAttack = false;
        if (myTarget && myTarget.tag == "Enemy")
            myShouldAutoAttack = true;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        myDirection.x = 0.0f;
        myDirection.z = 0.0f;
        myShouldAutoAttack = false;
        PostMaster.Instance.PostMessage(new Message(MessageType.PlayerDied, gameObject.GetInstanceID()));
    }

    public int GetControllerIndex()
    {
        return PlayerIndex;
    }

    public void SetPlayerControls(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }
}