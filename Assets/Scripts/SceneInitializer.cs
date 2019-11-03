using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitializer : MonoBehaviour {
    SceneManager manager;
    TerrainGrid terrainGrid;
    InterfaceManager ui;

    public GameObject ObjectsContainer;
    public Material availableTile;
    public Material unavailableTile;

    public Texture2D MouseSelect;
    public Texture2D MousePlace;
    public Texture2D MouseRotate;
    public Texture2D MouseResize;
    public Texture2D MouseDelete;
    public Texture2D MouseRandom;
    public CursorMode cursorMode = CursorMode.Auto;

    //Actual GameObject to be placed    
    GameObject objToPlace;
    //brush object which follows the mouse
    GameObject brushObject;
    bool hasObj;

    SceneObject objProperties;
    Vector3 mousePosition;
    Vector3 worldPosition;
    Node currentNode;

    private string selectedTool;

    private static SceneInitializer instance = null;
    public static SceneInitializer GetInstance() { return instance; }

    private void Awake() {
        instance = this;
    }

    void Start() {
        instance = this;
        manager = SceneManager.GetInstance();
        terrainGrid = TerrainGrid.GetInstance();
        ui = InterfaceManager.GetInstance();
    }

    void Update() {
        //Keep position in grid updated
        UpdateMousePosition();
        currentNode = terrainGrid.NodeFromWorldPosition(mousePosition);
        worldPosition = currentNode.GetPosition();

        //Update pointed brush position
        if (brushObject != null) {
            brushObject.transform.position = worldPosition + new Vector3(0f, brushObject.transform.lossyScale.y / 2, 0f);
        }

        //Actions
        ExecuteAction();
    }

    void UpdateMousePosition() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            mousePosition = hit.point;
        }
    }

    #region Brush Selection

    public void SetTool(string tool) {
        this.selectedTool = tool;
    }

    public void ExecuteAction() {
        switch (selectedTool) {
            case "select":
                SetCursor(MouseSelect);
                SelectObject();
                break;
            case "place":
                SetCursor(MousePlace);
                PlaceObject();
                break;
            case "delete":
                SetCursor(MouseDelete);
                DeleteObject();
                break;
            case "rotate":
                SetCursor(MouseRotate);
                RotateObject();
                break;
            case "resize":
                SetCursor(MouseResize);
                ResizeObject();
                break;
            case "line":
                SetCursor(MouseSelect);
                break;
            case "geometric":
                SetCursor(MouseSelect);
                break;
            case "random":
                SetCursor(MouseRandom);
                RandomBrush(3, 5);
                break;
            case "other":
                break;
        }
    }

    private void SetCursor(Texture2D cursor) {
        Cursor.SetCursor(cursor, Vector2.zero, cursorMode);
    }

    public void SetObjectToBePlaced(GameObject selectedObject) {
        ClearBrush();
        ui.AddMessageToBox("- Selected Object: " + selectedObject.name);
        hasObj = true;
        brushObject = selectedObject;
        objToPlace = selectedObject;
        SetTool("place");
    }
    #endregion

    #region Place Object in Scene
    void PlaceObject() {
        //If an object is selected
        if (Input.GetMouseButtonDown(0) && !ui.mouseOverUI || ((Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift)) && !ui.mouseOverUI)) {
            if (hasObj) {
                //If the brush was not initialized
                if (brushObject == null) {
                    brushObject = Instantiate(objToPlace, worldPosition, Quaternion.identity) as GameObject;
                    //objProperties = brushObject.GetComponent<SceneObject>();
                } else {

                    if (currentNode.placedObject != null) {
                        manager.inSceneObjets.Remove(currentNode.placedObject);
                        Destroy(currentNode.placedObject.objectModel);
                        currentNode.placedObject = null;
                    }

                    // Hack to compensate for the Google Poly asset's centered pivot.
                    worldPosition.y += objToPlace.transform.lossyScale.y / 2;

                    GameObject newObject = Instantiate(objToPlace, worldPosition, brushObject.transform.rotation, ObjectsContainer.transform) as GameObject;
                    SceneObject newSceneObject = new SceneObject(newObject.name, currentNode.GetPosition(), brushObject.transform.rotation, newObject);

                    manager.inSceneObjets.Add(newSceneObject);
                    //currentNode.tileRenderer.material = unavailableTile;
                    currentNode.placedObject = newSceneObject;
                    ui.AddMessageToBox("Object placed");
                }
            } else {
                if (brushObject != null) {
                    Destroy(brushObject);
                }
                ui.AddMessageToBox("No object selected!");
            }
        }
    }
    #endregion

    private void SelectObject() {
        if (Input.GetMouseButtonDown(0) && !ui.mouseOverUI) {
            ClearBrush();
            if (currentNode.placedObject != null) {
                ui.AddMessageToBox(currentNode.placedObject.objectModel.name);
                SetObjectToBePlaced(currentNode.placedObject.objectModel);
                Destroy(currentNode.placedObject.objectModel);
                currentNode.placedObject = null;
                //currentNode.tileRenderer.material = availableTile;
                SetTool("place");
            }
        }
    }

    #region Delete Object from Scene
    private void DeleteObject() {
        ClearBrush();
        if (Input.GetMouseButton(0) && !ui.mouseOverUI) {
            if (currentNode.placedObject != null) {
                manager.inSceneObjets.Remove(currentNode.placedObject);
                Destroy(currentNode.placedObject.objectModel);
                currentNode.placedObject = null;
                //currentNode.tileRenderer.material = availableTile;
            }
        }
    }
    #endregion

    private void RotateObject() {
        ClearBrush();
        //Rotate in natural direction (1)
        if (Input.GetMouseButtonDown(0) && !ui.mouseOverUI) {
            if (currentNode.placedObject != null) {
                currentNode.placedObject.Rotate(1);
            }
        }
        //Rotate in inverse direction (-1)
        if (Input.GetMouseButtonDown(1) && !ui.mouseOverUI) {
            if (currentNode.placedObject != null) {
                currentNode.placedObject.Rotate(-1);
            }
        }
    }

    private void ResizeObject() {
        ClearBrush();
        //Rotate in natural direction (1)
        if (Input.GetMouseButtonDown(0) && !ui.mouseOverUI) {
            if (currentNode.placedObject != null) {
                currentNode.placedObject.Resize(1);
            }
        }
        if (Input.GetMouseButtonDown(1) && !ui.mouseOverUI) {
            if (currentNode.placedObject != null) {
                currentNode.placedObject.Resize(-1);
            }
        }
    }
    
    private void RandomBrush(int amount, int radius) {
        if (hasObj) {
            //If the brush was not initialized
            if (brushObject == null) {
                brushObject = Instantiate(objToPlace, worldPosition, Quaternion.identity) as GameObject;                
            } else {
                if (Input.GetMouseButton(0) && !ui.mouseOverUI) {
                    List<Node> NodesToAdd = new List<Node>();

                    for (int i = 0; i < amount; ++i) {

                        Vector3 randompos = new Vector3(Random.insideUnitCircle.x * radius, 0, Random.insideUnitCircle.y * radius);
                        Node randomNode = terrainGrid.NodeFromWorldPosition(mousePosition + randompos);
                        if (randomNode.placedObject == null) {
                            NodesToAdd.Add(randomNode);
                        }
                    }
                    GroupPlace(NodesToAdd);
                }
            }
        }else {
            ui.AddMessageToBox("No object selected!");
        }
    }

    private void GroupPlace(List<Node> nodes) {
        foreach(Node node in nodes) {
            Vector3 position = node.GetPosition();
            position.y += objToPlace.transform.lossyScale.y / 2;

            GameObject newObject = Instantiate(objToPlace, position, brushObject.transform.rotation, ObjectsContainer.transform) as GameObject;
            SceneObject newSceneObject = new SceneObject(newObject.name, position, brushObject.transform.rotation, newObject);
            manager.inSceneObjets.Add(newSceneObject);
            
            //Add a random rotation to make objects seem more natural
            newSceneObject.Rotate(Random.Range(0,4));

            node.placedObject = newSceneObject;
            //node.tileRenderer.material = unavailableTile;            
        }        
    }

    void ClearBrush() {
        if (brushObject != null) {
            Destroy(brushObject);
        }
        if (objToPlace != null) {
            Destroy(objToPlace);
        }
        hasObj = false;
    }

}
