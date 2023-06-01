using UnityEngine;
public static class LayerMaskExtensions
{
    public static bool Contains(this LayerMask layerMask, int layerNumber)
    {
        if ((layerMask.value & (1 << layerNumber)) != 0)
        {
            return true;
        }

        return false;
    }
}