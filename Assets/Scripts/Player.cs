using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    private Color myColor;

    private CharacterController myController;

    private TargetHandler myTargetHandler;

    private UIManager myUIManager;

    private List<BuffSpell> myBuffs;

    private Vector3 myDirection;
    private CameraXZTransform myCameraXZTransform;
    private DpadInput myDpadInput;

    private bool myIsGrounded;
    private bool myShouldAutoAttack;

    protected override void Start()
    {
        base.Start();

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

        Transform uiHud = GameObject.Find("PlayerUI" + myControllerIndex).transform;
        SetupHud(uiHud);
        myClass.SetupSpellHud(CastSpell, uiHud);
    }

    protected override void Update()
    {
        base.Update();

        myDpadInput.Update();

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
        bool isTriggerDown = (Input.GetAxisRaw("LeftTrigger" + myControllerIndex) > 0.0f) || (Input.GetAxisRaw("RightTrigger" + myControllerIndex) > 0.0f);

        if (isTriggerDown && Input.GetButtonDown("A" + myControllerIndex))
            CastSpell(4);
        else if (isTriggerDown && Input.GetButtonDown("B" + myControllerIndex))
            CastSpell(5);
        else if (isTriggerDown && Input.GetButtonDown("X" + myControllerIndex))
            CastSpell(6);
        else if (isTriggerDown && Input.GetButtonDown("Y" + myControllerIndex))
            CastSpell(7);
        else if (Input.GetButtonDown("A" + myControllerIndex))
            CastSpell(0);
        else if (Input.GetButtonDown("B" + myControllerIndex))
            CastSpell(1);
        else if (Input.GetButtonDown("X" + myControllerIndex))
            CastSpell(2);
        else if (Input.GetButtonDown("Y" + myControllerIndex))
            CastSpell(3);
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

    protected override void SetTarget(GameObject aTarget)
    {
        base.SetTarget(aTarget);

        if (myTarget)
            myTarget.GetComponentInChildren<TargetProjector>().AddTargetProjection(myColor, myControllerIndex);

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
        AIPostMaster.Instance.PostAIMessage(new AIMessage(AIMessageType.PlayerDied, gameObject.GetInstanceID()));
    }

    public int GetControllerIndex()
    {
        return myControllerIndex;
    }
}