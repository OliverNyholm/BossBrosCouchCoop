using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public string myResourceName;

    public Color myResourceColor;

    public int myMaxResource = 100;
    public int myCurrentResource = 100;

    [SerializeField]
    private int myRegenerationPerTick;

    [SerializeField]
    private float myRegenerationTimer;
    private float myCurrentRegenerationTimer = 0.0f;

    public delegate void ResourceChanged(float aResourcePercentage, string aResourceText);

    public event ResourceChanged EventOnResourceChange;

    private void Update()
    {
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
        myCurrentResource -= aValue;
        if (myCurrentResource <= 0)
        {
            myCurrentResource = 0;
        }

        OnResourceChanged();
    }

    public void GainResource(int aValue)
    {
        myCurrentResource += aValue;
        if (myCurrentResource > myMaxResource)
        {
            myCurrentResource = myMaxResource;
        }

        OnResourceChanged();
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
            OnResourceChanged();
        }
    }
    private void OnResourceChanged()
    {
        EventOnResourceChange?.Invoke(GetResourcePercentage(), myCurrentResource.ToString() + "/" + MaxResource);
    }
}
