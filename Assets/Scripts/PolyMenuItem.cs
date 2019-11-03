using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolyMenuItem : MonoBehaviour
{
    [SerializeField]
    private Image thumbnail;

    public void SetThumbnail(Image thumbnail) {
        this.thumbnail = thumbnail;
    }
}
