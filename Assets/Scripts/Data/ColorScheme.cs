using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorScheme", menuName = "Scriptables/ColorScheme")]
public class ColorScheme : ScriptableObject
{
    public Material myMaterial;
    public Sprite myAvatar;
    public Color myColor;
}
