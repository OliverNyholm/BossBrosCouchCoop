using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHudHandler : MonoBehaviour
{
    [Header("The player to be spawned if level started without character select")]
    public GameObject myBossHudPrefab = null;

    public Dictionary<int, GameObject> myBossHuds = new Dictionary<int, GameObject>();

    public void AddBossHud(GameObject aBoss)
    {
        GameObject bossHud = Instantiate(myBossHudPrefab, transform, false);

        myBossHuds.Add(aBoss.GetInstanceID(), bossHud);

        UIComponent npc = aBoss.GetComponent<UIComponent>();
        if (npc)
            npc.SetupHud(bossHud.transform);

        if (myBossHuds.Count == 1)
        {
            return;
        }

        RecalculateHudPositions();
    }

    public void RemoveBossHud(GameObject aBoss)
    {
        Destroy(myBossHuds[aBoss.GetInstanceID()]);
        if (myBossHuds.Remove(aBoss.GetInstanceID()))
            RecalculateHudPositions();
    }

    void RecalculateHudPositions()
    {
        int count = myBossHuds.Count;
        int index = 0;
        foreach (KeyValuePair<int, GameObject> bossHud in myBossHuds)
        {
            RectTransform rectTransform = bossHud.Value.GetComponent<RectTransform>();
            Vector2 anchorSize = new Vector2(rectTransform.anchorMax.x - rectTransform.anchorMin.x, rectTransform.anchorMax.y - rectTransform.anchorMin.y);

            if (count == 1)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1.0f - (rectTransform.anchorMax.y - rectTransform.anchorMin.y));
                rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
            }
            if (count == 2)
            {
                SetBossPositionTopCorners(rectTransform, anchorSize, index);
            }
            if (count == 3 && index < 2)
            {
                SetBossPositionTopCorners(rectTransform, anchorSize, index);
            }

            index++;
        }
    }

    private void SetBossPositionTopCorners(RectTransform rectTransform, Vector2 anAnchorSize, int aBossIndex)
    {
        if (aBossIndex == 0)
        {
            rectTransform.anchorMin = new Vector2(0.05f, 1.0f - anAnchorSize.y);
            rectTransform.anchorMax = new Vector2(anAnchorSize.x, 1.0f);
        }
        else
        {
            rectTransform.anchorMin = new Vector2(1.0f - anAnchorSize.x, 1.0f - anAnchorSize.y);
            rectTransform.anchorMax = new Vector2(1.0f - 0.05f, 1.0f);
        }

        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;
    }
}
