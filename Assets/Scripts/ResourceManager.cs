using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public List<SceneGameObjectBase> sceneObjects = new List<SceneGameObjectBase>();
    public List<Material> sceneMaterials = new List<Material>();

    private static ResourceManager instance = null;
    public static ResourceManager GetInstance() { return instance; }

    private void Awake() {
        instance = this;
    }
        
    public SceneGameObjectBase GetObjectBase(string objectId) {
        SceneGameObjectBase result = null;
        for (int i = 0; i < sceneObjects.Count; ++i) {
            if (objectId.Equals(sceneObjects[i].object_id)){
                result = sceneObjects[i];
                break;
            }
        }
        return result;
    }

    public Material GetMaterial(int matId) {
        Material result = null;
        if (matId < sceneMaterials.Count) {
            result = sceneMaterials[matId];
        }
        return result;        
    }

    [System.Serializable]
    public class SceneGameObjectBase {
        public string object_id;
        public GameObject object_model;
    }
}
