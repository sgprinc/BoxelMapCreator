using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    TerrainGrid terrainGrid;

    public List<SceneObject> inSceneObjets = new List<SceneObject>();

    private static SceneManager instance = null;
    public static SceneManager GetInstance() { return instance; }

    void Awake() {
        instance = this;
    }

    void Start()
    {
        terrainGrid = TerrainGrid.GetInstance();
    }
    
}
