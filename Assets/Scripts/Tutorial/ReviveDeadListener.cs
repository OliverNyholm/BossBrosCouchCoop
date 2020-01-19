using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveDeadListener : MonoBehaviour
{
    [Header("Toggle true if revive on first death. If disabled, will revive when everyone has died")]
    [SerializeField]
    private bool myShouldReviveInstantly = true;

    [SerializeField]
    private float myTimeBeforeInstantRevive = 1.5f;

    [SerializeField]
    private List<Transform> myReviveSpots = new List<Transform>();

    private Dictionary<int, GameObject> myPlayers = new Dictionary<int, GameObject>();
    private List<int> myDeadPlayerIds = new List<int>();
    private Subscriber mySubscriber;


    private void Awake()
    {
        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += OnMessageReceived;
    }

    private void OnDisable()
    {
        mySubscriber.EventOnReceivedMessage -= OnMessageReceived;
    }

    private void OnMessageReceived(Message aMessage)
    {
        if(myShouldReviveInstantly)
        {
            myPlayers.TryGetValue(aMessage.Data.myInt, out GameObject player);
            if (!player)
                return;

            StartCoroutine(InstantReviveCoroutine(player, 0));
            return;
        }

        myDeadPlayerIds.Add(aMessage.Data.myInt);
        if(myDeadPlayerIds.Count == myPlayers.Count)
        {
            int count = 0;
            foreach (KeyValuePair<int, GameObject> player in myPlayers)
            {
                StartCoroutine(InstantReviveCoroutine(player.Value, count));
                count++;
            }
            myDeadPlayerIds.Clear();
        }
    }

    private void RevivePlayer(GameObject aPlayer, int aReviveCount)
    {
        aPlayer.GetComponent<Health>().ReviveToFullHealth();
        aPlayer.GetComponent<Player>().OnRevive();

        if (aReviveCount >= myReviveSpots.Count)
            aReviveCount = 0;

        aPlayer.transform.position = myReviveSpots[aReviveCount].position + Vector3.up * 2f;
    }

    public void ReviveAllDeadPlayers()
    {
        StopAllCoroutines();

        int count = 0;
        foreach (int playerID in myDeadPlayerIds)
        {
            StartCoroutine(InstantReviveCoroutine(myPlayers[playerID], count));
            count++;
        }
        myDeadPlayerIds.Clear();
    }

    private IEnumerator InstantReviveCoroutine(GameObject aPlayer, int anIndex)
    {
        float timer = myTimeBeforeInstantRevive;

        while(timer > 0.0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        RevivePlayer(aPlayer, anIndex);
    }

    public void ListenToDeaths()
    {
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
    }

    public void StopListening()
    {
        myPlayers.Clear();
        myDeadPlayerIds.Clear();
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageType.PlayerDied);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (myPlayers.Count == 0)
                ListenToDeaths();

            myPlayers.Add(other.gameObject.GetInstanceID(), other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<Health>().IsDead())
                return;

            myPlayers.Remove(other.gameObject.GetInstanceID());

            if (myPlayers.Count == 0)
                StopListening();
        }
    }
}
