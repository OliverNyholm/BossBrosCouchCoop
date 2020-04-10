using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetingComponent : MonoBehaviour
{
    public GameObject Target { get; protected set; }

    protected TargetHandler myTargetHandler;

    private void Start()
    {
        myTargetHandler = GameObject.Find("GameManager").GetComponent<TargetHandler>();
    }

    public virtual void SetTarget(GameObject aTarget)
    {
        Target = aTarget;
        GetComponent<UIComponent>().SetTargetHUD(aTarget);
    }
}
