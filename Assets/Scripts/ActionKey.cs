using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionKey : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField]
    private Button myButton;
    [SerializeField]
    private GameObject myDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        myDescription.GetComponentInChildren<Image>().enabled = true;
        myDescription.GetComponentInChildren<Text>().enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        myDescription.GetComponentInChildren<Image>().enabled = false;
        myDescription.GetComponentInChildren<Text>().enabled = false;
    }

    public void SetDescription(string aText)
    {
        myDescription.GetComponentInChildren<Text>().text = aText;
    }
}
