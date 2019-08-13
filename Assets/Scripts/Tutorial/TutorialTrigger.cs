using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private TutorialCompletion myParent = null;
    private Collider myCollider = null;

    private void Awake()
    {
        myParent = GetComponentInParent<TutorialCompletion>();
        myCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider aOther)
    {
        myParent.OnChildTriggerEnter(myCollider, aOther);
    }

    private void OnTriggerExit(Collider aOther)
    {
        myParent.OnChildTriggerExit(myCollider, aOther);
    }
}
