using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Friendlist : MonoBehaviour
{
    public GameObject myFriendlistHudPrefab;

    private Dictionary<NetworkInstanceId, CharacterHUD> myCharacterHuds = new Dictionary<NetworkInstanceId, CharacterHUD>();

    public void Clear()
    {
        myCharacterHuds.Clear();
    }

    public void AddHud(CharacterHUD aCharacterHud, NetworkInstanceId anID, string aName)
    {
        myCharacterHuds.Add(anID, aCharacterHud);
        Debug.Log(aName + "Added Health! :)" + anID);
    }

    public void ChangePartyHudHealth(float aHealthPercentage, NetworkInstanceId anID, string aName)
    {
        CharacterHUD characterHud;
        if (myCharacterHuds.TryGetValue(anID, out characterHud))
            characterHud.SetHealthBarFillAmount(aHealthPercentage);
    }
}
