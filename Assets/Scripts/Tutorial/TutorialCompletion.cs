using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCompletion : MonoBehaviour
{
    [Header("Tutorial Area")]
    [SerializeField]
    private GameObject myGate = null;

    [SerializeField]
    protected Collider myStartCollider = null;

    public List<GameObject> Players { get; set; } = new List<GameObject>();
    protected List<GameObject> myCompletedPlayers = new List<GameObject>();
    protected TutorialPanel myTutorialPanel;
    protected TargetHandler myTargetHandler;

    private bool myHasStarted = false;

    protected delegate IEnumerator FinishRoutine();
    protected FinishRoutine myFinishRoutine;

    private void Awake()
    {
        myTutorialPanel = FindObjectOfType<TutorialPanel>();
        myTargetHandler = FindObjectOfType<TargetHandler>();
    }

    private void Start()
    {
        if(myGate)
            myFinishRoutine = LowerGateRoutine;
    }

    protected virtual bool StartTutorial()
    {
        if (myHasStarted)
            return false;

        Players = new List<GameObject>(myTargetHandler.GetAllPlayers());
        myHasStarted = true;

        //myTutorialPanel.gameObject.SetActive(true);
        //myTutorialPanel.SetData(myTutorialText, myTutorialImageSprite, myTutorialKeySprite);

        return true;
    }

    protected void EndTutorial()
    {
        //myTutorialPanel.gameObject.SetActive(false);
        StartCoroutine(myFinishRoutine());
    }

    IEnumerator LowerGateRoutine()
    {
        Vector3 targetOffset = new Vector3(0.0f, -20.0f, 0.0f);
        Vector3 startPosition = myGate.transform.position;
        Vector3 endPosition = startPosition + targetOffset;
        float duration = 4.0f;
        float timer = duration;

        while (timer > 0.0f)
        {
            timer -= Time.deltaTime;

            float interpolation = 1.0f - (timer / duration);
            myGate.transform.position = Vector3.Lerp(startPosition, endPosition, interpolation);

            yield return null;
        }
    }
    
    public virtual void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        if (aChildCollider == myStartCollider)
            StartTutorial();
    }

    public virtual void OnChildTriggerExit(Collider aChildCollider, Collider aHit)
    {
    }

    public void Restart()
    {
        myHasStarted = false;
    }
}
