using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Resource : NetworkBehaviour
{
    [SyncVar]
    public string myResourceName;

    [SyncVar]
    public int myMaxResource = 100;
    [SyncVar]
    public int myCurrentResource = 100;

    [SerializeField]
    private int myRegenerationPerTick;

    [SerializeField]
    private float myRegenerationTimer;
    private float myCurrentRegenerationTimer = 0.0f;

    public delegate void ResourceChanged(float aResourcePercentage, string aResourceText);

    [SyncEvent]
    public event ResourceChanged EventOnResourceChange;

    private void Update()
    {
        if (!hasAuthority)
            return;

        if (myCurrentResource >= myMaxResource)
            return;

        if (myCurrentRegenerationTimer < myRegenerationTimer)
        {
            myCurrentRegenerationTimer += Time.deltaTime;

            if (myCurrentRegenerationTimer >= myRegenerationTimer)
            {
                myCurrentRegenerationTimer = 0.0f;
                GainResource(myRegenerationPerTick);
            }
        }
    }

    public void LoseResource(int aValue)
    {
        if (!isServer)
        {
            return;
        }

        myCurrentResource -= aValue;
        if (myCurrentResource <= 0)
        {
            myCurrentResource = 0;
        }

        CmdResourceChanged();
    }

    public void GainResource(int aValue)
    {
        if (!isServer)
        {
            return;
        }

        myCurrentResource += aValue;
        if (myCurrentResource > myMaxResource)
        {
            myCurrentResource = myMaxResource;
        }

        CmdResourceChanged();
    }

    public bool HasEnoughResource(int aValue)
    {
        return myCurrentResource >= aValue;
    }

    public float GetResourcePercentage()
    {
        return (float)myCurrentResource / myMaxResource;
    }

    public int MaxResource
    {
        get { return myMaxResource; }
        set
        {
            myMaxResource = value;
            CmdResourceChanged();
        }
    }

    [Command]
    private void CmdResourceChanged()
    {
        EventOnResourceChange?.Invoke(GetResourcePercentage(), myCurrentResource.ToString() + "/" + MaxResource);
    }
}
