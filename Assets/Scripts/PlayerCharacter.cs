using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class PlayerCharacter : NetworkBehaviour
{

    public float mySpeed;
    public float myJumpSpeed;
    public float myGravity;

    public Vector3 myDirection;

    public bool myShouldStrafe = true;

    public bool myIsTypingInChat = false;

    public float myStunDuration;

    [SyncVar]
    private string myName;

    private CharacterController myController;
    private Camera myCamera;

    public Class myClass;
    private Health myHealth;
    private Resource myResource;

    private Castbar myCastbar;
    private bool myIsCasting;
    private Coroutine myCastingRoutine;

    [SyncVar]
    private GameObject myTarget;

    private CharacterHUD myCharacterHUD;
    private CharacterHUD myTargetHUD;

    private UIManager myUIManager;

    // Use this for initialization
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        myCamera = Camera.main;
        myCamera.GetComponent<PlayerCamera>().SetTarget(this.transform);
    }

    private void Start()
    {
        myController = transform.GetComponent<CharacterController>();

        myClass = GetComponentInChildren<Class>();
        myClass.SetupSpellHud(CastSpell);

        myCastbar = GameObject.Find("Castbar Background").GetComponent<Castbar>();
        myCharacterHUD = GameObject.Find("PlayerHud").GetComponent<CharacterHUD>();
        myTargetHUD = GameObject.Find("TargetHud").GetComponent<CharacterHUD>();

        myUIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        myHealth = GetComponent<Health>();
        myHealth.EventOnHealthChange += ChangeMyHudHealth;
        ChangeMyHudHealth(myHealth.GetHealthPercentage(), myHealth.myCurrentHealth.ToString() + "/" + myHealth.myMaxHealth.ToString());

        myResource = GetComponent<Resource>();
        myResource.EventOnResourceChange += ChangeMyHudResource;
        ChangeMyHudResource(myResource.GetResourcePercentage(), myResource.myCurrentResource.ToString() + "/" + myResource.MaxResource.ToString());

        myCharacterHUD.SetName(myName + " (" + myClass.myClassName + ")");
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        if (!myIsTypingInChat && myStunDuration <= 0.0f)
        {
            DetectMovementInput();
            DetectPressedSpellOrRaycast();
        }
        else if (myStunDuration > 0.0f)
        {
            myStunDuration -= Time.deltaTime;
        }

        RotatePlayer();

        myDirection.y -= myGravity * Time.deltaTime;

        myController.Move(myDirection * Time.deltaTime);
    }

    private void DetectMovementInput()
    {
        if (!myController.isGrounded)
            return;

        if (myShouldStrafe)
        {
            myDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical")).normalized * mySpeed;
        }
        else
        {
            myDirection = new Vector3(0.0f, 0.0f, Input.GetAxisRaw("Vertical")) * mySpeed;
        }

        myDirection = transform.TransformDirection(myDirection);

        if (Input.GetButtonDown("Jump"))
            myDirection.y = myJumpSpeed;
    }

    private void DetectPressedSpellOrRaycast()
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

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                RaycastNewTarget();
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

    public void CastSpell(int aKeyIndex)
    {
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
            return;
        }

        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(0.0f);
        myCastbar.SetSpellName(spellScript.myName);
        myCastbar.SetCastbarColor(spellScript.myCastbarColor);
        myCastbar.SetSpellIcon(spellScript.myCastbarIcon);
        myCastbar.SetCastTimeText(spellScript.myCastTime.ToString());

        myCastingRoutine = StartCoroutine(CastbarProgress(aKeyIndex));
    }

    [Command]
    private void CmdSpawnSpell(int aKeyIndex, Vector3 aSpawnPosition)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);

        GameObject instance = Instantiate(spell, aSpawnPosition + new Vector3(0.0f, 0.5f, 0.0f), transform.rotation);

        Spell spellScript = instance.GetComponent<Spell>();
        spellScript.SetParent(transform.gameObject);
        spellScript.SetTarget(myTarget);

        NetworkServer.Spawn(instance);
    }

    private IEnumerator CastbarProgress(int aKeyIndex)
    {
        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();


        myIsCasting = true;
        float rate = 1.0f / spellScript.myCastTime;
        float progress = 0.0f;

        while (progress <= 1.0f)
        {
            myCastbar.SetCastbarFillAmount(Mathf.Lerp(0, 1, progress));
            myCastbar.SetCastTimeText((spellScript.myCastTime - (progress * spellScript.myCastTime)).ToString("0.0"));

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
        }
    }

    private Vector3 GetSpellSpawnPosition(Spell aSpellScript)
    {
        if (aSpellScript.mySpeed <= 0.0f)
            return myTarget.transform.position;

        return transform.position;
    }

    public void GiveImpulse(Vector3 aVelocity, bool aShouldLookAtDirection)
    {
        myStunDuration = 0.2f;
        myDirection = aVelocity;

        if (aShouldLookAtDirection)
            transform.LookAt(myTarget.transform);
    }

    public void InterruptSpellCast()
    {
        if (myIsCasting)
        {
            StopCasting();
        }
    }

    private void StopCasting()
    {
        StopCoroutine(myCastingRoutine);
        myIsCasting = false;
        myCastbar.FadeOutCastbar();
    }

    private bool CanRaycastToTarget()
    {
        RaycastHit hit;

        Vector3 hardcodedEyePosition = new Vector3(0.0f, 0.7f, 0.0f);
        Vector3 infrontOfPlayer = (transform.position + hardcodedEyePosition) + transform.forward;
        Vector3 direction = (myTarget.transform.position + hardcodedEyePosition) - infrontOfPlayer;

        if (Physics.Raycast(infrontOfPlayer, direction, out hit))
        {
            if (hit.transform == myTarget.transform)
            {
                return true;
            }
            else if (hit.transform.parent == myTarget.transform)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAbleToCastSpell(Spell aSpellScript)
    {
        if (!myTarget || myIsCasting)
        {
            myUIManager.CreateErrorMessage("Already casting or no target!");
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

        if (aSpellScript.IsFriendly() && myTarget.tag == "Enemy")
        {
            myUIManager.CreateErrorMessage("Can't cast friendly spells on enemies");
            return false;
        }

        if (!aSpellScript.IsFriendly() && myTarget.tag == "Player")
        {
            myUIManager.CreateErrorMessage("Can't cast hostile spells on friends.");
            return false;
        }

        return true;
    }

    private void RaycastNewTarget()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask layerMask = LayerMask.GetMask("Targetable");

        if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
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
        }
    }

    private bool IsMoving()
    {
        if (myDirection.x != 0 && myDirection.y != 0)
            return true;

        if (!myController.isGrounded)
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
            myTarget.GetComponent<Health>().myCurrentHealth.ToString() + "/" + myTarget.GetComponent<Health>().MaxHealth);

        if (myTarget.GetComponent<Resource>() != null)
        {
            myTarget.GetComponent<Resource>().EventOnResourceChange += ChangeTargetHudResource;
            ChangeTargetHudResource(myTarget.GetComponent<Resource>().GetResourcePercentage(),
                myTarget.GetComponent<Resource>().myCurrentResource.ToString() + "/" + myTarget.GetComponent<Resource>().MaxResource);
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
            if (myTarget.GetComponent<PlayerCharacter>() != null)
                myTargetHUD.SetName(myTarget.GetComponent<PlayerCharacter>().Name);
            myTargetHUD.SetNameColor(new Color(120f / 255f, 1.0f, 0.0f));
        }
    }

    private void ChangeTargetHudHealth(float aHealthPercentage, string aHealthText)
    {
        myTargetHUD.SetHealthBarFillAmount(aHealthPercentage);
        myTargetHUD.SetHealthText(aHealthText);
    }

    private void ChangeTargetHudResource(float aResourcePercentage, string aResourceText)
    {
        myTargetHUD.SetResourceBarFillAmount(aResourcePercentage);
        myTargetHUD.SetResourceText(aResourceText);
    }

    private void ChangeMyHudHealth(float aHealthPercentage, string aHealthText)
    {
        myCharacterHUD.SetHealthBarFillAmount(aHealthPercentage);
        myCharacterHUD.SetHealthText(aHealthText);
    }

    private void ChangeMyHudResource(float aResourcePercentage, string aResourceText)
    {
        myCharacterHUD.SetResourceBarFillAmount(aResourcePercentage);
        myCharacterHUD.SetResourceText(aResourceText);
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
        set { myName = value; }
    }
}
