using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [Header("The text that says press start")]
    [SerializeField]
    private Text myInstructionsText = null;
    private Vector3 myStartScale;

    private PlayerControls myKeyboardListener;
    private PlayerControls myJoystickListener;

    void Start()
    {
        Application.targetFrameRate = 300;
        myStartScale = myInstructionsText.rectTransform.localScale;

        myKeyboardListener = PlayerControls.CreateWithKeyboardBindings();
        myJoystickListener = PlayerControls.CreateWithJoystickBindings();
    }

    private void OnDestroy()
    {
        myJoystickListener.Destroy();
        myKeyboardListener.Destroy();
    }

    void Update()
    {
        float value = Mathf.Abs(Mathf.Sin(Time.time * 0.7f)); 

        const float scaleAddition = 0.08f;
        myInstructionsText.rectTransform.localScale = myStartScale * (1.0f + (scaleAddition * value));

        if(myKeyboardListener.Start.WasPressed)
        {
            SceneManager.LoadScene("Menu");
        }
        if(myJoystickListener.Start.WasPressed)
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
