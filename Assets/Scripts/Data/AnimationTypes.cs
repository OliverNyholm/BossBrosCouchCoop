using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enum name has to match the same in the animator system
/// </summary>
public enum SpellAnimationType
{
    DefaultCast,
    DefaultChannel,
    OverheadCast,
    OverheadChannel,
    EasyMelee,
    HeavyMelee,
    ShieldBlock,
    Meditate,
    AutoAttack,
    Count
}

public enum AnimationVariable
{
    IsRunning,
    Jump,
    Land,
    Death,
    IsGrounded,
    IsCasting,
    CastingDone,
    CastingCancelled,
    AutoAttackSpeed,
    RunSpeed,
    Count
}
