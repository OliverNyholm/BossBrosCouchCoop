using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHudHandler : MonoBehaviour
{
    [Header("The player to be spawned if level started without character select")]
    public GameObject myBossHudPrefab = null;

    public void HandoutBossHud(GameObject aBoss, int aTotalBossCount, int aBossIndex)
    {
        GameObject bossHud = Instantiate(myBossHudPrefab, transform, false);

        Enemy enemy = aBoss.GetComponent<Enemy>();
        if(enemy)
            enemy.SetBossHud(bossHud);
        else
        {
            FriendlyTargetDummy friendlyDummy = aBoss.GetComponent<FriendlyTargetDummy>();
            if (friendlyDummy)
                friendlyDummy.SetBossHud(bossHud);
        }

        if(aTotalBossCount == 1)
        {
            return;
        }

        RectTransform rectTransform = bossHud.GetComponent<RectTransform>();
        Vector2 anchorSize = new Vector2(rectTransform.anchorMax.x - rectTransform.anchorMin.x, rectTransform.anchorMax.y - rectTransform.anchorMin.y);
        if(aTotalBossCount == 2)
        {
            SetBossPositionTopCorners(rectTransform, anchorSize, aBossIndex);
        }
        if(aTotalBossCount == 3 && aBossIndex < 2)
        {
            SetBossPositionTopCorners(rectTransform, anchorSize, aBossIndex);
        }
    }

    private void SetBossPositionTopCorners(RectTransform rectTransform, Vector2 anAnchorSize, int aBossIndex)
    {
        if(aBossIndex == 0)
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
