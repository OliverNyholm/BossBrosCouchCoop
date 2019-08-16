using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [Header("Children that show Tutorial Facts")]
    [SerializeField]
    private Text myTutorialText = null;
    [SerializeField]
    private Image myInfoImage = null;
    [SerializeField]
    private Image myKeyToPress = null;

    [SerializeField]
    private List<GameObject> mySpellInfoPanels = new List<GameObject>();
    [SerializeField]
    private GameObject myPlayersReadyParent = null;
    [SerializeField]
    private Image myHighlightSpells = null;
    [SerializeField]
    private Image myHighlightSecondSpells = null;
    [SerializeField]
    private Image myHighlighError = null;


    [Header("Prefab and Sprites")]
    [SerializeField]
    private GameObject myPlayersReadyPrefab = null;
    [SerializeField]
    private Sprite myControllerSprite = null;
    [SerializeField]
    private Sprite myInfoSprite = null;
    [SerializeField]
    private Sprite myCompletedSprite = null;
    [SerializeField]
    private Sprite myUncompletedSprite = null;


    private int myCurrentTutorialSpellIndex;

    private void Start()
    {
        List<GameObject> players = FindObjectOfType<TargetHandler>().GetAllPlayers();
        for (int index = 0; index < players.Count; index++)
        {
            GameObject instance = Instantiate(myPlayersReadyPrefab, myPlayersReadyParent.transform);
            instance.GetComponentInChildren<Text>().text = players[index].GetComponent<Character>().myName;
        }
    }

    void Update()
    {
        if (myKeyToPress.enabled)
        {
            Color color = myKeyToPress.color;
            color.a = Mathf.Abs(Mathf.Sin(Time.time * 4.0f));
            myKeyToPress.color = color;
        }

        if (myHighlightSpells.enabled)
        {
            Color color = myHighlightSpells.color;
            color.a = Mathf.Abs(Mathf.Sin(Time.time * 4.0f));
            myHighlightSpells.color = color;
        }

        if (myHighlightSecondSpells.enabled)
        {
            Color color = myHighlightSpells.color;
            color.a = Mathf.Abs(Mathf.Sin(Time.time * 4.0f));
            myHighlightSecondSpells.color = color;
        }

        if (myHighlighError.enabled)
        {
            Color color = myHighlighError.color;
            color.a = Mathf.Abs(Mathf.Sin(Time.time * 4.0f));
            myHighlighError.color = color;
        }
    }

    public void SetData(string aTutorialText, Sprite aKeyToPressSprite)
    {
        myTutorialText.text = aTutorialText;
        if (aKeyToPressSprite)
        {
            myInfoImage.sprite = myControllerSprite;
            myKeyToPress.sprite = aKeyToPressSprite;
            myKeyToPress.enabled = true;
        }
        else
        {
            myInfoImage.sprite = myInfoSprite;
            myKeyToPress.enabled = false;
        }

        for (int index = 0; index < myPlayersReadyParent.transform.childCount; index++)
        {
            SetUncompletedAtIndex(index);
        }
    }

    public void SetSpellData(Spell aSpell, Resource aResource, int aPlayerIndex)
    {
        mySpellInfoPanels[aPlayerIndex].GetComponent<TutorialSpellUI>().SetDetails(aSpell, aResource);
        mySpellInfoPanels[aPlayerIndex].SetActive(true);
        myCurrentTutorialSpellIndex = aPlayerIndex;
    }

    public List<Spell> GetTutorialSpells()
    {
        List<Spell> spells = new List<Spell>(myPlayersReadyParent.transform.childCount);

        for (int index = 0; index < spells.Capacity; index++)
        {
            spells.Add(mySpellInfoPanels[index].GetComponent<TutorialSpellUI>().GetCurrentSpell());
        }

        return spells;
    }

    public void SetCompletedAtIndex(int aPlayerIndex)
    {
        myPlayersReadyParent.transform.GetChild(aPlayerIndex).GetComponentInChildren<Image>().sprite = myCompletedSprite;
        mySpellInfoPanels[aPlayerIndex].SetActive(false);
    }

    private void SetUncompletedAtIndex(int aPlayerIndex)
    {
        myPlayersReadyParent.transform.GetChild(aPlayerIndex).GetComponentInChildren<Image>().sprite = myUncompletedSprite;
    }

    public void SetSpellsHightlight(bool aShouldEnable, bool aIsSecondSpells)
    {
        if (aIsSecondSpells)
            myHighlightSecondSpells.enabled = aShouldEnable;
        else
            myHighlightSpells.enabled = aShouldEnable;
    }

    public void SetErrorHightlight(bool aShouldEnable)
    {
        myHighlighError.enabled = aShouldEnable;
    }
}
