using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    List<Message> messageList = new List<Message>();
    private int maxMessages = 10;

    public bool mouseOverUI;

    public GameObject notificationPanel;
    public GameObject notificationObject;

    private static InterfaceManager instance = null;
    public static InterfaceManager GetInstance() { return instance; }

    void Awake() {
        instance = this;    
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            AddMessageToBox("Space key pressed");
        }
    }

    public void MouseEnter() {
        mouseOverUI = true;
    }

    public void MouseExit() {
        mouseOverUI = false;
    }

    public void AddMessageToBox(string message) {
        if (messageList.Count >= maxMessages) {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message(message);
        GameObject newNotification = Instantiate(notificationObject, notificationPanel.transform);
        newMessage.textObject = newNotification.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;
        messageList.Add(newMessage);
    }
}

[System.Serializable]
public class Message {
    public string text;
    public Text textObject;
    public Message(string message) { this.text = message; }
}