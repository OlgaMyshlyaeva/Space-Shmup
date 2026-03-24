using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// General utility functions for the Space SHMUP project.
/// </summary>
public static class Utils
{
    //=================== Materials Management ===================\\

    /// <summary>
    /// Returns an array of all materials found on the provided GameObject and its children.
    /// Useful for global effects like damage flashing.
    /// </summary>
    /// <param name="go">The target GameObject to scan.</param>
    /// <returns>An array of Material components.</returns>
    static public Material[] GetAllMaterials(GameObject go)
    {
        // Get all Renderer components in the target object and its children
        Renderer[] rends = go.GetComponentsInChildren<Renderer>();

        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends)
        {
            // Note: Accessing .material creates a unique copy for the instance
            mats.Add(rend.material);
        }
        
        return mats.ToArray();
    }
}

