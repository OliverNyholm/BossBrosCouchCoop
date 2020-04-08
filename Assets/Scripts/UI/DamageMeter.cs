using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DamageMeter : MonoBehaviour
{
    class DamagerData
    {
        public Image myDamageBar;
        public Color myColor;
        public int myDamage;

        public DamagerData(Transform aTransform, Color aColor, int aDamage)
        {
            myDamageBar = aTransform.GetComponent<Image>();
            myDamageBar.fillAmount = 0.0f;
            myDamageBar.color = aColor;

            myColor = aColor;
            myDamage = aDamage;
        }
    }

    private Dictionary<int, DamagerData> myDamagers;
    private Subscriber mySubscriber;
    private int myTotalDamage;

    void Awake()
    {
        Subscribe();
        myDamagers = new Dictionary<int, DamagerData>();
    }

    void Update()
    {
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    void Subscribe()
    {
        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveAIMessage;
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.DamageDealt);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.RegisterPlayer);
    }

    void Unsubscribe()
    {
        mySubscriber.EventOnReceivedMessage -= ReceiveAIMessage;
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.DamageDealt);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.RegisterPlayer);
    }

    private void ReceiveAIMessage(Message aMessage)
    {
        switch (aMessage.Type)
        {
            case MessageCategory.DamageDealt:
                {
                    int id = (int)aMessage.Data.myVector2.x;
                    int value = (int)aMessage.Data.myVector2.y;

                    myTotalDamage += value;
                    myDamagers[id].myDamage += value;

                    SortDamageMeter();
                }
                break;
            case MessageCategory.RegisterPlayer:
                {
                    int id = aMessage.Data.myInt;
                    Vector3 rgb = aMessage.Data.myVector3;
                    Color color = new Color(rgb.x, rgb.y, rgb.z);
                    myDamagers.Add(id, new DamagerData(transform.GetChild(myDamagers.Count), color, 0));
                }
                break;
            case MessageCategory.UnregisterPlayer:
                {
                    int id = aMessage.Data.myInt;
                    myDamagers.Remove(id);
                }
                break;
            default:
                break;
        }
    }

    private void SortDamageMeter()
    {
        int index = 0;
        foreach (KeyValuePair<int, DamagerData> damager in myDamagers.OrderByDescending(key => key.Value.myDamage))
        {
            float percentage = (float)damager.Value.myDamage / myTotalDamage;
            Image damageBar = transform.GetChild(index).GetComponent<Image>();
            damageBar.fillAmount = percentage;
            damageBar.color = damager.Value.myColor;

            transform.GetChild(index).GetComponentInChildren<Text>().text = ((float)damager.Value.myDamage / 1000).ToString("0.0");
            index++;
        }
    }
}
