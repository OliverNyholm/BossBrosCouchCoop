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

    [SerializeField]
    private bool myUseLookAtHealTargeting = false;

    private List<GameObject> myPreviouslyTargetedEnemies = new List<GameObject>(8);
    private float myLatestSelectedTargetTime;

    public enum ManualHealTargetingMode
    {
        NotActive,
        LookAt,
        Joystick
    };

    private ManualHealTargetingMode myManualHealTargetingMode = ManualHealTargetingMode.NotActive;

    void Awake()
    {
        myAnimatorWrapper = GetComponent<AnimatorWrapper>();
        myPlayer = GetComponent<Player>();
        myHealth = GetComponent<Health>();
        myStats = GetComponent<Stats>();

        myHealth.EventOnHealthZero += OnDeath;
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

        if(myManualHealTargetingMode != ManualHealTargetingMode.NotActive)
            DetectFriendlyTargetInput(myPlayerControls.Movement != Vector2.zero);
    }

    public ManualHealTargetingMode GetHealTargetingMode()
    {
        return myManualHealTargetingMode;
    }

    public bool IsManualHealTargeting()
    {
        return myManualHealTargetingMode != ManualHealTargetingMode.NotActive;
    }

    public void SetPlayerController(PlayerControls aPlayerControls)
    {
        myPlayerControls = aPlayerControls;
    }

    public override void SetTarget(GameObject aTarget)
    {
        PlayerCastingComponent castingComponent = GetComponent<PlayerCastingComponent>();

        if (!castingComponent)
            return;

        if (Target)
        {
            Target.GetComponentInChildren<TargetProjector>().DropTargetProjection(myPlayer.PlayerIndex);
            Target.GetComponent<Health>().EventOnHealthZero -= OnTargetDied;
        }

        base.SetTarget(aTarget);

        if (Target)
        {
            Target.GetComponentInChildren<TargetProjector>().AddTargetProjection(GetComponent<UIComponent>().myCharacterColor, myPlayer.PlayerIndex);
            Target.GetComponent<Health>().EventOnHealthZero += OnTargetDied;
        }

        castingComponent.SetShouldAutoAttack(Target && Target.tag == "Enemy");
    }

    private void DetectTargetingInput()
    {
        if (Time.timeScale <= 0.0f)
            return;

        if (myPlayerControls.TargetEnemy.WasPressed && myManualHealTargetingMode == ManualHealTargetingMode.NotActive)
            DetermineNewEnemyTarget();
    }

    private void DetectFriendlyTargetInput(bool hasJoystickMoved)
    {
        if (myUseLookAtHealTargeting)
            GetFriendlyTargetByLooking(hasJoystickMoved);
        else
            GetFriendlyTargetByStickLocation();
    }

    private void GetFriendlyTargetByStickLocation()
    {
        const float axisRequired = 0.7f;

        int playerIndex = -1;
        Vector2 leftStickAxis = myPlayerControls.Movement;
        if (leftStickAxis.x <= -axisRequired)
            playerIndex = 0;
        else if (leftStickAxis.y >= axisRequired)
            playerIndex = 1;
        else if (leftStickAxis.y <= -axisRequired)
            playerIndex = 2;
        else if (leftStickAxis.x >= axisRequired)
            playerIndex = 3;

        GameObject target = myTargetHandler.GetPlayer(playerIndex);
        if (target && Target != target)
            SetTarget(target);

        if (Target && Target != gameObject)
            transform.LookAt(Target.transform, Vector3.up);
    }

    private void GetFriendlyTargetByLooking(bool hasJoystickMoved)
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

            if (players[index].GetComponent<Health>().IsDead())
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
        myManualHealTargetingMode = myUseLookAtHealTargeting ? ManualHealTargetingMode.LookAt : ManualHealTargetingMode.Joystick;

        if(myManualHealTargetingMode == ManualHealTargetingMode.LookAt)
            SetTarget(myTargetHandler.GetPlayer(myPlayer.PlayerIndex - 1));

        myAnimatorWrapper.SetBool(AnimationVariable.IsRunning, false);

        GetComponent<PlayerUIComponent>().HightlightHealTargeting(aSpellIndex, true);
        GetComponentInChildren<HealTargetArrow>().EnableHealTarget(GetComponent<UIComponent>().myCharacterColor);
    }

    public void DisableManualHealTargeting(int aSpellIndex)
    {
        myManualHealTargetingMode = ManualHealTargetingMode.NotActive;
        GetComponent<PlayerUIComponent>().HightlightHealTargeting(aSpellIndex, false);
        GetComponentInChildren<HealTargetArrow>().DisableHealTarget();
    }

    public void SetTargetWithLowestHealthAndWithoutBuff(int aKeyIndex)
    {
        float lowestHealthPercentage = 1.0f;
        int bestPlayerTarget = -1;
        int playerTargetedByBossIndex = -1;
        List<GameObject> players = myTargetHandler.GetAllPlayers();
        List<GameObject> enemies = myTargetHandler.GetAllEnemies();

        Spell spellToCast = GetComponent<Class>().GetSpell(aKeyIndex).GetComponent<Spell>();

        for (int index = 0; index < players.Count; index++)
        {
            GameObject player = players[index];
            float healthPercentage = player.GetComponent<Health>().GetHealthPercentage();
            if(healthPercentage < lowestHealthPercentage || lowestHealthPercentage >= 1.0f)
            {
                if(spellToCast is SpellOverTime)
                {
                    if(player.GetComponent<Stats>().HasSpellOverTime(spellToCast as SpellOverTime))
                        continue;
                }

                lowestHealthPercentage = healthPercentage;
                bestPlayerTarget = index;
            }

            foreach (GameObject enemyGO in enemies)
            {
                TargetingComponent npcTargetingComponent = enemyGO.GetComponent<TargetingComponent>();
                if (npcTargetingComponent && npcTargetingComponent.Target == player)
                    playerTargetedByBossIndex = index;
            }
        }

        if (bestPlayerTarget == -1)
        {
            if(playerTargetedByBossIndex != -1)
                bestPlayerTarget = playerTargetedByBossIndex;
            else
                bestPlayerTarget = myPlayer.PlayerIndex - 1; //Self
        }

        GameObject bestTarget = myTargetHandler.GetPlayer(bestPlayerTarget);
        if (Target != bestTarget)
            SetTarget(bestTarget);
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

            Health health = playerGO.GetComponent<Health>();
            if (health.IsDead())
                continue;

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

            float healthPercentage = health.GetHealthPercentage();
            if (playerGO.GetComponent<Class>().myClassRole == ClassRole.Tank)
            {
                score += 0.2f;
                healthPercentage -= 0.15f;
            }

            score += (1.0f - healthPercentage) * 10.0f;

            score += playerGO.GetComponent<Stats>().CalculateBuffSmartDamage();

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

    public void DetermineNewEnemyTarget()
    {
        const float resetTargetTimer = 2.0f;
        if (Time.time - myLatestSelectedTargetTime > resetTargetTimer)
            myPreviouslyTargetedEnemies.Clear();

        myLatestSelectedTargetTime = Time.time;

        List<GameObject> enemies = myTargetHandler.GetAllEnemies();
        if (myPreviouslyTargetedEnemies.Count == enemies.Count)
            myPreviouslyTargetedEnemies.Clear();

        int bestIndex = -1;
        float bestScore = float.MinValue;
        for (int index = 0; index < enemies.Count; index++)
        {
            if (enemies[index] == Target)
                continue;

            float deathScore = 0.0f;
            if (enemies[index].GetComponent<Health>().IsDead())
                deathScore = -50000.0f;

            if (myPreviouslyTargetedEnemies.Contains(enemies[index]))
                continue;

            Vector3 toTarget = enemies[index].transform.position - transform.position;
            float distance = toTarget.magnitude;
            toTarget.y = 0.0f;
            toTarget /= distance;

            float dotAngle = Vector3.Dot(transform.forward, toTarget);

            float score = (1 + dotAngle) * (100.0f - distance) + deathScore;
            if(score > bestScore)
            {
                bestScore = score;
                bestIndex = index;
            }
        }

        if (bestIndex != -1)
            SetTarget(enemies[bestIndex]);
    }

    private void OnDeath()
    {
        SetTarget(null);
        myManualHealTargetingMode = ManualHealTargetingMode.NotActive;
    }

    private void OnTargetDied()
    {
        SetTarget(null);
    }
}
