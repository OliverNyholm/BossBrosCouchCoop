using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DebugManager : MonoBehaviour
{
    [SerializeField]
    [Range(0, 3)]
    private float myGameSpeed = 1.0f;

    private void OnValidate()
    {
        Time.timeScale = myGameSpeed;
    }

    public void KillAll()
    {
        TargetHandler targetHandler = GameManager.FindObjectOfType<TargetHandler>();
        foreach (GameObject player in targetHandler.GetAllPlayers())
        {
            player.GetComponent<Health>().TakeDamage(100000, Color.black);
        }
        foreach (GameObject enemy in targetHandler.GetAllEnemies())
        {
            enemy.GetComponent<Health>().TakeDamage(100000, Color.black);
        }
    }
}


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
