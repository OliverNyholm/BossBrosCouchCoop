using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InControl;

public class CharacterSelector : MonoBehaviour
{
    [Header("Image to show class icon")]
    [SerializeField]
    private Image myClassIcon = null;
    [SerializeField]
    private Image myRoleIcon = null;

    [Header("Text to show player name")]
    [SerializeField]
    private TextMeshProUGUI myNameText = null;

    [Header("Text to show class name")]
    [SerializeField]
    private TextMeshProUGUI myClassNameText = null;

    [Header("Text to show class description")]
    [SerializeField]
    private TextMeshProUGUI myDescriptionText = null;

    [Header("Text to show current insctructions")]
    [SerializeField]
    private TextMeshProUGUI myInstructionsText = null;

    [SerializeField]
    private List<GameObject> mySpells = new List<GameObject>(4);

    [SerializeField]
    private CharacterSelectSpellInfo mySpellInfo = null;

    private GnomeAppearance myGnomeAppearance;

    public PlayerControls PlayerControls { get; set; }
    private CharacterSelectManager myManager;
    private ClassData myCurrentClassData;

    public delegate void OnClassChanged();
    public event OnClassChanged EventOnClassChanged;

    private int myPlayerIndex = -1;

    private bool myCanMoveLeftOrRight = true;
    private float mySelectedClassRawValue;
    private float mySelectedClassThreshold = 0.2f;

    public enum SelectionState
    {
        Leave,
        Class,
        Ready,
        Count
    }

    public SelectionState State { get; set; }

    private void Awake()
    {
        Hide();
    }

    private void Update()
    {
        Color color = myInstructionsText.color;
        color.a = Mathf.Abs(Mathf.Sin(Time.time));
        myInstructionsText.color = color;

        if (PlayerControls == null)
            return;

        if (PlayerControls.TargetEnemy.WasPressed)
        {
            myManager.PlayerSetState(this, --State);
        }

        if (PlayerControls.Jump.WasPressed || PlayerControls.Start.WasPressed)
        {
            myManager.PlayerSetState(this, ++State);
        }

        if (State == SelectionState.Ready)
            return;

        if (myCanMoveLeftOrRight && PlayerControls.Left.RawValue > mySelectedClassThreshold)
        {
            mySelectedClassRawValue = PlayerControls.Left.RawValue;
            myCanMoveLeftOrRight = false;
            switch (State)
            {
                case SelectionState.Class:
                    myManager.GetNextCharacter(this, -1);
                    break;
            }
        }
        if (myCanMoveLeftOrRight && PlayerControls.Right.RawValue > mySelectedClassThreshold)
        {
            mySelectedClassRawValue = PlayerControls.Right.RawValue;
            myCanMoveLeftOrRight = false;
            switch (State)
            {
                case SelectionState.Class:
                    myManager.GetNextCharacter(this, 1);
                    break;
            }
        }

        float resetThreshold = mySelectedClassThreshold - 0.05f;
        myCanMoveLeftOrRight = PlayerControls.Right.RawValue < resetThreshold && PlayerControls.Left.RawValue < resetThreshold;
    }

    public void Show(PlayerControls aPlayerControls, string aName, CharacterSelectManager aManager, GnomeAppearance aGnomeAppearance)
    {
        PlayerControls = aPlayerControls;
        myManager = aManager;
        myNameText.text = aName;
        myGnomeAppearance = aGnomeAppearance;

        myClassIcon.enabled = true;
        myRoleIcon.enabled = true;
        myNameText.enabled = true;
        myClassNameText.enabled = true;
        myDescriptionText.enabled = true;

        State = SelectionState.Class;
    }

    public void Hide()
    {
        PlayerControls = null;
        myManager = null;

        myClassIcon.enabled = false;
        myRoleIcon.enabled = false;
        myNameText.enabled = false;
        myClassNameText.enabled = false;
        myDescriptionText.enabled = false;
    }

    public void SetColor(ColorScheme aColorScheme)
    {
        myNameText.color = aColorScheme.myColor;
        myGnomeAppearance.SetColorMaterial(aColorScheme.myMaterial);
        myGnomeAppearance.GetComponentInParent<CharacterSelectUIComponent>().SetCharacterColor(aColorScheme.myColor);
    }

    public void SetClass(ClassData aClassData, List<Sprite> someClassRoleSprite)
    {
        myCurrentClassData = aClassData;

        myClassIcon.sprite = aClassData.myIconSprite;
        myRoleIcon.sprite = someClassRoleSprite[(int)aClassData.myClassRole];
        myClassNameText.text = aClassData.myName;
        myDescriptionText.text = aClassData.myDescription;

        myClassNameText.color = aClassData.myClassColor;
        myDescriptionText.color = aClassData.myClassColor;

        myGnomeAppearance.EquipItemInHand(aClassData.myLeftItem, true);
        myGnomeAppearance.EquipItemInHand(aClassData.myRightItem, false);

        EventOnClassChanged?.Invoke();
    }

    public void SetInstructions(string aInstruction)
    {
        myInstructionsText.text = aInstruction;
    }
    
    public void SetIndex(int anIndex)
    {
        myPlayerIndex = anIndex;
    }

    public int GetIndex()
    {
        return myPlayerIndex;
    }

    public string GetName()
    {
        return myNameText.text;
    }

    public ClassData GetCurrentClassData()
    {
        return myCurrentClassData;
    }

    public List<GameObject> GetSpells()
    {
        return mySpells;
    }

    public void ShowSpellInfo(Spell aSpell)
    {
        mySpellInfo.ShowSpellInfo(aSpell);
    }

    public void HideSpellInfo()
    {
        mySpellInfo.HideSpellInfo();
    }
}
