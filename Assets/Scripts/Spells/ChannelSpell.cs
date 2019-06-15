using UnityEngine;

public class ChannelSpell : MonoBehaviour {
    
    [SerializeField]
    protected Sprite mySpellIcon;

    protected float myTimerBeforeDestroy = -1.0f;

    public virtual void SetToDestroy()
    {
        myTimerBeforeDestroy = 0.0f;
    }

    public void Update()
    {
        if(myTimerBeforeDestroy >= 0.0f)
        {
            myTimerBeforeDestroy -= Time.deltaTime;
            if (myTimerBeforeDestroy <= 0.0f)
                Destroy(gameObject);
        }
    }
}
