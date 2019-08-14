using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class GetTargetBasedOn : Action
{
    public enum BasedOperator
    {
        Lowest,
        Highest
    }

    public enum BasedEnum
    {
        Distance,
        Health
    }

    public SharedGameObjectList myListOfGameObjects = null;
    public SharedGameObject myReturnObject = null;

    public BasedOperator myOperator;
    public BasedEnum myBasedOn;

    public override TaskStatus OnUpdate()
    {
        if (myListOfGameObjects.Value.Count == 0)
        {
            Debug.Log("No Gamobjects in list for GetRandomObjects.");
            return TaskStatus.Failure;
        }

        switch (myBasedOn)
        {
            case BasedEnum.Distance:
                myReturnObject.Value = FindDistance();
                break;
            case BasedEnum.Health:
                myReturnObject.Value = FindHealth();
                break;
            default:
                Debug.Log("BasedOnEnum not chosen to a workable enum.");
                myReturnObject.Value = null;
                break;
        }

        return TaskStatus.Success;
    }

    private GameObject FindDistance()
    {
        GameObject target = null;
        float bestDistanceSqr = myOperator == BasedOperator.Highest ? float.MinValue : float.MaxValue;

        for (int index = 0; index < myListOfGameObjects.Value.Count; index++)
        {
            Health health = myListOfGameObjects.Value[index].GetComponent<Health>();
            if (health.IsDead())
                continue;

            float sqrDistance = (myListOfGameObjects.Value[index].transform.position - transform.position).sqrMagnitude;
            if (IsBetterTarget(sqrDistance, bestDistanceSqr))
            {
                target = myListOfGameObjects.Value[index];
                bestDistanceSqr = sqrDistance;
            }
        }

        return target;
    }

    private GameObject FindHealth()
    {
        GameObject target = null;
        int bestHealth = myOperator == BasedOperator.Highest ? int.MinValue : int.MaxValue;

        for (int index = 0; index < myListOfGameObjects.Value.Count; index++)
        {
            Health health = myListOfGameObjects.Value[index].GetComponent<Health>();
            if (health.IsDead())
                continue;

            int currentHealth = health.myCurrentHealth;
            if (IsBetterTarget(currentHealth, bestHealth))
            {
                target = myListOfGameObjects.Value[index];
                bestHealth = currentHealth;
            }
        }

        return target;
    }

    private bool IsBetterTarget(float aValue, float aComparison)
    {
        if (myOperator == BasedOperator.Highest)
            return aValue > aComparison;
        else
            return aValue < aComparison;
    }

    private bool IsBetterTarget(int aValue, int aComparison)
    {
        if (myOperator == BasedOperator.Highest)
            return aValue > aComparison;
        else
            return aValue < aComparison;
    }
}
