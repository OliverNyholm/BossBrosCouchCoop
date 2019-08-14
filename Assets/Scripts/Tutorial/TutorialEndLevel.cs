using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEndLevel : TutorialCompletion
{
    public override void OnChildTriggerEnter(Collider aChildCollider, Collider aHit)
    {
        FindObjectOfType<LevelProgression>().UnlockLevels();
        SceneManager.LoadScene("LevelSelect");
    }
}
