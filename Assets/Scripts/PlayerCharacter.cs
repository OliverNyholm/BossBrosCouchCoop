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

    [SyncVar]
    private string myName;

    private CharacterController myController;
    private Camera myCamera;

    public Class myClass;

    private Castbar myCastbar;
    private bool myIsCasting;
    private Coroutine myCastingRoutine;

    [SyncVar]//(hook ="OnTargetChange")
    private GameObject myTarget;

    CharacterHUD myCharacterHUD;
    CharacterHUD myTargetHUD;

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

        myCharacterHUD.SetName(myName + " (" + myClass.myClassName + ")");
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority)
            return;

        if (!myIsTypingInChat)
        {
            DetectMovementInput();
            DetectPressedSpellOrRaycast();
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
        if(myClass.IsSpellOnCooldown(aKeyIndex))
        {
            Debug.Log("Can't cast that spell yet");
            return;
        }

        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (!IsAbleToCastSpell(spellScript))
            return;


        if (spellScript.mySpeed <= 0.0f)
        {
            CmdSpawnSpell(aKeyIndex, myTarget.transform.position);
            myClass.SetSpellOnCooldown(aKeyIndex);
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

            if (IsMoving())
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
            CmdSpawnSpell(aKeyIndex, transform.position);
            myClass.SetSpellOnCooldown(aKeyIndex);
        }
    }

    public void InterruptSpellCast()
    {
        if (myIsCasting)
        {
            StopCasting();
        }
    }

    public void InterruptTarget()
    {
        if (isServer)
            RpcInterrupt();
        else if (hasAuthority)
            CmdInterrupt();
    }

    [Command]
    private void CmdInterrupt()
    {
        myTarget.GetComponent<PlayerCharacter>().InterruptSpellCast();
    }

    [ClientRpc]
    private void RpcInterrupt()
    {
        myTarget.GetComponent<PlayerCharacter>().InterruptSpellCast();
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

        if (Physics.Raycast(transform.position + Vector3.up, ((myTarget.transform.position + Vector3.up * 0.5f) - (transform.position + Vector3.up)), out hit))
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
            Debug.Log("Already casting or no target!");
            return false;
        }

        if (IsMoving())
        {
            Debug.Log("Can't cast while moving!");
            return false;
        }

        if (!CanRaycastToTarget())
        {
            Debug.Log("Target not in line of sight!");
            return false;
        }

        float distance = Vector3.Distance(transform.position, myTarget.transform.position);
        if (distance > aSpellScript.myRange)
        {
            Debug.Log("Out of range!");
            return false;
        }

        if (aSpellScript.IsFriendly() && myTarget.tag == "Enemy")
        {
            Debug.Log("Can't cast friendly spells on enemies");
            return false;
        }

        if (!aSpellScript.IsFriendly() && myTarget.tag == "Player")
        {
            Debug.Log("Can't cast hostile spells on friends.");
            return false;
        }

        return true;
    }

    private void RaycastNewTarget()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask layerMask = LayerMask.GetMask("Targetable") | LayerMask.GetMask("UI");

        if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                Debug.Log("Hit UI");
                return;
            }

            if (myTarget != null)
            {
                myTarget.GetComponentInChildren<Projector>().enabled = false;
                myTarget.GetComponent<Health>().EventOnHealthChange -= ChangeHudHealth;
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
                myTarget.GetComponent<Health>().EventOnHealthChange -= ChangeHudHealth;
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
        RpcUpdateTargetForAllClients(aTarget);

        if (myTarget)
            Debug.Log("Target is: " + myTarget.name);
        else
            Debug.Log("Target is null now!");
    }

    [ClientRpc]
    private void RpcUpdateTargetForAllClients(GameObject aTarget)
    {
        Debug.Log("Target is updated for all clients!");
        myTarget = aTarget;
    }

    private void SetTargetHUD()
    {
        myTarget.GetComponent<Health>().EventOnHealthChange += ChangeHudHealth;

        myTargetHUD.Show();
        ChangeHudHealth(myTarget.GetComponent<Health>().GetHealthPercentage(),
            myTarget.GetComponent<Health>().myCurrentHealth.ToString() + "/" + myTarget.GetComponent<Health>().MaxHealth);

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

    private void ChangeHudHealth(float aHealthPercentage, string aHealthText)
    {
        myTargetHUD.SetHealthBarFillAmount(aHealthPercentage);
        myTargetHUD.SetHealthText(aHealthText);
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
