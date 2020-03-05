using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PooledParticleEffect : PoolableObject
{
    public override void Reset()
    {
        GetComponent<ParticleSystem>().Play();

        ParticleSystem[] children = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem child in children)
            child.Play();
    }
}
