using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FriendHud : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private PlayerCharacterOLD myParent;
    [SerializeField]
    private GameObject myCharacter;

    public void SetParent(PlayerCharacterOLD aParent)
    {
        myParent = aParent;
    }

    public void SetCharacter(GameObject aCharacter)
    {
        myCharacter = aCharacter;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        myParent.AssignTargetUI(myCharacter);
    }
}
