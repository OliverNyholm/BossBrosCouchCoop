﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public GameObject[] myCharacterPrefabs;

    private PlayerConnection myPlayerConnection;

    public void SetPlayerCharacter(PlayerConnection aPlayerConnection)
    {
        myPlayerConnection = aPlayerConnection;
    }

    public void SetPrefabCharacter(int anIndex)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        myPlayerConnection.SetCharacterPrefab(myCharacterPrefabs[anIndex]);
    }
}
