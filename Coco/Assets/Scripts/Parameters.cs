using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters : MonoBehaviour {

    static Parameters instance;
    public static Parameters Instance { get { return instance; } }

    // Define public parameters for controlling prefab 
    // spawn range, spawn time, etc.
    [System.Serializable]
    public struct YSpawnRange
    {
        public float minY;
        public float maxY;
    }

    [System.Serializable]
    public struct XSpawnRange
    {
        public float minX;
        public float maxX;
    }
    public YSpawnRange ySpawnRange;
    public XSpawnRange xSpawnRange;
}
