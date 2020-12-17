using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidingStarPulse : Spell
{
    [SerializeField]
    private float myPulseLifeTime = 3.0f;
    private float myDuration = 0.0f;

    [SerializeField]
    private float myTargetRadius = 10.0f;
    [SerializeField]
    private float myEdgeRadius = 1.5f;


    [SerializeField]
    private bool myShouldUseHealCurve = false;
    [Tooltip("The curves 0-1 value will be multiplied with this.")]
    [SerializeField]
    private int myHealCurveMultiplier = 1000;
    [SerializeField]
    private AnimationCurve myHealCurve = null;
    [SerializeField]
    private AnimationCurve myRadiusCurve = null;

    private ParticleSystem myParticleSystem;

    private List<GameObject> myPlayers = new List<GameObject>(4);

    private void Awake()
    {
        myParticleSystem = GetComponent<ParticleSystem>();
        myParticleSystem.Stop();
        ParticleSystem.MainModule mainModule = myParticleSystem.main;
        mainModule.duration = myPulseLifeTime - 0.3f;
    }

    public void SetPlayers(List<GameObject> somePlayers)
    {
        myPlayers.Clear();
        foreach (GameObject player in somePlayers)
        {
            if (player == myParent) //Heals self in GuidingStar
                continue;

            Health health = player.GetComponent<Health>();
            if (health && !health.IsDead())
                myPlayers.Add(player);
        }
    }

    public override void Restart()
    {
        base.Restart();

        myDuration = 0.0f;
        ParticleSystem.ShapeModule shapeModule = myParticleSystem.shape;
        shapeModule.radius = 0.0f;

        myParticleSystem.Play();
    }

    protected override void Update()
    {
        myDuration += Time.deltaTime;

        float lifePercentage = myDuration / myPulseLifeTime;

        float radius = myRadiusCurve.Evaluate(lifePercentage) * myTargetRadius;

        ParticleSystem.ShapeModule shapeModule = myParticleSystem.shape;
        shapeModule.radius = radius;

        if (myShouldUseHealCurve)
        {
            float healCurveValue = myHealCurve.Evaluate(lifePercentage);
            myHealValue = (int)(healCurveValue * myHealCurveMultiplier);
        }

        HealNearby(radius);

        if (myDuration >= myPulseLifeTime)
        {
            ReturnToPool();
            myParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void HealNearby(float aRadius)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            Health health = myPlayers[index].GetComponent<Health>();
            if (!health || health.IsDead())
                continue;

            Vector3 playerPositionWithOffset = myPlayers[index].transform.position;
            float distanceToPlayerFromCenterSqr = (playerPositionWithOffset - transform.position).sqrMagnitude;
            float maxDistanceFromPlayerSqr = aRadius * aRadius;
            float minDistanceFromPlayerSqr =(aRadius - myEdgeRadius) * (aRadius - myEdgeRadius);
            if (distanceToPlayerFromCenterSqr <= maxDistanceFromPlayerSqr && distanceToPlayerFromCenterSqr >= minDistanceFromPlayerSqr)
            {
                myTarget = myPlayers[index];
                DealSpellEffect();
                myPlayers.RemoveAt(index);
                index--;
            }
        }
    }
}
