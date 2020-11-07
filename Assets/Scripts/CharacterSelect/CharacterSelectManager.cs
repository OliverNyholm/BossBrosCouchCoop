using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{

    [Header("The HUD gameobjects for each player")]
    [SerializeField]
    private List<GameObject> myPlayerHuds = new List<GameObject>();
    [SerializeField]
    private List<GameObject> myPlayerReadyText = new List<GameObject>();

    [Header("The classes to be selectable")]
    [SerializeField]
    private List<ClassData> myClassHuds = new List<ClassData>();

    [Header("The colors to be selectable")]
    [SerializeField]
    private List<ColorScheme> myColorSchemes = new List<ColorScheme>();

    [Space(5)]
    [SerializeField]
    private List<GameObject> myGnomes = new List<GameObject>();
    [SerializeField]
    private List<Sprite> myClassRoleSprites = new List<Sprite>();

    private CharacterGameData myCharacterGameData = null;

    private List<int> myPlayerClassIndex;
    private List<int> myPlayerColorIndex;

    [Header("Level to load")]
    [SerializeField]
    private string myNextLevel = "Coop";

    void Start()
    {
        myCharacterGameData = FindObjectOfType<CharacterGameData>();
        List<PlayerSelectData> characters = myCharacterGameData.GetPlayerData();

        myPlayerClassIndex = new List<int>(myPlayerHuds.Count);
        myPlayerColorIndex = new List<int>(myPlayerHuds.Count);
        for (int index = 0; index < myPlayerHuds.Count; index++) //This loop could probably be combined with the latter one...
        {
            CharacterSelector characterSelector = myPlayerHuds[index].GetComponent<CharacterSelector>();
            characterSelector.SetIndex(index);
            PlayerSetState(characterSelector, CharacterSelector.SelectionState.Class);

            if(characters.Count - 1 >= index && characters[index].myClassData)
            {
                myPlayerClassIndex.Add(GetClassIndex(characters[index].myClassData));
                myPlayerColorIndex.Add(GetColorIndex(characters[index].myColorScheme));
            }
            else
            {
                myPlayerClassIndex.Add(index);
                myPlayerColorIndex.Add(index);
            }

            myPlayerHuds[index].SetActive(false);
            myGnomes[index].SetActive(false);
        }

        TargetHandler targetHandler = FindObjectOfType<TargetHandler>();
        for (int index = 0; index < characters.Count; index++)
        {
            myPlayerHuds[index].SetActive(true);
            myGnomes[index].SetActive(true);
            CharacterSelector characterSelector = GetAvailableCharacterSelector(out int availableIndex);
            SetupCharacterSelector(characterSelector, characters[index], availableIndex);
            characterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]], myClassRoleSprites);
            characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            myPlayerReadyText[index].GetComponent<TextMeshPro>().color = myColorSchemes[myPlayerColorIndex[index]].myColor;

            myGnomes[index].GetComponent<CharacterSelectPlayer>().SetCharacterSelector(characterSelector);
            myGnomes[index].GetComponent<CharacterSelectTargetingComponent>().SetPlayerIndex(index);

            targetHandler.AddPlayer(myGnomes[index]);
        }

        if (characters.Count == 0)
        {
            myPlayerHuds[0].SetActive(true);
            myGnomes[0].SetActive(true);
            PlayerSelectData selectData = new PlayerSelectData(PlayerControls.CreateWithKeyboardBindings(), null, null, "DebugPlayer");
            CharacterSelector characterSelector = GetAvailableCharacterSelector(out int availableIndex);
            SetupCharacterSelector(characterSelector, selectData, availableIndex);
            characterSelector.SetClass(myClassHuds[myPlayerClassIndex[0]], myClassRoleSprites);
            characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[0]]);

            myGnomes[0].GetComponent<CharacterSelectPlayer>().SetCharacterSelector(characterSelector);
            myGnomes[0].GetComponent<CharacterSelectTargetingComponent>().SetPlayerIndex(0);
            myCharacterGameData.AddPlayerData(selectData.myPlayerControls, "DebugPlayer");

            targetHandler.AddPlayer(myGnomes[0]);
            DebugActivateOtherGnomes(targetHandler, 1);
            DebugActivateOtherGnomes(targetHandler, 2);
            DebugActivateOtherGnomes(targetHandler, 3);
        }

        myNextLevel = myCharacterGameData.mySceneToLoad;
    }

    CharacterSelector GetAvailableCharacterSelector(out int anIndex)
    {
        for (int index = 0; index < myPlayerHuds.Count; index++)
        {
            if (myPlayerHuds[index].GetComponent<CharacterSelector>().PlayerControls == null)
            {
                anIndex = index;
                return myPlayerHuds[index].GetComponent<CharacterSelector>();
            }
        }

        anIndex = -1;
        return null;
    }

    void SetupCharacterSelector(CharacterSelector aCharacterSelector, PlayerSelectData aCharacterSelectData, int anIndex)
    {
        if (!aCharacterSelector)
            return;

        aCharacterSelector.Show(aCharacterSelectData.myPlayerControls, aCharacterSelectData.myName, this, myGnomes[anIndex].GetComponentInChildren<GnomeAppearance>());
        PlayerSetState(aCharacterSelector, CharacterSelector.SelectionState.Class);
    }

    public void GetNextCharacter(CharacterSelector aCharacterSelector, int aModifier)
    {
        for (int index = 0; index < myPlayerHuds.Count; index++)
        {
            if (myPlayerHuds[index].GetComponent<CharacterSelector>() == aCharacterSelector)
            {
                myPlayerClassIndex[index] = GetNextAvailableOption(CharacterSelector.SelectionState.Class, myClassHuds, myPlayerClassIndex, myPlayerClassIndex[index], aModifier);
                aCharacterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]], myClassRoleSprites);
            }
        }
    }

    public void PlayerSetState(CharacterSelector aCharacterSelector, CharacterSelector.SelectionState aState)
    {
        if (aState == CharacterSelector.SelectionState.Count)
        {
            aCharacterSelector.State = CharacterSelector.SelectionState.Ready;
            return;
        }

        aCharacterSelector.State = aState;
        switch (aCharacterSelector.State)
        {
            case CharacterSelector.SelectionState.Leave:
                if (DoAllPlayersHaveSameState(CharacterSelector.SelectionState.Class, aCharacterSelector.GetIndex()))
                    SceneManager.LoadScene("LevelSelect");
                else
                    aCharacterSelector.State = CharacterSelector.SelectionState.Class;

                break;
            case CharacterSelector.SelectionState.Class:
                myPlayerReadyText[aCharacterSelector.GetIndex()].GetComponent<MeshRenderer>().enabled = false;
                break;
            case CharacterSelector.SelectionState.Ready:
                RemoveSameSelections(aState);
                if (DoAllPlayersHaveSameState(CharacterSelector.SelectionState.Ready, aCharacterSelector.GetIndex()))
                    StartPlaying();
                else
                    myPlayerReadyText[aCharacterSelector.GetIndex()].GetComponent<MeshRenderer>().enabled = true;

                break;
            default:
                break;
        }
    }

    private void RemoveSameSelections(CharacterSelector.SelectionState aState)
    {
        for (int index = 0; index < myPlayerHuds.Count; index++)
        {
            if (!myPlayerHuds[index].activeInHierarchy)
                continue;

            CharacterSelector characterSelector = myPlayerHuds[index].GetComponent<CharacterSelector>();
            if (characterSelector.State >= aState)
                continue;

            if (!IsSelectionAvailable(characterSelector.State, myPlayerClassIndex, myPlayerClassIndex[index]))
            {
                myPlayerClassIndex[index] = GetNextAvailableOption(characterSelector.State, myClassHuds, myPlayerClassIndex, myPlayerClassIndex[index]);
                characterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]], myClassRoleSprites);
            }
        }
    }

    private bool IsSelectionAvailable(CharacterSelector.SelectionState aState, List<int> aPlayerIndexList, int aIndex)
    {
        for (int index = 0; index < aPlayerIndexList.Count; index++)
        {
            if (myPlayerHuds[index].GetComponent<CharacterSelector>().State > aState && aPlayerIndexList[index] == aIndex)
                return false;
        }

        return true;
    }

    private int GetNextAvailableOption<T>(CharacterSelector.SelectionState aState, List<T> aOptionList, List<int> aPlayerIndexList, int aIndex, int aDirection = 1)
    {
        aIndex += aDirection;
        for (int counter = 0; counter < aOptionList.Count; counter++)
        {
            if (aIndex >= aOptionList.Count)
                aIndex = 0;
            if (aIndex < 0)
                aIndex = aOptionList.Count - 1;

            if (IsSelectionAvailable(aState, aPlayerIndexList, aIndex))
                return aIndex;

            aIndex += aDirection;
        }

        return -1;
    }

    public void StartPlaying()
    {
        for (int index = 0; index < myPlayerHuds.Count; index++)
        {
            CharacterSelector playerSelector = myPlayerHuds[index].GetComponent<CharacterSelector>();
            if (playerSelector.PlayerControls == null)
                continue;

            if (playerSelector.State != CharacterSelector.SelectionState.Ready)
                return;
        }

        for (int index = 0; index < myPlayerHuds.Count; index++)
        {
            PlayerControls playerControls = myPlayerHuds[index].GetComponent<CharacterSelector>().PlayerControls;
            if (playerControls == null)
                continue;

            myCharacterGameData.AddCharacterData(playerControls, myClassHuds[myPlayerClassIndex[index]], myColorSchemes[myPlayerColorIndex[index]]);
        }

        SceneManager.LoadScene(myNextLevel);
    }

    private int GetClassIndex(ClassData aClassData)
    {
        return Mathf.Clamp(myClassHuds.IndexOf(aClassData), 0, myClassHuds.Count - 1);
    }

    private int GetColorIndex(ColorScheme aColorScheme)
    {
        return myColorSchemes.IndexOf(aColorScheme);
    }

    private void DebugActivateOtherGnomes(TargetHandler aTargetHandler, int anIndex)
    {
        myGnomes[anIndex].SetActive(true);
        PlayerSelectData selectData = new PlayerSelectData(PlayerControls.CreateWithJoystickBindings(), null, null, "DebugPlayer");
        CharacterSelector characterSelector = GetAvailableCharacterSelector(out int availableIndex);
        SetupCharacterSelector(characterSelector, selectData, availableIndex);
        characterSelector.SetClass(myClassHuds[myPlayerClassIndex[anIndex]], myClassRoleSprites);
        characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[anIndex]]);

        myGnomes[anIndex].GetComponent<CharacterSelectPlayer>().SetCharacterSelector(characterSelector);
        myGnomes[anIndex].GetComponent<CharacterSelectTargetingComponent>().SetPlayerIndex(anIndex);

        aTargetHandler.AddPlayer(myGnomes[anIndex]);
    }

    private bool DoAllPlayersHaveSameState(CharacterSelector.SelectionState aState, int anIgnoreIndex)
    {
        for (int index = 0; index < myPlayerHuds.Count; index++)
        {
            if (anIgnoreIndex == index)
                continue;

            CharacterSelector characterSelector = myPlayerHuds[index].GetComponent<CharacterSelector>();
            if (characterSelector.PlayerControls == null)
                continue;

            if (characterSelector.State != aState)
                return false;
        }

        return true;
    }
}
