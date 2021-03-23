using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
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
    private Image myTargetBar = null;
    [SerializeField]
    private Text myHealthText = null;
    [SerializeField]
    private Text myResourceText = null;
    [SerializeField]
    private TextMeshProUGUI myNameText = null;
    [SerializeField]
    private GameObject myBuffParent = null;
    [SerializeField]
    private GameObject myBuffPrefab = null;

    private List<GameObject> myBuffs = new List<GameObject>();

    private PoolManager myPoolManager;

    private void Awake()
    {
        myPoolManager = PoolManager.Instance;
        myPoolManager.AddPoolableObjects(myBuffPrefab, myBuffPrefab.GetComponent<UniqueID>().GetID(), 6);
    }

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
        if (myResourceBarImage)
            myResourceBarImage.fillAmount = aValue;
    }

    public void SetResourceBarColor(Color aColor)
    {
        if (myResourceBarImage)
            myResourceBarImage.color = aColor;
    }

    public void SetHealthbarColor(Color aColor)
    {
        if (myHealthbarImage)
            myHealthbarImage.color = aColor;
    }

    public void SetAvatarSprite(Sprite aSprite)
    {
        if (myAvatarImage)
            myAvatarImage.sprite = aSprite;
    }

    public void SetClassSprite(Sprite aSprite)
    {
        if (myClassImage)
            myClassImage.sprite = aSprite;
    }

    public void SetName(string aName)
    {
        if (myNameText)
            myNameText.text = aName;
    }

    public void SetNameColor(Color aColor)
    {
        if (myNameText)
            myNameText.color = aColor;
    }

    public void SetHudColor(Color aColor)
    {
        if (myNameText)
             myNameText.color = aColor;
        if (myHealthbarImage)
            myHealthbarImage.color = aColor;
    }

    public void SetHealthText(string aString)
    {
        if (myHealthText)
            myHealthText.text = aString;
    }

    public void SetResourceText(string aString)
    {
        if (myResourceText)
            myResourceText.text = aString;
    }

    public GameObject AddBuffAndGetRef(SpellOverTime aSpell)
    {
        GameObject buff = myPoolManager.GetPooledObject(myBuffPrefab.GetComponent<UniqueID>().GetID());
        if (!buff)
            return null;

        buff.transform.SetParent(myBuffParent.transform, false);

        buff.GetComponentInChildren<Image>().sprite = aSpell.mySpellIcon;
        buff.GetComponentInChildren<TextMeshProUGUI>().text = aSpell.GetStackCount() > 1 ? aSpell.GetStackCount().ToString() : "";
        myBuffs.Add(buff);

        return buff;
    }

    public void UpdateBuffCount(int anIndex, int aBuffCount)
    {
        myBuffs[anIndex].GetComponentInChildren<TextMeshProUGUI>().text = aBuffCount > 1 ? aBuffCount.ToString() : ""; ;
    }

    public void RemoveBuff(GameObject aBuffWidget)
    {
        myPoolManager.ReturnObject(aBuffWidget, myBuffPrefab.GetComponent<UniqueID>().GetID());
        myBuffs.Remove(aBuffWidget);
    }

    public void SetMinionTargetHudColor(Color aColor)
    {
        if (myTargetBar)
            myTargetBar.color = aColor;
    }


    public void ToggleUIText(int aPlayerID)
    {
        if (myHealthText || myResourceText)
            return;

        myHealthText.enabled = !myHealthText.enabled;
        myResourceText.enabled = !myResourceText.enabled;

        PostMaster.Instance.PostMessage(new Message(MessageCategory.UITextToggle, aPlayerID));
    }
}
