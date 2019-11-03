using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int nodePosX;
    public float nodePosY;
    public int nodePosZ;
    public GameObject vis;
    public MeshRenderer tileRenderer;
    public bool isWalkable;
    public SceneObject placedObject;
    public List<SceneObject> stackedObjects = new List<SceneObject>();
    public Vector3 GetPosition() { return new Vector3(nodePosX, nodePosY, nodePosZ); }
}
