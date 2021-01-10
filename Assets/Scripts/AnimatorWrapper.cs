using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorWrapper : MonoBehaviour
{
    private Animator myAnimator;
    private int[] mySpellAnimationHashes;
    private int[] myAnimationVariableHashes;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        FillAnimationHashes();
    }

    public void SetTrigger(SpellAnimationType aSpellAnimationType)
    {
        if (aSpellAnimationType != SpellAnimationType.None)
            myAnimator.SetTrigger(GetAnimationHash(aSpellAnimationType));
    }

    public void SetTrigger(AnimationVariable anAnimationVariable)
    {
        myAnimator.SetTrigger(GetAnimationHash(anAnimationVariable));
    }

    public bool GetBool(AnimationVariable anAnimationVariable)
    {
        return myAnimator.GetBool(GetAnimationHash(anAnimationVariable));
    }

    public void SetBool(AnimationVariable anAnimationVariable, bool aValue)
    {
        myAnimator.SetBool(GetAnimationHash(anAnimationVariable), aValue);
    }

    public float GetFloat(AnimationVariable anAnimationVariable)
    {
        return myAnimator.GetFloat(GetAnimationHash(anAnimationVariable));
    }

    public void SetFloat(AnimationVariable anAnimationVariable, float aValue)
    {
        myAnimator.SetFloat(GetAnimationHash(anAnimationVariable), aValue);
    }

    public void ResetTrigger(SpellAnimationType aSpellAnimationType)
    {
        if (aSpellAnimationType != SpellAnimationType.None)
            myAnimator.ResetTrigger(GetAnimationHash(aSpellAnimationType));
    }

    public void ResetTrigger(AnimationVariable anAnimationVariable)
    {
        myAnimator.ResetTrigger(GetAnimationHash(anAnimationVariable));
    }

    private void FillAnimationHashes()
    {
        mySpellAnimationHashes = new int[(int)SpellAnimationType.Count];
        foreach (SpellAnimationType type in Enum.GetValues(typeof(SpellAnimationType)))
        {
            if (type == SpellAnimationType.Count)
                break;

            mySpellAnimationHashes[(int)type] = Animator.StringToHash(type.ToString());
        }

        myAnimationVariableHashes = new int[(int)AnimationVariable.Count];
        foreach (AnimationVariable type in Enum.GetValues(typeof(AnimationVariable)))
        {
            if (type == AnimationVariable.Count)
                break;

            myAnimationVariableHashes[(int)type] = Animator.StringToHash(type.ToString());
        }
    }

    protected int GetAnimationHash(SpellAnimationType aSpellAnimationType)
    {
        return mySpellAnimationHashes[(int)aSpellAnimationType];
    }

    protected int GetAnimationHash(AnimationVariable anAnimationVariable)
    {
        return myAnimationVariableHashes[(int)anAnimationVariable];
    }
}
