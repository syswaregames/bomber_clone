using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingPerspectiveController : MonoBehaviour
{
    
    public SpriteRenderer spriteRenderer;
    public Transform pivotTransform;
    public float minY = -100f;
    public float maxY = 100f;
    public int minSorting = 0;
    public int maxSorting = 32767;
    public float YOffset = 0f;

    void Update()
    {
        var relativeY = maxY - (pivotTransform.position.y+YOffset);
        var range = maxY - minY;
        var factor = relativeY / range;
        spriteRenderer.sortingOrder = minSorting + Mathf.RoundToInt((maxSorting - minSorting) * factor);
    }
}
