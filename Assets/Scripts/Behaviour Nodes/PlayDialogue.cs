using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class PlayDialogue : Action
{
    public DialogueData myDialogue = null;

    public override void OnAwake()
    {
        base.OnAwake();
    }

    public override TaskStatus OnUpdate()
    {
        if (myDialogue.myAudioEvent != "")
            AkSoundEngine.PostEvent(myDialogue.myAudioEvent, gameObject);

        DialogueCanvas.Instance.AddDialogue(myDialogue, GetComponent<UIComponent>().myCharacterColor);

        return TaskStatus.Success;
    }
}
