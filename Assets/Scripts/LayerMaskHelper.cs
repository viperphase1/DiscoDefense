using UnityEngine;

public static class LayerMaskHelper
{
    // Method to get combined layer mask from an array of layer names
    public static int GetLayerMask(params string[] layerNames)
    {
        int mask = 0;
        foreach (string layerName in layerNames)
        {
            mask |= 1 << LayerMask.NameToLayer(layerName);
        }
        return mask;
    }
}