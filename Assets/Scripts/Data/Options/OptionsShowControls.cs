using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsShowControls : OptionsBase
{
    [SerializeField]
    private Image myImageToSetSpriteOn = null;

    [SerializeField]
    private Sprite myControllerSprite = null;

    public override void OnSelected()
    {
        myImageToSetSpriteOn.sprite = myControllerSprite;
        myImageToSetSpriteOn.material = null;
    }

    public override void OnDeselected()
    {

    }

    public override void NextOptions()
    {
    }

    public override void InitData()
    {
    }

    public override void SetData()
    {
    }
}
