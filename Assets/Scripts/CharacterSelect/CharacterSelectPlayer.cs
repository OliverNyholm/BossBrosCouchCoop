using System.Collections;
using System.Collections.Generic;

public class CharacterSelectPlayer : Player
{
    protected override void Start()
    {
    }

    protected override void Update()
    {
    }

    public void SetCharacterSelector(CharacterSelector aCharacterSelector)
    {
        GetComponent<CharacterSelectCastingComponent>().SetCharacterSelector(aCharacterSelector);
        GetComponent<CharacterSelectUIComponent>().SetCharacterSelector(aCharacterSelector);
        GetComponent<CharacterSelectClass>().SetCharacterSelector(aCharacterSelector);
    }
}
