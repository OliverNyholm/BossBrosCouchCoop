using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DebugManager : MonoBehaviour
{
    [SerializeField]
    [Range(0, 3)]
    private float myGameSpeed = 1.0f;

    PlayerControls myPlayerControls;

    private void Awake()
    {
        myPlayerControls = PlayerControls.CreateWithKeyboardBindings();
    }

    public void Update()
    {
        if (myPlayerControls.NumpadOne.WasPressed)
            SetGameSpeed(1.0f);
        if (myPlayerControls.NumpadTwo.WasPressed)
            SetGameSpeed(0.0f);
        if (myPlayerControls.NumpadThree.WasPressed)
            SetGameSpeed(5.0f);
    }

    public void SetGameSpeed(float aGameSpeed)
    {
        myGameSpeed = aGameSpeed;
        Time.timeScale = myGameSpeed;
    }

    private void OnValidate()
    {
        Time.timeScale = myGameSpeed;
    }
    public void KillAll()
    {
        TargetHandler targetHandler = FindObjectOfType<TargetHandler>();
        foreach (GameObject player in targetHandler.GetAllPlayers())
        {
            player.GetComponent<Health>().TakeDamage(100000, Color.black, player.transform.position);
        }
        foreach (GameObject enemy in targetHandler.GetAllEnemies())
        {
            enemy.GetComponent<Health>().TakeDamage(100000, Color.black, enemy.transform.position);
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(DebugManager))]
public class DebugManagerEditor : Editor
{
    DebugManager myDebugManager;

    void Awake()
    {
        myDebugManager = target as DebugManager;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Kill All"))
        {
            myDebugManager.KillAll();
        }
    }
}
#endif