using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct GlobalSFX
    {
        public string myName;
        public AudioClip mySoundEffect;
    }

    [Header("Global sound effects to play")]
    [SerializeField]
    private List<GlobalSFX> mySoundBank = new List<GlobalSFX>();

    private Dictionary<string, AudioClip> myAudioClips;

    private static AudioManager ourAudioManager;

    private AudioSource ourAudioSource;

    void Start()
    {
        DontDestroyOnLoad(this);
        if (ourAudioManager)
            Destroy(gameObject);
        else
            ourAudioManager = this;

        myAudioClips = new Dictionary<string, AudioClip>();
        for (int index = 0; index < mySoundBank.Count; index++)
        {
            myAudioClips.Add(mySoundBank[index].myName, mySoundBank[index].mySoundEffect);
        }

        ourAudioSource = GetComponent<AudioSource>();
    }

    public static AudioManager Instance
    {
        get
        {
            return ourAudioManager;
        }
    }

    public void PlaySoundEffect(string aName)
    {
        if(!myAudioClips.ContainsKey(aName))
        {
            Debug.Log("AudioManager does not have soundEffect called: " + aName);
            return;
        }

        ourAudioSource.PlayOneShot(myAudioClips[aName]);
    }
}
