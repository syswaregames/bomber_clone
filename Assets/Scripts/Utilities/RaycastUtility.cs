using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public static class Physics2DExtended
{


    /// <summary>
    /// Just made simple raycast but draw line for debug
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
    {
        var hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        Debug.DrawRay(origin, direction * distance, hit.collider is null ? Color.red : Color.blue);
        if(hit.collider is not null)
        Debug.DrawRay(hit.point, hit.normal*0.3f, Color.magenta);
        return hit;
    }
}