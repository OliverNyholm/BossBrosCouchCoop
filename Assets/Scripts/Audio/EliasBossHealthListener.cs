using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliasBossHealthListener : MonoBehaviour
{
    [SerializeField]
    private Health myHealth = null;

    [System.Serializable]
    public struct EliasBossHealthListenerEvent
    {
        [Range(0, 1.0f)]
        public float myHealthTriggerPercentage;
        public EliasSetLevel mySetLevel;
    }

    [Header("Health Level Events")]
    [SerializeField]
    private List<EliasBossHealthListenerEvent> myHealthEvents = new List<EliasBossHealthListenerEvent>();

    [Header("On Death Event")]
    [SerializeField]
    private string myOnDeathActionPresetName = string.Empty;
    [SerializeField]
    private bool myOnDeathAllowRequiredThemeMissmatch = false;

    private EliasPlayer myEliasPlayer;

    private void Awake()
    {
        myEliasPlayer = FindObjectOfType<EliasPlayer>();
        myHealth.EventOnHealthChange += OnHealthChanged;
        myHealth.EventOnHealthZero += OnDeath;
    }

    private void OnHealthChanged(float aHealthPercentage, string aHealthText, int aShieldValue, bool aIsDamage)
    {
        for (int index = 0; index < myHealthEvents.Count; index++)
        {            
            EliasBossHealthListenerEvent listenerEvent = myHealthEvents[index];
            if (aHealthPercentage <= listenerEvent.myHealthTriggerPercentage)
            {
                Debug.Log("Triggering health event for percentage[" + listenerEvent.myHealthTriggerPercentage.ToString("0.00f") + "]. Setting level [" + listenerEvent.mySetLevel.level.ToString() + "].");
                myEliasPlayer.QueueEvent(listenerEvent.mySetLevel.CreateSetLevelEvent(myEliasPlayer.Elias));
                myHealthEvents.RemoveAt(index);
                return;
            }
        }
    }

    private void OnDeath()
    {
        Debug.Log("Triggering death event[" + myOnDeathActionPresetName + "].");
        myEliasPlayer.RunActionPreset(myOnDeathActionPresetName, myOnDeathAllowRequiredThemeMissmatch);
    }
}
