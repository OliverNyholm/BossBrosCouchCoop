using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemBoss : Enemy
{
    protected override void Start()
    {
        myStats = GetComponent<Stats>();
    }

    protected override void OnDestroy()
    {
    }

    protected override void Update()
    {
    }
}
