using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Message
{
    public string myText;
    public Text myTextObject;
}

public class Chat : MonoBehaviour {

    private List<Message> myMessages = new List<Message>();
    public int myMaxMessageCount;

    public GameObject myChatPanel;
    public GameObject myTextObject;
    public InputField myInputField;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (!myInputField.isFocused)
            return;

        if(myInputField.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            myInputField.text.Remove(myInputField.text.Length - 1);
            SendMessageToChat(myInputField.text);
            myInputField.text = "";
        }
	}

    public void SendMessageToChat(string aText)
    {
        if (myMessages.Count >= myMaxMessageCount)
        {
            Destroy(myMessages[0].myTextObject.gameObject);
            myMessages.Remove(myMessages[0]);
        }

        Message message = new Message();
        message.myText = aText;

        GameObject text = Instantiate(myTextObject, myChatPanel.transform); ;
        message.myTextObject = text.GetComponent<Text>();
        message.myTextObject.text = message.myText;

        myMessages.Add(message);
    }
}
