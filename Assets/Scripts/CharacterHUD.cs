using UnityEngine;
using UnityEngine.UI;

public class CharacterHUD : MonoBehaviour
{
    [SerializeField]
    private Image myAvatarImage;
    [SerializeField]
    private Image myHealthbarImage;
    [SerializeField]
    private Image myResourceBarImage;
    [SerializeField]
    private Text myHealthText;
    [SerializeField]
    private Text myResourceText;
    [SerializeField]
    private Text myNameText;

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
}
