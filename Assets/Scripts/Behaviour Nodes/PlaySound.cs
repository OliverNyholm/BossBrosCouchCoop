using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class PlaySound : Action
{
    public AudioClip myAudioClip;

    public override TaskStatus OnUpdate()
    {
        GetComponent<AudioSource>().PlayOneShot(myAudioClip);

        return TaskStatus.Success;
    }
}
