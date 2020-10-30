using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{  
    private List<Transform> myObjectsToMove;
    private PlayerWrapper[] myPlayers;

    private TargetHandler myTargetHandler;

    private Vector3 myPreviousPosition;
    private Quaternion myPreviosRotation;

    private struct PlayerWrapper
    {
        public GameObject myPlayer;
        public MovementComponent myMovementComponent;
        public bool myIsOnPlatform;
    }

    private void Awake()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();
        myObjectsToMove = new List<Transform>(8);
        myPlayers = new PlayerWrapper[4];
    }

    private void Start()
    {
        List<GameObject> players = myTargetHandler.GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            myPlayers[index].myPlayer = players[index];
            myPlayers[index].myMovementComponent = players[index].GetComponent<PlayerMovementComponent>();
        }

        myPreviousPosition = transform.position;
        myPreviosRotation = transform.rotation;
    }

    private void Update()
    {
        Vector3 movementDifference = transform.position - myPreviousPosition;
        Quaternion rotationDifference = transform.rotation * Quaternion.Inverse(myPreviosRotation);

        for (int index = 0; index < myPlayers.Length; index++)
        {
            PlayerWrapper player = myPlayers[index];
            if (!player.myIsOnPlatform || player.myMovementComponent.IsMoving())
                continue;

            MoveObject(player.myPlayer.transform, movementDifference, rotationDifference);
        }

        for (int index = 0; index < myObjectsToMove.Count; index++)
        {
            MoveObject(myObjectsToMove[index], movementDifference, rotationDifference);
        }

        myPreviousPosition = transform.position;
        myPreviosRotation = transform.rotation;
    }

    private void MoveObject(Transform aTransform, Vector3 aMovementDifference, Quaternion aRotationDifference)
    {
        aTransform.position += aMovementDifference;
        Vector3 toTransform = aTransform.position - transform.position;
        float lengthToPlayer = toTransform.magnitude;
        toTransform /= lengthToPlayer;

        aTransform.position = transform.position + (aRotationDifference * toTransform) * lengthToPlayer;
        aTransform.rotation = aRotationDifference * aTransform.rotation;
    }

    public void AddToPlatform(GameObject aGameObject)
    {
        if(aGameObject.GetComponent<Player>())
        {
            for (int index = 0; index < myPlayers.Length; index++)
            {
                if (myPlayers[index].myPlayer == aGameObject)
                {
                    myPlayers[index].myIsOnPlatform = true;
                    break;
                }
            }
        }
        else
        {
            myObjectsToMove.Add(aGameObject.transform);
        }
    }

    public void RemoveFromPlatform(GameObject aGameObject)
    {
        if (aGameObject.GetComponent<Player>())
        {
            for (int index = 0; index < myPlayers.Length; index++)
            {
                if (myPlayers[index].myPlayer == aGameObject)
                {
                    myPlayers[index].myIsOnPlatform = false;
                    break;
                }
            }
        }
        else
        {
            myObjectsToMove.Remove(aGameObject.transform);
        }
    }
}
