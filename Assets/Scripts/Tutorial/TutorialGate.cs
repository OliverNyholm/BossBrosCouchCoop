using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGate : MonoBehaviour
{
    [SerializeField]
    private float myRotationDuration = 2.0f;
    [SerializeField]
    private float myRotationAngle = 90.0f;

    public void Open()
    {
        //GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(OpenGateEnumerator());
    }

    private IEnumerator OpenGateEnumerator()
    {
        float duration = 0.0f;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0.0f, myRotationAngle, 0.0f));

        while(duration < myRotationDuration)
        {
            duration += Time.deltaTime;
            float interval = duration / myRotationDuration;

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, interval);
            yield return null;
        }
    }
}
