using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeHeal : Spell
{
    [SerializeField]
    private float myChannelTime;

    private float myIntervalTimer;
    private float myCurrentIntervalTimer;
    private int myHealthPerTick;

    private List<GameObject> myPlayers;

    private GameObject myVFX;

    protected override void Start()
    {
        myPlayers = FindObjectOfType<TargetHandler>().GetAllPlayers();

        const int NrOfTicks = 10;
        myIntervalTimer = myChannelTime / NrOfTicks;
        myCurrentIntervalTimer = 0.0f;

        myHealthPerTick = myDamage / NrOfTicks;

        myChannelTime += 0.02f;

        StartCoroutine();
        myVFX = SpawnVFX(myChannelTime + 1.5f);
        ParticleSystem particleSystem = myVFX.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSystem.main;
        main.duration = myChannelTime;

        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.radius = myRange;

        particleSystem.Play();
    }

    private void OnDestroy()
    {
        myVFX.GetComponent<ParticleSystem>().Stop();
        myVFX.transform.parent = null;
    }

    protected override void Update()
    {
        myCurrentIntervalTimer += Time.deltaTime;
        if (myCurrentIntervalTimer >= myIntervalTimer)
        {
            myCurrentIntervalTimer -= myIntervalTimer;
            HealNearby();
        }

        myChannelTime -= Time.deltaTime;
        if (myChannelTime <= 0.0f)
            Destroy(gameObject);
    }

    private void StartCoroutine()
    {
        myParent.GetComponent<Player>().StartChannel(myChannelTime, this, gameObject);
    }

    private void HealNearby()
    {
        Vector3 spellPositionXZ = transform.position;
        spellPositionXZ.y = 0;
        for (int index = 0; index < myPlayers.Count; index++)
        {
            Vector3 positionXZ = myPlayers[index].transform.position;
            positionXZ.y = 0;
            if ((positionXZ - spellPositionXZ).sqrMagnitude <= myRange * myRange)
            {
                myPlayers[index].GetComponent<Health>().GainHealth(myHealthPerTick);
            }
        }
    }

    protected override string GetSpellDetail()
    {
        string detail = "to heal all players within range for a total of " + myDamage + " over a period of " + myChannelTime + " seconds.";

        return detail;
    }
}
