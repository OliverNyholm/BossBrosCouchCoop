using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : Spell
{
    [Header("The Model to rotate")]
    [SerializeField]
    private Transform myMeshTransform = null;

    [SerializeField]
    private Transform myDecalTransform = null;
    private Vector3 myLandPosition;

    bool myHitTargets = false;

    protected override void OnFirstUpdate()
    {
        base.OnFirstUpdate();

        const float maxDistance = 100.0f;
        if (UtilityFunctions.FindGroundFromLocation(transform.position, out Vector3 hitLocation, out MovablePlatform movablePlatform, maxDistance))
            myLandPosition = hitLocation;

        if (myShouldRotate)
        {
            myRotationSpeed = Random.Range(0.3f, 1.5f);
            myRandomRotation = Random.rotation.eulerAngles;
        }

        myDecalTransform.transform.position = myLandPosition;
    }

    protected override void Update()
    {
        if (myIsFirstUpdate)
            OnFirstUpdate();

        if (myShouldRotate)
            myMeshTransform.Rotate(myRandomRotation * myRotationSpeed * Time.deltaTime);

        transform.position += Vector3.down * mySpeed * Time.deltaTime;
        myDecalTransform.transform.position = myLandPosition;
        if (transform.position.y <= myLandPosition.y)
        {
            OnHitGround();
            SpawnVFX(2.5f);
            ReturnToPool();
        }
    }

    private void OnHitGround()
    {
        List<GameObject> targets = UtilityFunctions.HasSpellTarget(mySpellTarget, SpellTargetType.Player) ? myTargetHandler.GetAllPlayers() : myTargetHandler.GetAllEnemies();

        UtilityFunctions.GetAllCharactersInRadius(targets, transform.position, myRange, out List<GameObject> charactersInRange);
        if (charactersInRange.Count == 0)
            return;

        int damage = myDamage / charactersInRange.Count;
        foreach (GameObject character in charactersInRange)
            DealDamage(damage, transform.position, character);

        myHitTargets = true;
    }
}
