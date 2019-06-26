using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorMessageManager : MonoBehaviour
{

    [SerializeField]
    private GameObject myErrorMessageBox = null;

    [SerializeField]
    private GameObject myErrorMessagePrefab = null;

    private List<GameObject> myErrorMessages;

    private void Start()
    {
        myErrorMessages = new List<GameObject>();
    }

    void Update()
    {
        for (int index = 0; index < myErrorMessages.Count; index++)
        {
            if (myErrorMessages[index].GetComponent<ErrorMessage>().ShouldRemove())
            {
                Destroy(myErrorMessages[index]);
                myErrorMessages.RemoveAt(index);
            }
        }
    }

    public void CreateErrorMessage(string aMessage)
    {
        GameObject errorMessage = Instantiate(myErrorMessagePrefab, myErrorMessageBox.transform);

        errorMessage.GetComponent<ErrorMessage>().SetText(aMessage);
        myErrorMessages.Add(errorMessage);

        if (myErrorMessages.Count > 3)
        {
            Destroy(myErrorMessages[0]);
            myErrorMessages.RemoveAt(0);
        }
    }
}
