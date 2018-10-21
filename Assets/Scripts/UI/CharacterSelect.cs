using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]
    private InputField myNameInputField;
    [SerializeField]
    private GameObject[] myCharacterPrefabs;

    private PlayerConnection myPlayerConnection;

    public void SetPlayerCharacter(PlayerConnection aPlayerConnection)
    {
        myPlayerConnection = aPlayerConnection;
    }

    public void SetPrefabCharacter(int anIndex)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;

        string name = myNameInputField.text == string.Empty ? "Player" : myNameInputField.text;
        myPlayerConnection.SpawnCharacterPrefab(anIndex, name);
    }

    public GameObject GetCharacterPrefab(int anIndex)
    {
        return myCharacterPrefabs[anIndex];
    }
}
