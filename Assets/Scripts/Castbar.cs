using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Castbar : MonoBehaviour
{
    [SerializeField]
    private float myFadeoutSpeed;

    [SerializeField]
    private Image myCastbarUI;
    [SerializeField]
    private Image mySpellIconUI;
    [SerializeField]
    private Text mySpellNameUI;
    [SerializeField]
    private Text myCastTimeUI;

    private Coroutine fadeCoroutine;

    public void ShowCastbar()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void HideCastbar()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    public void FadeOutCastbar()
    {
        fadeCoroutine = StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        float timeLeft = myFadeoutSpeed;

        while (GetComponent<CanvasGroup>().alpha > 0.0f)
        {
            timeLeft -= Time.deltaTime;
            GetComponent<CanvasGroup>().alpha = timeLeft / myFadeoutSpeed;
            yield return null;
        }
    }

    public void SetCastbarFillAmount(float aValue)
    {
        myCastbarUI.fillAmount = aValue;
    }

    public void SetSpellName(string aName)
    {
        mySpellNameUI.text = aName;
    }

    public void SetCastbarColor(Color aColor)
    {
        myCastbarUI.color = aColor;
    }

    public void SetSpellIcon(Sprite aSprite)
    {
        mySpellIconUI.sprite = aSprite;
    }

    public void SetCastTimeText(string aString)
    {
        myCastTimeUI.text = aString;
    }
}
