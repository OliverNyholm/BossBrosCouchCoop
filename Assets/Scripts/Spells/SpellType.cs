using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[System.Flags]
public enum SpellType
{
    None = 0,
    Damage = 1,
    Heal = 1 << 1,
    Interrupt = 1 << 2,
    Taunt = 1 << 3,
    Ressurect = 1 << 4,
    Stun = 1 << 5
} //Adding anything to this enum must be done at the end, as it will otherwise fuck up all the other spells.

public enum SpellOverTimeType
{
    None = 0,
    DOT = 1,
    HOT = 1 << 1,
    Shield = 1 << 2,
    Stats = 1 << 3
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SpellType))]
[CustomPropertyDrawer(typeof(SpellOverTimeType))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    [System.Flags]
    enum TriBool
    {
        Unset = 0,
        False = 1,
        True = 2,
        Both = 3
    }

    struct Entry
    {
        public string label;
        public int mask;
        public TriBool currentValue;
    }

    List<SerializedProperty> _properties;
    List<Entry> _entries;
    int _rowCount;
    int _columnCount;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (_properties == null)
            Initialize(property);
        return _rowCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        Rect labelRect = new Rect(
            position.x,
            position.y,
            EditorGUIUtility.labelWidth,
            position.height);
        EditorGUI.LabelField(labelRect, label);

        float buttonStride = (position.width - labelRect.width) / _columnCount;
        float buttonWidth = buttonStride * 1.0f;
        Rect buttonRect = new Rect(
            labelRect.max.x,
            labelRect.y,
            buttonWidth,
            EditorGUIUtility.singleLineHeight);

        int column = 0;

        GUIStyle mixedButton = new GUIStyle(GUI.skin.button);
        mixedButton.normal.textColor = Color.grey;

        for (int i = 0; i < _entries.Count; i++)
        {
            //Debug.Log(_entries[i].label + " : " + _entries[i].currentValue);
            var entry = _entries[i];

            EditorGUI.BeginChangeCheck();
            bool pressed = GUI.Toggle(
                buttonRect,
                entry.currentValue == TriBool.True,
                entry.label,
                entry.currentValue == TriBool.Both ? mixedButton : GUI.skin.button);

            if (EditorGUI.EndChangeCheck())
            {

                if (pressed)
                {
                    foreach (var prop in _properties)
                        prop.intValue |= entry.mask;
                    entry.currentValue = TriBool.True;
                }
                else
                {
                    foreach (var prop in _properties)
                        prop.intValue &= ~entry.mask;
                    entry.currentValue = TriBool.False;
                }
                _entries[i] = entry;
            }

            buttonRect.x += buttonStride;
            if (++column >= _columnCount)
            {
                column = 0;
                buttonRect.x = labelRect.max.x;
                buttonRect.y += buttonRect.height;
            }
        }

        foreach (var prop in _properties)
            prop.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();

        EditorGUI.showMixedValue = false;
    }

    void Initialize(SerializedProperty property)
    {
        var allTargetObjects = property.serializedObject.targetObjects;
        _properties = new List<SerializedProperty>(allTargetObjects.Length);
        foreach (var targetObject in allTargetObjects)
        {
            SerializedObject iteratedObject = new SerializedObject(targetObject);
            SerializedProperty iteratedProperty = iteratedObject.FindProperty(property.propertyPath);
            if (iteratedProperty != null)
                _properties.Add(iteratedProperty);
        }

        var parentType = property.serializedObject.targetObject.GetType();
        var fieldInfo = parentType.GetField(property.propertyPath);
        var enumType = fieldInfo.FieldType;
        var trueNames = System.Enum.GetNames(enumType);

        var typedValues = GetTypedValues(property, enumType);
        var display = property.enumDisplayNames;
        var names = property.enumNames;

        _entries = new List<Entry>();

        for (int i = 0; i < names.Length; i++)
        {
            int sortedIndex = System.Array.IndexOf(trueNames, names[i]);
            int value = typedValues[sortedIndex];
            int bitCount = 0;

            for (int temp = value; (temp != 0 && bitCount <= 1); temp >>= 1)
                bitCount += temp & 1;

            //Debug.Log(names[i] + ": " + value + " ~ " + bitCount);

            if (bitCount != 1)
                continue;

            TriBool consensus = TriBool.Unset;
            foreach (var prop in _properties)
            {
                if ((prop.intValue & value) == 0)
                    consensus |= TriBool.False;
                else
                    consensus |= TriBool.True;
            }

            _entries.Add(new Entry { label = display[i], mask = value, currentValue = consensus });
        }

        _rowCount = Mathf.CeilToInt(_entries.Count / 4f);
        _columnCount = Mathf.Min(_entries.Count, (Mathf.CeilToInt(_entries.Count / 3f) == _rowCount) ? 3 : 4);
    }

    int[] GetTypedValues(SerializedProperty property, System.Type enumType)
    {
        var values = System.Enum.GetValues(enumType);
        var underlying = System.Enum.GetUnderlyingType(enumType);

        if (underlying == typeof(int))
            return ConvertFrom<int>(values);
        else if (underlying == typeof(uint))
            return ConvertFrom<uint>(values);
        else if (underlying == typeof(short))
            return ConvertFrom<short>(values);
        else if (underlying == typeof(ushort))
            return ConvertFrom<ushort>(values);
        else if (underlying == typeof(sbyte))
            return ConvertFrom<sbyte>(values);
        else if (underlying == typeof(byte))
            return ConvertFrom<byte>(values);
        else
            throw new System.InvalidCastException("Cannot use enum backing types other than byte, sbyte, ushort, short, uint, or int.");
    }

    int[] ConvertFrom<T>(System.Array untyped) where T : System.IConvertible
    {
        var typedValues = new int[untyped.Length];

        for (int i = 0; i < typedValues.Length; i++)
            typedValues[i] = System.Convert.ToInt32((T)untyped.GetValue(i));

        return typedValues;
    }
}
#endif