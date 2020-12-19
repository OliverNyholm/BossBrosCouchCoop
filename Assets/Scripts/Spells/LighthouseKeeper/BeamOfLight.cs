using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamOfLight : ChannelSpell
{
    [SerializeField]
    private LayerMask myIgnoreLayer = 0;

    [SerializeField]
    private Vector3 myOriginOffset = Vector3.zero;

    [SerializeField]
    private Vector3 myHitParticleOffset = Vector3.zero;

    [SerializeField]
    private float myIntervalTimer = 1;
    private float myCurrentIntervalTimer;

    private List<GameObject> myPlayers;

    [SerializeField]
    private GameObject myBeam = null;
    [SerializeField]
    private ParticleSystem myHitParticle = null;

    private Vector3 myImpactLocation = new Vector3();
    private bool myBeamHitSomething = false;

    TargetHandler myTargetHandler = null;

    private void Awake()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();
    }

    public override void Reset()
    {
        base.Reset();

        myHitParticle.Stop();
    }

    public override void Restart()
    {
        myCurrentIntervalTimer = 0.0f;
        myChannelTime += 0.02f;

        myPlayers = myTargetHandler.GetAllPlayers();
        myHitParticle.Play();

        StartCoroutine();
    }

    protected override void Update()
    {
        transform.rotation = myParent.transform.rotation;
        transform.position = myParent.transform.position + transform.rotation * myOriginOffset;
        FindImpactPoint();

        myCurrentIntervalTimer += Time.deltaTime;
        if (myCurrentIntervalTimer >= myIntervalTimer)
        {
            myCurrentIntervalTimer -= myIntervalTimer;
            HealNearby();
            DamageNearby();
        }
    }

    private void StartCoroutine()
    {
        myParent.GetComponent<CastingComponent>().StartChannel(myChannelTime, this, gameObject, 0.0f);
    }

    private void HealNearby()
    {
        if (!myBeamHitSomething)
            return;


        foreach (GameObject player in myPlayers)
        {
            if (player)
                continue;

            Health health = player.GetComponent<Health>();
            if (health || health.IsDead())
                continue;

            Vector3 playerPositionWithOffset = player.transform.position + myOriginOffset;
            if ((playerPositionWithOffset - myImpactLocation).sqrMagnitude <= myRange * myRange)
                health.GainHealth(myHealValue);
        }
    }

    private void DamageNearby()
    {
        if (!myBeamHitSomething)
            return;

        foreach (GameObject enemy in myTargetHandler.GetAllEnemies())
        {
            Vector3 playerPositionWithOffset = enemy.transform.position + myOriginOffset;
            if ((playerPositionWithOffset - myImpactLocation).sqrMagnitude <= myRange * myRange)
                DealDamage(myDamage, myImpactLocation, enemy);
        }
    }

    private void FindImpactPoint()
    {
        float distance = 100.0f;
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hitInfo;
        myBeamHitSomething = Physics.Raycast(ray, out hitInfo, distance, ~myIgnoreLayer);

        if (myBeamHitSomething)
            myImpactLocation = hitInfo.point;
        else
            myImpactLocation = ray.origin + ray.direction * distance;

        distance = (myImpactLocation - transform.position).magnitude;

        myBeam.transform.localScale = new Vector3(myBeam.transform.localScale.x, myBeam.transform.localScale.y, distance);
        myHitParticle.transform.position = myImpactLocation + myParent.transform.rotation * myHitParticleOffset;
    }

    public override bool IsCastableWhileMoving()
    {
        return myIsCastableWhileMoving;
    }
}
