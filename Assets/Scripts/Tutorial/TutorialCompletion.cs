using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCompletion : MonoBehaviour
{
    [Header("Data to show on tutorial UI")]
    [SerializeField]
    [TextArea(3, 6)]
    private string myTutorialText = "No tutorial text set!";

    [SerializeField]
    private Sprite myTutorialKeySprite = null;

    [Header("Tutorial Area")]
    [SerializeField]
    private GameObject myGate = null;

    [SerializeField]
    private Collider myStartCollider = null;

    protected List<GameObject> myPlayers;
    protected TutorialPanel myTutorialPanel;
    protected TargetHandler myTargetHandler;

    private bool myHasStarted = false;

    private void Awake()
    {
        myTutorialPanel = FindObjectOfType<TutorialPanel>();
        myTargetHandler = FindObjectOfType<TargetHandler>();
    }

    private void Start()
    {
        myPlayers = new List<GameObject>(myTargetHandler.GetAllPlayers());
    }

    protected virtual bool StartTutorial()
    {
        if (myHasStarted)
            return false;

        myHasStarted = true;

        myTutorialPanel.gameObject.SetActive(true);
        myTutorialPanel.SetData(myTutorialText, myTutorialKeySprite);

        return true;
    }

    protected void EndTutorial()
    {
        myTutorialPanel.gameObject.SetActive(false);
        StartCoroutine(LowerGateRoutine());
    }

    protected void SetPlayerCompleted(GameObject aPlayer)
    {
        for (int index = 0; index < myPlayers.Count; index++)
        {
            if(aPlayer == myPlayers[index])
            {
                myTutorialPanel.SetCompletedAtIndex(index);
                break;
            }
        }
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
}
