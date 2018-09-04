using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerConnection : NetworkBehaviour
{

    public GameObject myCharacterPrefab;
    public string myName;

    //private InputField myChatInputField;
    //private PlayerCharacter myCharacter;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        myName = "Player" + Random.Range(0, 1000);

        CharacterSelect characterSelect = GameObject.Find("CharacterSelect").GetComponent<CharacterSelect>();
        characterSelect.GetComponent<CanvasGroup>().alpha = 1;
        characterSelect.SetPlayerCharacter(this);
        //CmdSpawnCharacter();

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

    public void SetCharacterPrefab(GameObject aPrefab)
    {
        myCharacterPrefab = aPrefab;
        CmdSpawnCharacter();
    }

    [Command]
    private void CmdSpawnCharacter()
    {
        GameObject go = Instantiate(myCharacterPrefab, this.transform);
        go.GetComponent<PlayerCharacter>().Name = myName;

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);

        //myCharacter = go.GetComponent<PlayerCharacter>();
    }
}
