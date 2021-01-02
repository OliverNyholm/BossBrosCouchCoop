using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class FallingObject : Spell
{
    [Header("The Model to rotate")]
    [SerializeField]
    private Transform myMeshTransform = null;

    [SerializeField]
    private float myMinRotationSpeed = 0;
    [SerializeField]
    private float myMaxRotationSpeed = 0;

    [SerializeField]
    private Transform myDecalTransform = null;
    private Vector3 myLandPosition;

    [SerializeField]
    private bool myShouldNotifyBossOnHitGround = false;
    [SerializeField]
    private string myNotifyBossEvent = "SpellHitGround";

    protected override void OnFirstUpdate()
    {
        base.OnFirstUpdate();

        const float maxDistance = 100.0f;
        if (UtilityFunctions.FindGroundFromLocation(transform.position, out Vector3 hitLocation, out _, maxDistance))
            myLandPosition = hitLocation;

        if (myShouldRotate)
        {
            myRotationSpeed = Random.Range(myMinRotationSpeed, myMaxRotationSpeed);
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

        bool hitPlayers = charactersInRange.Count > 0;
        if (hitPlayers)
        {
            int damage = myDamage / charactersInRange.Count;
            foreach (GameObject character in charactersInRange)
                DealDamage(damage, transform.position, character);
        }

        if (myShouldNotifyBossOnHitGround)
        {
            BehaviorTree behaviorTree = myParent.GetComponent<BehaviorTree>();
            if (behaviorTree)
                behaviorTree.SendEvent<object, object>(myNotifyBossEvent, hitPlayers ? 1 : 0, (float)myDamage);
        }
    }
}
