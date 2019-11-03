using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject
{
    public string objectName;
    public int gridPosX;
    public int gridPosY;
    public int gridPosZ;
    public GameObject objectModel;
    public Vector3 worldPositionOffset;
    public Quaternion worldRotation;

    public bool isStackable = false;
    public bool isObstructing = false;

    public float rotateDegrees = 45;

    public SceneObject() {

    }

    public SceneObject(string name, Vector3 position, Quaternion rotation, GameObject model) {
        this.objectName = name;
        this.objectModel = model;
        this.worldRotation = rotation;
        SetPosition(position);
    }

    public void UpdateNode(Node[,] grid) {
        Node node = grid[gridPosX, gridPosZ];
        Vector3 worldPosition = node.vis.transform.position;
        worldPosition += worldPositionOffset;
        objectModel.transform.rotation = worldRotation;
        objectModel.transform.position = worldPosition;
    }
    public void Rotate(int direction) {
        Vector3 eulerAngles = objectModel.transform.eulerAngles;
        eulerAngles += new Vector3(0, direction * rotateDegrees, 0);
        objectModel.transform.localRotation = Quaternion.Euler(eulerAngles);
    }

    public void Resize(int scale) {
        objectModel.transform.localScale += new Vector3(scale, scale, scale);
        Vector3 position = objectModel.transform.position;
        objectModel.transform.position = new Vector3(position.x, this.gridPosY + objectModel.transform.localScale.y/2, position.z);
    }

    public void SetPosition(Vector3 position) {
        this.gridPosX = (int)(position.x);
        this.gridPosY = (int)(position.y);
        this.gridPosZ = (int)(position.z);
    }

    public void Move(Vector3 position) {
        Debug.Log("moving");
        objectModel.transform.position = position;
    }
}
