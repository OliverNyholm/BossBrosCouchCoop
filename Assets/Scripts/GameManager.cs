using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        AIPostMaster.Create();
    }

    private void Update()
    {
        AIPostMaster.Instance.DelegateMessages();

        if (Input.GetButtonDown("Restart"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}