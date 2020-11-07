using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(SpellOverTime)), CanEditMultipleObjects]
public class SpellOverTimeInspectorEditor : SpellInspectorEditor
{
    public SerializedProperty
        myDuration,

        mySpellOverTimeType,
        myShieldValue,
        myIsCastManually,
        myNumberOfTicks,

        mySpeedMultiplier,
        myAttackSpeed,
        myDamageIncrease,
        myDamageMitigator;

    public override void OnEnable()
    {
        base.OnEnable();
        // Setup the SerializedProperties

        myDuration = serializedObject.FindProperty("myDuration");

        mySpellOverTimeType = serializedObject.FindProperty("mySpellOverTimeType");
        myShieldValue = serializedObject.FindProperty("myShieldValue");
        myIsCastManually = serializedObject.FindProperty("myIsCastManually");
        myNumberOfTicks = serializedObject.FindProperty("myNumberOfTicks");

        mySpeedMultiplier = serializedObject.FindProperty("mySpeedMultiplier");
        myAttackSpeed = serializedObject.FindProperty("myAttackSpeed");
        myDamageIncrease = serializedObject.FindProperty("myDamageIncrease");
        myDamageMitigator = serializedObject.FindProperty("myDamageMitigator");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(myIsCastManually, new GUIContent("Is Cast Manually"));
        if(!myIsCastManually.boolValue)
            EditorGUILayout.PropertyField(myPoolSize);

        EditorGUILayout.HelpBox("Spell Over Time Values", MessageType.None);

        EditorGUILayout.PropertyField(myDuration, new GUIContent("Spell Duration"));
        EditorGUILayout.PropertyField(mySpellOverTimeType);
        SpellOverTimeType spellOverTimeType = (SpellOverTimeType)mySpellOverTimeType.intValue;

        if (UtilityFunctions.HasSpellType(spellOverTimeType, SpellOverTimeType.DOT))
            EditorGUILayout.PropertyField(myDamage, new GUIContent("Damage Value"));

        if (UtilityFunctions.HasSpellType(spellOverTimeType, SpellOverTimeType.HOT))
            EditorGUILayout.PropertyField(myDamage, new GUIContent("Heal Value"));

        if(UtilityFunctions.HasSpellType(spellOverTimeType, SpellOverTimeType.DOT | SpellOverTimeType.HOT))
            EditorGUILayout.PropertyField(myNumberOfTicks, new GUIContent("Number of Ticks"));

        if (UtilityFunctions.HasSpellType(spellOverTimeType, SpellOverTimeType.Shield))
            EditorGUILayout.PropertyField(myShieldValue, new GUIContent("Shield Value"));

        if (UtilityFunctions.HasSpellType(spellOverTimeType, SpellOverTimeType.Stats))
        {
            EditorGUILayout.PropertyField(mySpeedMultiplier, new GUIContent("Speed Multiplier"));
            EditorGUILayout.PropertyField(myAttackSpeed, new GUIContent("Attack Speed"));
            EditorGUILayout.PropertyField(myDamageIncrease, new GUIContent("Damage Increase Value"));
            EditorGUILayout.PropertyField(myDamageMitigator, new GUIContent("Damage Mitigator"));
        }

        if (!myIsCastManually.boolValue)
        {
            EditorGUILayout.HelpBox("Visuals and Effects", MessageType.None);
            DrawEffects();
            EditorGUILayout.Space();
            DrawUI();
        }

        serializedObject.ApplyModifiedProperties();

        if (myIsCastManually.boolValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Manual Casting Values", MessageType.None);
            base.OnInspectorGUI();
        }
    }
}
#endif