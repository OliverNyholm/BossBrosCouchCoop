using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterHUD : MonoBehaviour
{
    [SerializeField]
    private Image myAvatarImage = null;
    [SerializeField]
    private Image myClassImage = null;
    [SerializeField]
    private Image myHealthbarImage = null;
    [SerializeField]
    private Image myShieldBarImage = null;
    [SerializeField]
    private Image myResourceBarImage = null;
    [SerializeField]
    private Text myHealthText = null;
    [SerializeField]
    private Text myResourceText = null;
    [SerializeField]
    private Text myNameText = null;
    [SerializeField]
    private GameObject myBuffParent = null;
    [SerializeField]
    private GameObject myBuffPrefab = null;

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
        float width = myHealthbarImage.rectTransform.rect.width;

        float pivotX = myHealthbarImage.rectTransform.anchoredPosition.x;
        float x = pivotX - (width / 2f) + (width * myHealthbarImage.fillAmount) - myShieldBarImage.rectTransform.rect.width / 2;
        myShieldBarImage.rectTransform.anchoredPosition = new Vector2(x, myShieldBarImage.rectTransform.anchoredPosition.y);

        float fillAmount = (float)aShieldValue / aCurrentHealth;

        if (fillAmount > myHealthbarImage.fillAmount)
            fillAmount = myHealthbarImage.fillAmount;

        myShieldBarImage.fillAmount = fillAmount;
    }

    public void SetResourceBarFillAmount(float aValue)
    {
        myResourceBarImage.fillAmount = aValue;
    }

    public void SetResourceBarColor(Color aColor)
    {
        myResourceBarImage.color = aColor;
    }

    public void SetHealthbarColor(Color aColor)
    {
        myHealthbarImage.color = aColor;
    }

    public void SetAvatarSprite(Sprite aSprite)
    {
        myAvatarImage.sprite = aSprite;
    }

    public void SetClassSprite(Sprite aSprite)
    {
        myClassImage.sprite = aSprite;
    }

    public void SetName(string aName)
    {
        if(myNameText)
            myNameText.text = aName;
    }

    public void SetNameColor(Color aColor)
    {
        if(myNameText)
            myNameText.color = aColor;
    }

    public void SetHudColor(Color aColor)
    {
        if(myNameText)
             myNameText.color = aColor;
        myHealthbarImage.color = aColor;
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

    public void ToggleUIText(int aPlayerID)
    {
        myHealthText.enabled = !myHealthText.enabled;
        myResourceText.enabled = !myResourceText.enabled;

        PostMaster.Instance.PostMessage(new Message(MessageType.UITextToggle, aPlayerID));
    }
}
