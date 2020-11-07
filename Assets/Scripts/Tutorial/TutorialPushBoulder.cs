using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPushBoulder : TutorialCompletion
{
    [Header("The spell to try out")]
    [SerializeField]
    private GameObject myPushSpell = null;

    [SerializeField]
    private GameObject myBoulder = null;
    [SerializeField]
    private GameObject myBoulderMesh = null;
    [SerializeField]
    private ParticleSystem mySpawnVFX = null;

    public float myLaunchAngle = 30.0f;
    public float myBoulderGravity = 500.0f;
    public Transform myTargetHitWallTransform = null;

    protected override void Awake()
    {
        base.Awake();

        Spell spell = myPushSpell.GetComponent<Spell>();
        spell.CreatePooledObjects(PoolManager.Instance, 6);
    }

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        myTargetHandler.AddEnemy(myBoulder, true);
        myBoulder.GetComponent<Health>().EventOnHealthZero += OnTargetDied;
        myBoulder.GetComponentInChildren<TutorialBoulder>().EnableBoulder();

        foreach (GameObject player in Players)
            player.GetComponent<Class>().ReplaceSpell(myPushSpell, 0);


        return true;
    }

    private void OnTargetDied()
    {
        myTargetHandler.RemoveEnemy(myBoulder);
        myBoulder.GetComponentInChildren<TutorialBoulder>().DisableBoulder();
        StartCoroutine(BoulderFlyingCoroutine());
    }

    private Vector3 CalculateLaunchVelocity(out float outDuration)
    {
        // Selected angle in radians
        float angle = myLaunchAngle * Mathf.Deg2Rad;

        // Positions of this object and the target on the same plane
        Vector3 planarTarget = new Vector3(myTargetHitWallTransform.position.x, 0, myTargetHitWallTransform.position.z);
        Vector3 planarPostion = new Vector3(myBoulder.transform.position.x, 0, myBoulder.transform.position.z);

        // Planar distance between objects
        float distance = Vector3.Distance(planarTarget, planarPostion);
        // Distance along the y axis between objects
        float yOffset = myBoulder.transform.position.y - myTargetHitWallTransform.position.y;

        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * myBoulderGravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        outDuration = distance / velocity.z;

        // Rotate our velocity to match the direction between the two objects
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        //Have to inverse in x, since angleBetweenObjects seems to ignore that fact //Oliver
        if (myTargetHitWallTransform.transform.position.x < myBoulder.transform.position.x)
            finalVelocity.x *= -1;


        // Fire!
        return finalVelocity;
    }

    IEnumerator BoulderFlyingCoroutine()
    {
        float duration;
        Vector3 launchVelocity = CalculateLaunchVelocity(out duration);
        Vector3 rotation = new Vector3(50.0f, 90.0f, -70.0f);

        float elapsedTime = 0.0f;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            Vector3 currentVelocity = launchVelocity;
            currentVelocity.y -= myBoulderGravity * elapsedTime;
            myBoulder.transform.position += currentVelocity * Time.deltaTime;

            myBoulderMesh.transform.Rotate(rotation * Time.deltaTime);

            yield return null;
        }

        mySpawnVFX.Play();
    }
}
