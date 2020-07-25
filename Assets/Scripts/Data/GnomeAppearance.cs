using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeAppearance : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer myMeshRenderer = null;
    [SerializeField]
    private Transform myLeftHandWeaponSocket = null;
    [SerializeField]
    private Transform myRightHandWeaponSocket = null;

    public void SetColorMaterial(Material aMaterial)
    {
        myMeshRenderer.material = aMaterial;
    }

    public void EquipItemInHand(GameObject anItem, bool aIsLeftHand)
    {
        Transform socket = aIsLeftHand ? myLeftHandWeaponSocket : myRightHandWeaponSocket;

        if(socket.childCount > 0)
            Destroy(socket.GetChild(0).gameObject);

        if(anItem)
        {
            GameObject item = Instantiate(anItem, socket);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
        }
    }
}
