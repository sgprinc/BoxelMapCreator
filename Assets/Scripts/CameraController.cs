using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    protected Transform cameraTransform;
    protected Transform targetTransform;

    protected GameObject dummyTarget;
    protected GameObject target;

    protected float distance = 15f;

    public float MouseSensitivity;
    public float ScrollSensitvity;
    public float PanSpeed;

    private float currentX;
    private float currentY;

    private Rect screen = new Rect(0, 0, Screen.width, Screen.height);


    // Use this for initialization
    void Start() {
        this.dummyTarget = new GameObject("Default Target");
        this.dummyTarget.transform.rotation = Quaternion.identity;
        this.dummyTarget.transform.position = Vector3.zero;
        this.dummyTarget.transform.parent = this.cameraTransform; 
        this.target = this.dummyTarget;
        this.targetTransform = this.target.transform;

        this.cameraTransform = Camera.main.transform;
        this.currentX = cameraTransform.position.x;
        this.currentY = cameraTransform.position.y;
    }

    // Reference: http://wiki.unity3d.com/index.php/MouseOrbitZoom
    void LateUpdate() {
        if (screen.Contains(Input.mousePosition)) {
            if (!InterfaceManager.GetInstance().mouseOverUI) {
                if (!target) {
                    target = dummyTarget;
                }

                //Rotation of the Camera based on Mouse Coordinates
                if (Input.GetMouseButton(1)) {
                    currentX += Input.GetAxis("Mouse X") * MouseSensitivity;
                    currentY -= Input.GetAxis("Mouse Y") * MouseSensitivity;
                    currentY = Mathf.Clamp(currentY, 0f, 90f);
                }

                //Panning
                if (Input.GetMouseButton(2)) {
                    targetTransform.rotation = transform.rotation;
                    targetTransform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * PanSpeed);
                    targetTransform.Translate(transform.up * -Input.GetAxis("Mouse Y") * PanSpeed, Space.World);
                }

                // Zooming
                if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
                    distance -= Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity * 3f;
                    distance = Mathf.Clamp(distance, 0f, 100f);
                }

                Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
                Vector3 negDistance = new Vector3(0f, 0f, -distance);
                Vector3 position = rotation * negDistance + targetTransform.position;

                cameraTransform.rotation = rotation;
                cameraTransform.position = position;                
            }
        }
    }

    public void UpdateTarget(GameObject newTarget) {
        this.target = newTarget;
        this.targetTransform = target.transform;
    }
}