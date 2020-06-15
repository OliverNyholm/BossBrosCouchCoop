using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


[RequireComponent(typeof(ParticleSystem))]
public class PooledParticleEffect : PoolableObject
{
    public override void Reset()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem)
            particleSystem.Play();

        VisualEffect visualEffect = GetComponent<VisualEffect>();
        if (visualEffect)
        {
            visualEffect.Reinit();
            visualEffect.Play();
        }

        ParticleSystem[] children = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem child in children)
            child.Play();

        VisualEffect[] visualEffectChildren = GetComponentsInChildren<VisualEffect>();
        foreach (VisualEffect child in visualEffectChildren)
        {
            child.Reinit();
            child.Play();
        }
    }
}
