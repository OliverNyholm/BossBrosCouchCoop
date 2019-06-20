using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorScheme", menuName = "Scriptables/ColorScheme")]
public class ColorScheme : ScriptableObject
{
    public Texture myTexture;
    public Sprite myAvatar;
    public Color myColor;
}
