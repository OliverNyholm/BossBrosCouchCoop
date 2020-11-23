using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class ExtensionMethods
{
    public static Transform FindInChildren(this Transform self, string name)
    {
        int count = self.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = self.GetChild(i);
            if (child.name == name) return child;
            Transform subChild = child.FindInChildren(name);
            if (subChild != null) return subChild;
        }
        return null;
    }

    public static GameObject FindInChildren(this GameObject self, string name)
    {
        Transform transform = self.transform;
        Transform child = transform.FindInChildren(name);
        return child != null ? child.gameObject : null;
    }
}

/// <summary>
/// Small helper class to convert viewport, screen or world positions to canvas space.
/// Only works with screen space canvases.
/// Usage:
/// objectOnCanvasRectTransform.anchoredPosition = specificCanvas.WorldToCanvasPoint(worldspaceTransform.position);
/// </summary>
public static class CanvasPositioningExtensions
{
    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        var viewportPosition = camera.WorldToViewportPoint(worldPosition);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                                           screenPosition.y / Screen.height,
                                           0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }
}

public static class UtilityFunctions
{
    public static bool HasSpellType(SpellType anAttacktype, SpellType aHasType)
    {
        return (anAttacktype & aHasType) != 0;
    }

    public static bool HasSpellType(SpellOverTimeType anAttacktype, SpellOverTimeType aHasType)
    {
        return (anAttacktype & aHasType) != 0;
    }

    public static bool HasSpellTarget(SpellTarget aSpellTarget, SpellTarget aTargetType)
    {
        return (aSpellTarget & aTargetType) != 0;
    }

    public static bool FindGroundFromLocation(Vector3 aStartLocation, out Vector3 outHitLocation, out MovablePlatform outMovablePlatform)
    {
        float distance = 5.0f;
        Ray ray = new Ray(aStartLocation + Vector3.up, Vector3.down);
        LayerMask layerMask = LayerMask.GetMask("Terrain");

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, layerMask))
        {
            outHitLocation = hitInfo.point;
            outMovablePlatform = hitInfo.collider.gameObject.GetComponent<MovablePlatform>();
            return true; 
        }

        outHitLocation = Vector3.zero;
        outMovablePlatform = null;
        return false;
    }

    public static void InvertTextZRotation(TextMeshProUGUI aText)
    {
        Vector3 invertRotation = aText.transform.eulerAngles;
        invertRotation.z *= -1.0f;
        aText.transform.eulerAngles = invertRotation;
    }
}
