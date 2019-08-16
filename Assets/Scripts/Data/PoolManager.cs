﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("The Pool prefab that others can use to pool objects")]
    [SerializeField]
    private GameObject myPoolPrefab = null;
    
    [Header("GameObject for HealthPool")]
    [SerializeField]
    private ObjectPool myHealthPool = null;

    [Header("GameObject for AutoAttack Pool")]
    [SerializeField]
    private ObjectPool myAutoAttackPool = null;

    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetFloatingHealth()
    {
        return myHealthPool.GetPooled();
    }

    public ObjectPool GetAutoAttackPool()
    {
        return myAutoAttackPool;
    }

    public GameObject GetPoolPrefab()
    {
        return myPoolPrefab;
    }
}