using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

public class TutorialHealWithCombat : TutorialCompletion
{
    [Header("The Totem")]
    [SerializeField]
    private GameObject myTotemBoss = null;
    [SerializeField]
    private List<ParticleSystem> myBurningEyes = new List<ParticleSystem>();

    [SerializeField]
    private GameObject myHealSpell = null;

    [SerializeField]
    private Collider myStartFightCollider = null;

    [SerializeField]
    private GameObject myHighlightBossTimerObject = null;

    private Subscriber mySubscriber;

    protected override void Awake()
    {
        base.Awake();

        Spell spell = myHealSpell.GetComponent<Spell>();
        spell.CreatePooledObjects(PoolManager.Instance, 6);
    }

    protected override bool StartTutorial()
    {
        if (!base.StartTutorial())
            return false;

        foreach (GameObject player in Players)
            player.GetComponent<Class>().ReplaceSpell(myHealSpell, 0);

        return true;
    }

    private void StartFight()
    {
        myTotemBoss.GetComponent<BehaviorTree>().enabled = true;
        myTotemBoss.GetComponent<BehaviorTree>().SendEvent("Activate");
        foreach (ParticleSystem burningEyes in myBurningEyes)
        {
            burningEyes.Clear();
            burningEyes.Play();
        }

        myStartFightCollider.enabled = false;

        mySubscriber = new Subscriber();
        mySubscriber.EventOnReceivedMessage += ReceiveMessage;

        StartCoroutine(HighlightBossTimer());

        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.TutorialHealFightComplete);
        PostMaster.Instance.RegisterSubscriber(ref mySubscriber, MessageCategory.Wipe);
    }

    private void ReceiveMessage(Message aMessage)
    {
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.TutorialHealFightComplete);
        PostMaster.Instance.UnregisterSubscriber(ref mySubscriber, MessageCategory.Wipe);
        mySubscriber.EventOnReceivedMessage -= ReceiveMessage;

        myTotemBoss.GetComponent<BehaviorTree>().enabled = true;
        myTotemBoss.GetComponent<BehaviorTree>().SendEvent("Deactivate");

        foreach (ParticleSystem burningEyes in myBurningEyes)
            burningEyes.Stop();

        if (aMessage.Type == MessageCategory.TutorialHealFightComplete)
        {
            EndTutorial();
            PostMaster.Instance.PostMessage(new Message(MessageCategory.RessurectDead));
        }
        else if(aMessage.Type == MessageCategory.Wipe)
        {
            Restart();
            myStartFightCollider.enabled = true;
        }
    }

    public override void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        if (aChildCollider == myStartCollider)
            StartTutorial();

        if (aChildCollider == myStartFightCollider)
            StartFight();
    }

    private IEnumerator HighlightBossTimer()
    {
        float waitDuration = 1.0f;
        while (waitDuration > 0.0f)
        {
            waitDuration -= Time.deltaTime;
            yield return null;
        }

        Vector3 inversePosition = myHighlightBossTimerObject.GetComponentInParent<Canvas>().transform.InverseTransformPoint(myHighlightBossTimerObject.transform.position);        
        FindObjectOfType<TutorialHighlightManager>().HighlightArea(inversePosition, Vector3.one);
    }
}
