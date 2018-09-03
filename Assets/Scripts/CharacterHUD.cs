using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterHUD : MonoBehaviour
{
    [SerializeField]
    private Image myAvatarImage;
    [SerializeField]
    private Image myHealthbarImage;
    [SerializeField]
    private Image myShieldBarImage;
    [SerializeField]
    private Image myResourceBarImage;
    [SerializeField]
    private Text myHealthText;
    [SerializeField]
    private Text myResourceText;
    [SerializeField]
    private Text myNameText;
    [SerializeField]
    private GameObject myBuffParent;
    [SerializeField]
    private GameObject myBuffPrefab;

    private List<GameObject> myBuffs = new List<GameObject>();

    public void Show()
    {
        GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void Hide()
    {
        GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void SetHealthBarFillAmount(float aValue)
    {
        myHealthbarImage.fillAmount = aValue;
    }

    public void SetShieldBar(int aShieldValue, int aCurrentHealth)
    {
        //float width = myHealthbarImage.rectTransform.rect.width;
        //float x = (myHealthbarImage.transform.position.x - (width / 2f)) + (width * myHealthbarImage.fillAmount) - myShieldBarImage.rectTransform.rect.width / 2;
        //myShieldBarImage.transform.position = new Vector2(x, myShieldBarImage.transform.position.y);

        float fillAmount = (float)aShieldValue / aCurrentHealth;

        if (fillAmount > myHealthbarImage.fillAmount)
            fillAmount = myHealthbarImage.fillAmount;

        myShieldBarImage.fillAmount = fillAmount;
    }

    public void SetResourceBarFillAmount(float aValue)
    {
        myResourceBarImage.fillAmount = aValue;
    }

    public void SetResourceBarFillAmount(Color aColor)
    {
        myResourceBarImage.color = aColor;
    }

    public void SetName(string aName)
    {
        myNameText.text = aName;
    }

    public void SetNameColor(Color aColor)
    {
        myNameText.color = aColor;
    }

    public void SetHealthText(string aString)
    {
        myHealthText.text = aString;
    }

    public void SetResourceText(string aString)
    {
        myResourceText.text = aString;
    }

    public void AddBuff(Sprite aSprite)
    {
        GameObject buff = Instantiate(myBuffPrefab, myBuffParent.transform);

        buff.GetComponent<Image>().sprite = aSprite;
        myBuffs.Add(buff);
    }

    public void RemoveBuff(int anIndex)
    {
        Destroy(myBuffs[anIndex]);
        myBuffs.RemoveAt(anIndex);
    }
}
