using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OptionsBase : MonoBehaviour
{
    public abstract void OnSelected();

    public abstract void OnDeselected();

    public abstract void GetData();

    public abstract void SetData();

    public abstract void NextOptions();
}
