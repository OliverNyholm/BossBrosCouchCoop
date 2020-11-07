using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{

    [Header("The HUD gameobjects for each player")]
    [SerializeField]
    private List<GameObject> myPlayers = new List<GameObject>();

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

        myPlayerClassIndex = new List<int>(myPlayers.Count);
        myPlayerColorIndex = new List<int>(myPlayers.Count);
        for (int index = 0; index < myPlayers.Count; index++) //This loop could probably be combined with the latter one...
        {
            CharacterSelector characterSelector = myPlayers[index].GetComponent<CharacterSelector>();
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

            myPlayers[index].SetActive(false);
            myGnomes[index].SetActive(false);
        }

        for (int index = 0; index < characters.Count; index++)
        {
            myPlayers[index].SetActive(true);
            myGnomes[index].SetActive(true);
            CharacterSelector characterSelector = GetAvailableCharacterSelector(out int availableIndex);
            SetupCharacterSelector(characterSelector, characters[index], availableIndex);
            characterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]], myClassRoleSprites);
            characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);

            myGnomes[index].GetComponent<CharacterSelectPlayer>().SetCharacterSelector(characterSelector);
            myGnomes[index].GetComponent<CharacterSelectTargetingComponent>().SetPlayerIndex(index);
        }

        if (characters.Count == 0)
        {
            myPlayers[0].SetActive(true);
            myGnomes[0].SetActive(true);
            PlayerSelectData selectData = new PlayerSelectData(PlayerControls.CreateWithKeyboardBindings(), null, null, "DebugPlayer");
            CharacterSelector characterSelector = GetAvailableCharacterSelector(out int availableIndex);
            SetupCharacterSelector(characterSelector, selectData, availableIndex);
            characterSelector.SetClass(myClassHuds[myPlayerClassIndex[0]], myClassRoleSprites);
            characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[0]]);

            myGnomes[0].GetComponent<CharacterSelectPlayer>().SetCharacterSelector(characterSelector);
            myGnomes[0].GetComponent<CharacterSelectTargetingComponent>().SetPlayerIndex(0);
            myCharacterGameData.AddPlayerData(selectData.myPlayerControls, "DebugPlayer");
        }
        
        myNextLevel = myCharacterGameData.mySceneToLoad;
    }

    CharacterSelector GetAvailableCharacterSelector(out int anIndex)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>().PlayerControls == null)
            {
                anIndex = index;
                return myPlayers[index].GetComponent<CharacterSelector>();
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
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>() == aCharacterSelector)
            {
                myPlayerClassIndex[index] = GetNextAvailableOption(CharacterSelector.SelectionState.Class, myClassHuds, myPlayerClassIndex, myPlayerClassIndex[index], aModifier);
                aCharacterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]], myClassRoleSprites);
            }
        }
    }

    public void GetNextColor(CharacterSelector aCharacterSelector, int aModifier)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>() == aCharacterSelector)
            {
                myPlayerColorIndex[index] = GetNextAvailableOption(CharacterSelector.SelectionState.Color, myColorSchemes, myPlayerColorIndex, myPlayerColorIndex[index], aModifier);

                aCharacterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            }
        }
    }

    public void PlayerSetState(CharacterSelector aCharacterSelector, CharacterSelector.SelectionState aState)
    {
        aCharacterSelector.State = aState;
        switch (aCharacterSelector.State)
        {
            case CharacterSelector.SelectionState.Leave:
                SceneManager.LoadScene("LevelSelect");
                break;
            case CharacterSelector.SelectionState.Class:
                aCharacterSelector.SetInstructions("Choose your class and press Right Bumper when ready.");
                break;
            case CharacterSelector.SelectionState.Color:
                aCharacterSelector.SetInstructions("Choose a colour and press Right Bumper when happy!");
                RemoveSameSelections(aState);
                break;
            case CharacterSelector.SelectionState.Ready:
                aCharacterSelector.SetInstructions("Press Start when everyone is ready");
                RemoveSameSelections(aState);
                break;
            default:
                break;
        }
    }

    private void RemoveSameSelections(CharacterSelector.SelectionState aState)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if (!myPlayers[index].activeInHierarchy)
                continue;

            CharacterSelector characterSelector = myPlayers[index].GetComponent<CharacterSelector>();
            if (characterSelector.State >= aState)
                continue;

            if (!IsSelectionAvailable(characterSelector.State, myPlayerClassIndex, myPlayerClassIndex[index]))
            {
                myPlayerClassIndex[index] = GetNextAvailableOption(characterSelector.State, myClassHuds, myPlayerClassIndex, myPlayerClassIndex[index]);
                characterSelector.SetClass(myClassHuds[myPlayerClassIndex[index]], myClassRoleSprites);
            }
            if (!IsSelectionAvailable(characterSelector.State, myPlayerColorIndex, myPlayerColorIndex[index]))
            {
                myPlayerColorIndex[index] = GetNextAvailableOption(characterSelector.State, myColorSchemes, myPlayerColorIndex, myPlayerColorIndex[index]);
                characterSelector.SetColor(myColorSchemes[myPlayerColorIndex[index]]);
            }
        }
    }

    private bool IsSelectionAvailable(CharacterSelector.SelectionState aState, List<int> aPlayerIndexList, int aIndex)
    {
        for (int index = 0; index < aPlayerIndexList.Count; index++)
        {
            if (myPlayers[index].GetComponent<CharacterSelector>().State > aState && aPlayerIndexList[index] == aIndex)
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
        for (int index = 0; index < myPlayers.Count; index++)
        {
            CharacterSelector playerSelector = myPlayers[index].GetComponent<CharacterSelector>();
            if (playerSelector.PlayerControls == null)
                continue;

            if (playerSelector.State != CharacterSelector.SelectionState.Ready)
                return;
        }

        for (int index = 0; index < myPlayers.Count; index++)
        {
            PlayerControls playerControls = myPlayers[index].GetComponent<CharacterSelector>().PlayerControls;
            if (playerControls == null)
                continue;

            myCharacterGameData.AddCharacterData(playerControls, myClassHuds[myPlayerClassIndex[index]], myColorSchemes[myPlayerColorIndex[index]]);
        }

        SceneManager.LoadScene(myNextLevel);
    }

    private int GetClassIndex(ClassData aClassData)
    {
        return myClassHuds.IndexOf(aClassData);
    }

    private int GetColorIndex(ColorScheme aColorScheme)
    {
        return myColorSchemes.IndexOf(aColorScheme);
    }
}
