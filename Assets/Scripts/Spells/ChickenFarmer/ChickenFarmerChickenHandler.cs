using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenFarmerChickenHandler : MonoBehaviour
{
    [SerializeField]
    private const int myMaxChickenCount = 8;
    private List<GameObject> myChickens = new List<GameObject>(myMaxChickenCount);

    [SerializeField]
    private Vector3[] myFlightFormation = new Vector3[myMaxChickenCount];

    [SerializeField]
    private float myFlightDurationPerChicken = 1.0f;

    private void Awake()
    {
        myFlightFormation[0] = new Vector3(1.0f, 2.5f, 0.0f);
        myFlightFormation[1] = new Vector3(-1.0f, 2.5f, 0.0f);
        myFlightFormation[2] = new Vector3(0.0f, 2.5f, -1.0f);
        myFlightFormation[3] = new Vector3(0.0f, 2.5f, 1.0f);
        myFlightFormation[4] = new Vector3(1.5f, 2.5f, 1.5f);
        myFlightFormation[5] = new Vector3(-1.5f, 2.5f, 1.5f);
        myFlightFormation[6] = new Vector3(1.5f, 2.5f, -1.5f);
        myFlightFormation[7] = new Vector3(1.5f, 2.5f, 1.5f);
    }

    public void EnableFlightMode()
    {
        float flightDuration = myChickens.Count * myFlightDurationPerChicken;
        for (int index = 0; index < myChickens.Count; index++)
            myChickens[index].GetComponent<ChickenMovementComponent>().SetFlightMode(myFlightFormation[index], flightDuration);
    }

    public void AddChickenAndSetPosition(GameObject aChicken)
    {
        Vector3 spawnPosition = myFlightFormation[myChickens.Count];
        spawnPosition.y = 0.0f;

        aChicken.transform.position = transform.position + transform.rotation * spawnPosition;

        aChicken.GetComponent<Chicken>().SetParent(gameObject);
        aChicken.GetComponent<ChickenMovementComponent>().SetParentAndOffset(gameObject, spawnPosition);

        myChickens.Add(aChicken);
    }
    
    public bool HasMaxAmountOfChickenActive()
    {
        return myChickens.Count == myMaxChickenCount;
    }

    public int GetMaxChickenCount()
    {
        return myMaxChickenCount;
    }
}
