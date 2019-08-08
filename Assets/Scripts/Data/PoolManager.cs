using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Header("GameObject for HealthPool")]
    [SerializeField]
    private ObjectPool myHealthPool;

    public static PoolManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetFloatingHealth()
    {
        return myHealthPool.GetPooled();
    }
}
