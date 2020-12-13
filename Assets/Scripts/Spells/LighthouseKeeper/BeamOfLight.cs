using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamOfLight : ChannelSpell
{
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

    private void Awake()
    {
        myPlayers = FindObjectOfType<TargetHandler>().GetAllPlayers();
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
        }
    }

    private void StartCoroutine()
    {
        myParent.GetComponent<CastingComponent>().StartChannel(myChannelTime, this, gameObject, 0.0f);
    }

    private void HealNearby()
    {
        if (myImpactLocation == Vector3.zero)
            return;

        for (int index = 0; index < myPlayers.Count; index++)
        {
            Vector3 playerPositionWithOffset = myPlayers[index].transform.position + myOriginOffset;
            if ((playerPositionWithOffset - myImpactLocation).sqrMagnitude <= myRange * myRange)
            {
                myPlayers[index].GetComponent<Health>().GainHealth(myHealValue);
            }
        }
    }
    
    private void FindImpactPoint()
    {
        float distance = 100.0f;
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            myImpactLocation = hitInfo.point;

            float distanceToObject = (myImpactLocation - transform.position).magnitude;
            myBeam.transform.localScale = new Vector3(myBeam.transform.localScale.x, myBeam.transform.localScale.y, distanceToObject);

            myHitParticle.transform.position = myImpactLocation + myParent.transform.rotation * myHitParticleOffset;
            if(!myHitParticle.isPlaying)
                myHitParticle.Play();
        }
        else
        {
            myImpactLocation = Vector3.zero;
            myHitParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

    }

    public override bool IsCastableWhileMoving()
    {
        return myIsCastableWhileMoving;
    }
}
