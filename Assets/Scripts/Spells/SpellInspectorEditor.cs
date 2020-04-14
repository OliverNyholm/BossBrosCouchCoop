using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Spell), true), CanEditMultipleObjects]
public class SpellInspectorEditor : Editor
{
    public SerializedProperty
        myCastTime,
        myCooldown,
        myResourceCost,
        mySpeed,
        myRange,

        myIsCastableWhileMoving,
        myCanCastOnSelf,
        myIsOnlySelfCast,

        myThreatModifier,

        mySpellType,
        mySpellTarget,
        myDamage,
        myHealValue,
        myStunDuration,

        myAnimationType,
        mySpellSFX,
        mySpellVFX,
        myShouldRotate,

        myCastbarColor,
        mySpellIcon,
        myName,
        myQuickInfo,
        myTutorialInfo;

    protected bool myShouldShowIcon;

    public virtual void OnEnable()
    {
        // Setup the SerializedProperties
        myCastTime = serializedObject.FindProperty("myCastTime");
        myCooldown = serializedObject.FindProperty("myCooldown");
        myResourceCost = serializedObject.FindProperty("myResourceCost");
        mySpeed = serializedObject.FindProperty("mySpeed");
        myRange = serializedObject.FindProperty("myRange");

        myIsCastableWhileMoving = serializedObject.FindProperty("myIsCastableWhileMoving");
        myCanCastOnSelf = serializedObject.FindProperty("myCanCastOnSelf");
        myIsOnlySelfCast = serializedObject.FindProperty("myIsOnlySelfCast");

        myThreatModifier = serializedObject.FindProperty("myThreatModifier");

        mySpellType = serializedObject.FindProperty("myAttackType");
        mySpellTarget = serializedObject.FindProperty("mySpellTarget");
        myDamage = serializedObject.FindProperty("myDamage");
        myHealValue = serializedObject.FindProperty("myHealValue");
        myStunDuration = serializedObject.FindProperty("myStunDuration");

        myAnimationType = serializedObject.FindProperty("myAnimationType");
        mySpellSFX = serializedObject.FindProperty("mySpellSFX");
        mySpellVFX = serializedObject.FindProperty("mySpellVFX");
        myShouldRotate = serializedObject.FindProperty("myShouldRotate");

        myCastbarColor = serializedObject.FindProperty("myCastbarColor");
        mySpellIcon = serializedObject.FindProperty("mySpellIcon");
        myName = serializedObject.FindProperty("myName");
        myQuickInfo = serializedObject.FindProperty("myQuickInfo");
        myTutorialInfo = serializedObject.FindProperty("myTutorialInfo");

        myShouldShowIcon = false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Spell Base", MessageType.None);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("mySpawnedOnHit"));
        EditorGUILayout.PropertyField(mySpellTarget);
        EditorGUILayout.PropertyField(mySpellType);
        AttackType spellType = (AttackType)mySpellType.intValue;

        // ------------------------------------------------------------------
        EditorGUILayout.HelpBox("Attack Type Attributes", MessageType.None);

        if (UtilityFunctions.HasSpellType(spellType, AttackType.Damage))
            EditorGUILayout.PropertyField(myDamage, new GUIContent("Damage Amount"));
        if (UtilityFunctions.HasSpellType(spellType, AttackType.Heal))
            EditorGUILayout.PropertyField(myHealValue, new GUIContent("Heal Amount"));

        if (UtilityFunctions.HasSpellType(spellType, AttackType.Stun))
            EditorGUILayout.PropertyField(myStunDuration, new GUIContent("Stun Duration"));
        // ------------------------------------------------------------------

        EditorGUILayout.Space();

        // ------------------------------------------------------------------
        EditorGUILayout.HelpBox("Castabilities", MessageType.None);

        EditorGUILayout.PropertyField(myCastTime, new GUIContent("Cast Time"));
        EditorGUILayout.PropertyField(myCooldown, new GUIContent("Cooldown"));
        EditorGUILayout.PropertyField(myResourceCost, new GUIContent("Resource Cost"));
        EditorGUILayout.PropertyField(myRange, new GUIContent("Range"));
        EditorGUILayout.PropertyField(mySpeed, new GUIContent("Speed"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(myCanCastOnSelf, new GUIContent("Can Cast On Self"));
        EditorGUILayout.PropertyField(myIsOnlySelfCast, new GUIContent("Is Only Self Cast"));
        if (myCastTime.floatValue > 0.0f)
            EditorGUILayout.PropertyField(myIsCastableWhileMoving, new GUIContent("Can Move While Casting"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(myThreatModifier, new GUIContent("Threat Modifier"));
        // ------------------------------------------------------------------

        EditorGUILayout.Space();

        // ------------------------------------------------------------------
        EditorGUILayout.HelpBox("Visuals and Effects", MessageType.None);
        EditorGUILayout.PropertyField(myAnimationType, new GUIContent("Animation Type"));
        DrawEffects();
        EditorGUILayout.PropertyField(myShouldRotate, new GUIContent("Should Spell Rotate"));
        // ------------------------------------------------------------------

        EditorGUILayout.Space();

        // ------------------------------------------------------------------
        DrawUI();
        // ------------------------------------------------------------------

        serializedObject.ApplyModifiedProperties();
    }

    protected void DrawUI()
    {
        EditorGUILayout.HelpBox("UI", MessageType.None);
        EditorGUILayout.PropertyField(myName, new GUIContent("Spell Name"));
        if (myCastTime.floatValue > 0.0f)
        {
            EditorGUILayout.PropertyField(myCastbarColor, new GUIContent("Castbar Color"));
            EditorGUILayout.PropertyField(mySpellIcon, new GUIContent("Castbar Spell Icon"));
        }
        else
        {
            if(myShouldShowIcon)
                EditorGUILayout.PropertyField(mySpellIcon, new GUIContent("Castbar Spell Icon"));
        }
        EditorGUILayout.PropertyField(myQuickInfo, new GUIContent("One-Liner Spell Info"));
        EditorGUILayout.PropertyField(myTutorialInfo, new GUIContent("Tutorial Spell Info"));
    }

    protected void DrawEffects()
    {
        if (myCastTime.floatValue > 0.0f)
            EditorGUILayout.PropertyField(mySpellSFX.FindPropertyRelative("myCastSound"), new GUIContent("Sound While Casting"));
        if (mySpeed.floatValue > 0.0f)
            EditorGUILayout.PropertyField(mySpellSFX.FindPropertyRelative("mySpawnSound"), new GUIContent("Sound When Spell Spawns"));
        EditorGUILayout.PropertyField(mySpellSFX.FindPropertyRelative("myHitSound"), new GUIContent("Sound When Spell Hits Target"));

        EditorGUILayout.PropertyField(mySpellVFX, new GUIContent("On Hit VFX"));
    }
}