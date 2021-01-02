using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{  
    private List<Transform> myObjectsToMove;
    private CharacterWrapper[] myCharacters;

    private TargetHandler myTargetHandler;

    private Vector3 myPreviousPosition;
    private Quaternion myPreviosRotation;

    private struct CharacterWrapper
    {
        public GameObject myCharacter;
        public MovementComponent myMovementComponent;
        public bool myIsOnPlatform;
    }

    private void Awake()
    {
        myTargetHandler = FindObjectOfType<TargetHandler>();
        myObjectsToMove = new List<Transform>(8);
        myCharacters = new CharacterWrapper[8];
    }

    private void Start()
    {
        List<GameObject> players = myTargetHandler.GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            myCharacters[index].myCharacter = players[index];
            myCharacters[index].myMovementComponent = players[index].GetComponent<PlayerMovementComponent>();
        }

        List<GameObject> enemies = myTargetHandler.GetAllEnemies();
        for (int index = 0; index < enemies.Count; index++)
        {
            myCharacters[players.Count + index].myCharacter = enemies[index];
            myCharacters[players.Count + index].myMovementComponent = enemies[index].GetComponent<MovementComponent>();
        }

        myPreviousPosition = transform.position;
        myPreviosRotation = transform.rotation;
    }

    private void Update()
    {
        Vector3 movementDifference = transform.position - myPreviousPosition;
        Quaternion rotationDifference = transform.rotation * Quaternion.Inverse(myPreviosRotation);

        for (int index = 0; index < myCharacters.Length; index++)
        {
            CharacterWrapper character = myCharacters[index];
            if (!character.myCharacter)
                continue;

            if (!character.myIsOnPlatform || (character.myMovementComponent && character.myMovementComponent.IsMoving()))
                continue;

            MoveObject(character.myCharacter.transform, movementDifference, rotationDifference);
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
        if(aGameObject.GetComponent<Character>())
        {
            for (int index = 0; index < myCharacters.Length; index++)
            {
                if (myCharacters[index].myCharacter == null)
                    myCharacters[index].myCharacter = aGameObject;

                if (myCharacters[index].myCharacter == aGameObject)
                {
                    myCharacters[index].myIsOnPlatform = true;
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
        if (aGameObject.GetComponent<Character>())
        {
            for (int index = 0; index < myCharacters.Length; index++)
            {
                if (myCharacters[index].myCharacter == aGameObject)
                {
                    myCharacters[index].myIsOnPlatform = false;
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
