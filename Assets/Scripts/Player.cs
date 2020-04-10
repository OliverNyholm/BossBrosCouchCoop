using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Player : Character
{
    private PlayerControls myPlayerControls;
    private TargetHandler myTargetHandler;

    private SpellErrorHandler mySpellErrorHandler;

    private bool myShouldAutoAttack;

    public int PlayerIndex { get; set; }

    protected override void Start()
    {
        base.Start();

        myTargetHandler = GameObject.Find("GameManager").GetComponent<TargetHandler>();
    }

    protected override void Update()
    {
        if (PauseMenu.ourIsGamePaused)
            return;

        base.Update();




        DetectInput();

        if (myShouldAutoAttack)
            AutoAttack();
    }

    protected override bool IsMoving()
    {
        return false;
    }

    private void DetectInput()
    {

        if (myPlayerControls.ToggleInfo.WasPressed)
            GetComponent<UIComponent>().ToggleSpellInfo();

        if (myPlayerControls.ToggleUIText.WasPressed)
            GetComponent<UIComponent>().ToggleUIText(gameObject.GetInstanceID());

        if (myPlayerControls.Restart.WasPressed)
            FindObjectOfType<GameManager>().RestartLevel();
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

    public float CalculateBuffSmartDamage()
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

    public void SetPosition(Vector3 aPosition)
    {
        transform.position = aPosition;
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        myShouldAutoAttack = false;
        PostMaster.Instance.PostMessage(new Message(MessageCategory.PlayerDied, gameObject.GetInstanceID()));
    }

    public void OnRevive()
    {
        GetComponent<AnimatorWrapper>().SetTrigger(SpellAnimationType.AutoAttack);
    }

    public int GetControllerIndex()
    {
        return PlayerIndex;
    }

    public void SetPlayerControls(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
        GetComponent<PlayerMovementComponent>().SetPlayerController(aPlayerControls);
    }
}