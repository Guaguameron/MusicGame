using UnityEngine;

[System.Serializable]
public class BackgroundLayer
{
    public GameObject backgroundObject;
    public GameObject duplicateObject;
    public float scrollSpeed;
    public float resetPosition = -10f;
    public float startPosition = 10f;
}