using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewOrbOfLight : PreviewSpellChannel
{
    [SerializeField]
    private LayerMask myBlockMovmentLayerMask = 0;

    [SerializeField]
    private Transform myOrbHighlightTransform = null;

    [SerializeField]
    private ParticleSystem myHighlightPathParticleSystem = null;
    ParticleSystem.Particle[] myParticles;

    private OrbOfLight myOrbOfLight;
    private float myOriginalParticleLifetime = 0.0f;

    private void Awake()
    {
        myOrbOfLight = mySpellToSpawn.GetComponent<OrbOfLight>();
        myOriginalParticleLifetime = myHighlightPathParticleSystem.main.startLifetime.constant;

        myParticles = new ParticleSystem.Particle[myHighlightPathParticleSystem.main.maxParticles];
    }

    protected override void Update()
    {
        base.Update();

        float offsetFromGround = myOrbOfLight.GetOffsetFromGround(); ;
        Ray ray = new Ray(transform.position + Vector3.up * offsetFromGround, myParent.transform.forward);

        const float offsetFromWall = 0.4f;
        float desiredDistance = Mathf.Max(0.01f, myOrbOfLight.GetTravelDistance());
        float distanceToMove = desiredDistance;
        if (Physics.Raycast(ray, out RaycastHit hitInfo, distanceToMove, myBlockMovmentLayerMask))
            distanceToMove = hitInfo.distance - offsetFromWall;

        myOrbHighlightTransform.position = ray.origin + ray.direction * distanceToMove;

        float particleLifeTime = distanceToMove / desiredDistance * myOriginalParticleLifetime;
        ParticleSystem.MainModule mainModule = myHighlightPathParticleSystem.main;
        mainModule.startLifetime = particleLifeTime;

        int numParticlesAlive = myHighlightPathParticleSystem.GetParticles(myParticles);
        for (int index = 0; index < numParticlesAlive; index++)
            myParticles[index].startLifetime = particleLifeTime;

        myHighlightPathParticleSystem.SetParticles(myParticles, numParticlesAlive);
    }
}
