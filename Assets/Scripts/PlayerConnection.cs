using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerConnection : NetworkBehaviour
{
    public GameObject myCharacterPrefab;

    //private InputField myChatInputField;
    //private PlayerCharacter myCharacter;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        CharacterSelect characterSelect = GameObject.Find("CharacterSelect").GetComponent<CharacterSelect>();
        characterSelect.GetComponent<CanvasGroup>().alpha = 1;
        characterSelect.SetPlayerCharacter(this);

        //myChatInputField = GameObject.Find("Canvas").GetComponentInChildren<InputField>();
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        //if (myChatInputField.isFocused && !myCharacter.myIsTypingInChat)
        //{
        //    myCharacter.myIsTypingInChat = true;
        //}
        //else if (!myChatInputField.isFocused && myCharacter.myIsTypingInChat)
        //{
        //    myCharacter.myIsTypingInChat = false;
        //}
    }

    public void SpawnCharacterPrefab(int anIndex, string aName)
    {
        if (!isLocalPlayer)
            return;

        CmdSpawnCharacter(anIndex, aName);
    }

    [Command]
    private void CmdSpawnCharacter(int aCharacterID, string aName)
    {
        CharacterSelect characterSelect = GameObject.Find("CharacterSelect").GetComponent<CharacterSelect>();
        myCharacterPrefab = characterSelect.GetCharacterPrefab(aCharacterID);
        GameObject go = Instantiate(myCharacterPrefab, this.transform);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        RpcSetChildParent(gameObject, go, aName);
    }

    [ClientRpc]
    private void RpcSetChildParent(GameObject aParent, GameObject aChild, string aName)
    {
        aChild.transform.parent = aParent.transform;
        aChild.GetComponent<PlayerCharacter>().Name = aName;
    }
}
