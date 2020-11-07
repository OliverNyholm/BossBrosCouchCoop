using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItemSpell : Spell
{
    [Header("Lifetime of movement until removed")]
    [SerializeField]
    private float myLifeTime = 20f;
    private float myLifeTimeReset;

    private Vector3 myDirection;

    private bool myHasDealtDamage = false;

    private Rigidbody myRigidBody;

    private void Awake()
    {
        myLifeTimeReset = myLifeTime;
        myRigidBody = GetComponent<Rigidbody>();
    }

    public override void Reset()
    {
        base.Reset();
        myLifeTime = myLifeTimeReset;
        myHasDealtDamage = false;
        myRigidBody.useGravity = false;
    }

    public override void Restart()
    {
        base.Restart();
        myDirection = myParent.transform.forward;
    }

    protected override void Update()
    {
        myLifeTime -= Time.deltaTime;
        if (myLifeTime <= 0.0f)
        {
            ReturnToPool();
        }

        if (myHasDealtDamage)
            return;

        if (myShouldRotate)
            transform.Rotate(myRandomRotation * myRotationSpeed * Time.deltaTime);

        transform.position += myDirection * mySpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision aCollision)
    {
        if (myHasDealtDamage)
            return;

        if (aCollision.gameObject == myParent)
            return;

        Health health = aCollision.gameObject.GetComponentInParent<Health>();
        if (health && !health.IsDead() && myDamage > 0.0f)
            health.TakeDamage(myDamage, myParent.GetComponent<UIComponent>().myCharacterColor, aCollision.gameObject.transform.position);

        SpawnVFX(2.5f, aCollision.gameObject.gameObject);
        myRigidBody.useGravity = true;

        myHasDealtDamage = true;
<<<<<<< HEAD

        Debug.Log("Food hit: " + aCollision.gameObject.name);
=======
>>>>>>> master
    }
}
