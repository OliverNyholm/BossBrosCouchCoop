using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerCharacter : NetworkBehaviour
{

    public float mySpeed;
    public float myJumpSpeed;
    public float myGravity;

    public Vector3 myDirection;

    public bool myShouldStrafe = true;
    private string myName;

    private CharacterController myController;
    private Camera myCamera;

    public Class myClass;

    private Castbar myCastbar;
    private bool myIsCasting;
    private Coroutine castingRoutine;

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

        DetectMovementInput();
        DetectPressedSpellOrRaycast();
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

    private void CastSpell(int aKeyIndex)
    {
        Debug.Log("Cast spell Index: " + aKeyIndex);

        GameObject spell = myClass.GetSpell(aKeyIndex);
        Spell spellScript = spell.GetComponent<Spell>();

        if (!IsUnableToCastSpell(spellScript))
            return;


        if (spellScript.mySpeed <= 0.0f)
        {
            CmdSpawnSpell(aKeyIndex, myTarget.transform.position);
            return;
        }

        myCastbar.ShowCastbar();
        myCastbar.SetCastbarFillAmount(0.0f);
        myCastbar.SetSpellName(spellScript.myName);
        myCastbar.SetCastbarColor(spellScript.myCastbarColor);
        myCastbar.SetSpellIcon(spellScript.myCastbarIcon);
        myCastbar.SetCastTimeText(spellScript.myCastTime.ToString());

        castingRoutine = StartCoroutine(CastbarProgress(aKeyIndex));
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

            yield return null;
        }

        myIsCasting = false;
        myCastbar.FadeOutCastbar();

        if (IsInSightOrCloseEnough(spellScript.myRange))
            CmdSpawnSpell(aKeyIndex, transform.position);
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
        }

        return false;
    }

    private bool IsInSightOrCloseEnough(float aSpellRange)
    {
        if (!CanRaycastToTarget())
        {
            Debug.Log("Target not in line of sight!");
            return false;
        }

        float distance = Vector3.Distance(transform.position, myTarget.transform.position);
        if (distance > aSpellRange)
        {
            Debug.Log("Out of range!");
            return false;
        }

        return true;
    }

    private bool IsUnableToCastSpell(Spell aSpellScript)
    {
        if (!myTarget || myIsCasting)
        {
            if (!myTarget)
                Debug.Log("no target!");
            if (myIsCasting)
                Debug.Log("Casting!");

            Debug.Log("Already casting or no target!");
            return false;
        }

        if (!IsInSightOrCloseEnough(aSpellScript.myRange))
            return false;

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
        LayerMask layerMask = LayerMask.GetMask("Targetable");

        if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
        {
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
        ChangeHudHealth();

        if (myTarget.tag == "Enemy")
        {
            myTargetHUD.SetName(myTarget.name);
            myTargetHUD.SetNameColor(Color.red);
        }
        else if (myTarget.tag == "Player")
        {
            myTargetHUD.SetName(myTarget.GetComponent<PlayerCharacter>().myName);
            myTargetHUD.SetNameColor(new Color(120f / 255f, 1.0f, 0.0f));
        }
    }

    private void ChangeHudHealth()
    {
        Health targetHealth = myTarget.GetComponent<Health>();
        myTargetHUD.SetHealthBarFillAmount(targetHealth.GetHealthPercentage());
        myTargetHUD.SetHealthText(targetHealth.myCurrentHealth.ToString() + "/" + targetHealth.MaxHealth);
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
