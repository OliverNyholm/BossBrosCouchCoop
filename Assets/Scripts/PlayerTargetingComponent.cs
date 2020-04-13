using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetingComponent : TargetingComponent
{
    private PlayerControls myPlayerControls;

    private AnimatorWrapper myAnimatorWrapper;
    private Player myPlayer;
    private Health myHealth;
    private Stats myStats;

    [Header("The duration of holding down a spell button before enabling targeting system")]
    [SerializeField]
    private float mySmartTargetHoldDownMaxDuration = 0.35f;
    private bool myIsHealTargetingEnabled;

    void Awake()
    {
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
        myPlayer = GetComponent<Player>();
        myHealth = GetComponent<Health>();
        myStats = GetComponent<Stats>();
    }

    public float GetSmartTargetHoldDownMaxDuration()
    {
        return mySmartTargetHoldDownMaxDuration;
    }

    private void Update()
    {
        DetectTargetingInput();
        if (myHealth.IsDead())
            return;

        if (myStats.IsStunned())
            return;

        if(myIsHealTargetingEnabled)
            DetectFriendlyTargetInput(myPlayerControls.Movement != Vector2.zero);
    }

    public bool IsHealTargeting()
    {
        return myIsHealTargetingEnabled;
    }

    public void SetPlayerController(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }

    public override void SetTarget(GameObject aTarget)
    {
        if (Target)
            Target.GetComponentInChildren<TargetProjector>().DropTargetProjection(myPlayer.PlayerIndex);

        base.SetTarget(aTarget);

        if (Target)
            Target.GetComponentInChildren<TargetProjector>().AddTargetProjection(GetComponent<UIComponent>().myCharacterColor, myPlayer.PlayerIndex);

        GetComponent<PlayerCastingComponent>().SetShouldAutoAttack(Target && Target.tag == "Enemy");
    }

    private void DetectTargetingInput()
    {
        if (myPlayerControls.TargetEnemy.WasPressed && !myIsHealTargetingEnabled)
            SetTarget(GameObject.Find("GameManager").GetComponent<TargetHandler>().GetEnemy(myPlayer.PlayerIndex));
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
            if (index == (myPlayer.PlayerIndex - 1))
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

    public void EnableManualHealTargeting(int aSpellIndex)
    {
        myIsHealTargetingEnabled = true;
        SetTarget(myTargetHandler.GetPlayer(myPlayer.PlayerIndex - 1));
        myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, false);

        GetComponent<PlayerUIComponent>().HightlightHealTargeting(aSpellIndex, true);
        GetComponentInChildren<HealTargetArrow>().EnableHealTarget(GetComponent<UIComponent>().myCharacterColor);
    }

    public void DisableManualHealTargeting(int aSpellIndex)
    {
        myIsHealTargetingEnabled = false;
        GetComponent<PlayerUIComponent>().HightlightHealTargeting(aSpellIndex, false);
        GetComponentInChildren<HealTargetArrow>().DisableHealTarget();
    }

    public void SetTargetWithSmartTargeting(int aKeyIndex)
    {
        float bestScore = 0.0f;
        int selfIndex = myPlayer.PlayerIndex - 1;
        int bestPlayerTarget = selfIndex;
        List<GameObject> players = myTargetHandler.GetAllPlayers();
        List<GameObject> enemies = myTargetHandler.GetAllEnemies();

        Spell aSpell = GetComponent<Class>().GetSpell(aKeyIndex).GetComponent<Spell>();

        for (int index = 0; index < players.Count; index++)
        {
            float score = 0.0f;
            GameObject playerGO = players[index];

            if (index == selfIndex && !aSpell.myCanCastOnSelf)
            {
                //If there has been no valid target yet, and the current target is the player whom can't cast on self -> put best target to player one or two.
                if (players.Count > 1 && bestPlayerTarget == selfIndex && index == 0)
                    bestPlayerTarget = 1;
                else if (players.Count > 1 && bestPlayerTarget == selfIndex && index > 0)
                    bestPlayerTarget = 0;

                continue;
            }

            Player player = playerGO.GetComponent<Player>();
            if (aSpell.mySpawnedOnHit != null)
            {
                if (myStats.HasSpellOverTime(aSpell.GetComponent<SpellOverTime>())) //SO BAD, redo buff system from networking legacy
                    score -= 2.0f;
                else
                    score += 1.0f;
            }

            float distance = Vector3.Distance(transform.position, playerGO.transform.position);
            if (distance > aSpell.myRange || !GetComponent<Character>().CanRaycastToObject(playerGO))
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
                TargetingComponent npcTargetingComponent = enemyGO.GetComponent<TargetingComponent>();
                if (npcTargetingComponent && npcTargetingComponent.Target == playerGO)
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
}
