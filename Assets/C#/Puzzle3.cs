using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Puzzle3 : MonoBehaviour
{
    public GameObject[] Start_EndPoint;
    public GameObject[] PositionPoint;
    public GameObject[] Buttons;
    public Canvas canvas;

    private RectTransform draggingButtonRectTransform;
    private Vector2 offset;

    public GameObject HitPage;

    // Time limit to detect a double-click (in seconds)
    private float doubleClickTimeLimit = 0.5f;

    // Dictionary to store the last click time for each button GameObject
    private Dictionary<GameObject, float> buttonLastClickTime = new Dictionary<GameObject, float>();

    // List to hold child objects named '1' and '2', and Start_EndPoint objects
    private List<GameObject> allObjectsList = new List<GameObject>();

    // Dictionary to track if each object is "connected" (true or false)
    private Dictionary<GameObject, bool> objectConnectionStatus = new Dictionary<GameObject, bool>();

    private bool allConnected = false;  // To track if all objects are connected

    void Start()
    {
        // Add click listeners to each button GameObject and initialize drag events
        foreach (GameObject btnObj in Buttons)
        {
            // Add event triggers for drag and click functionality
            AddEventTriggers(btnObj);
        }

        // Populate the list with named child objects and Start_EndPoint objects
        CollectObjects();
    }

    void Update()
    {
        // Update object positions every frame
        CheckObjectPositions();

        // Check if all objects are connected based on the updated positions
        CheckIfAllConnected();

        // Output the result based on the connection status
        if (allConnected)
        {
            Debug.Log("全连接了");
        }
        else
        {
            Debug.Log("没有全连接");
        }
    }
    // Method to add EventTrigger components and assign drag-related events
    void AddEventTriggers(GameObject buttonObj)
    {
        EventTrigger trigger = buttonObj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = buttonObj.AddComponent<EventTrigger>();
        }

        // Pointer Down (Press Button)
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data, buttonObj); });
        trigger.triggers.Add(pointerDownEntry);

        // Dragging
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        trigger.triggers.Add(dragEntry);

        // Pointer Up (Release Button)
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data, buttonObj); });
        trigger.triggers.Add(pointerUpEntry);
    }

    // Pointer down method to start the dragging
    public void OnPointerDown(PointerEventData eventData, GameObject buttonObj)
    {
        draggingButtonRectTransform = buttonObj.GetComponent<RectTransform>();

        // Calculate the offset from the mouse position to the center of the button
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
        offset = (Vector2)draggingButtonRectTransform.localPosition - localPoint;
    }

    // Drag method to update the position of the button
    public void OnDrag(PointerEventData eventData)
    {
        if (draggingButtonRectTransform != null)
        {
            // Convert the screen position to local canvas position and update button position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out Vector2 localPoint);
            draggingButtonRectTransform.localPosition = localPoint + offset;
        }
    }

    // Pointer up method to stop the dragging, check for snapping, and handle double-click
    public void OnPointerUp(PointerEventData eventData, GameObject buttonObj)
    {
        draggingButtonRectTransform = null;  // Clear the reference to the button's RectTransform

        // Check for double-click
        OnButtonClick(buttonObj);

        // Check if the button is close enough to any PositionPoint for snapping
        CheckForSnapping(buttonObj);
    }

    // Method to check the distance between the dragged button and all PositionPoints
    void CheckForSnapping(GameObject buttonObj)
    {
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

        foreach (GameObject point in PositionPoint)
        {
            RectTransform pointRect = point.GetComponent<RectTransform>();

            // Calculate the distance between the button and the position point
            float distance = Vector2.Distance(buttonRect.anchoredPosition, pointRect.anchoredPosition);

            // If the distance is less than 5, snap the button to the position point
            if (distance < 40f)
            {
                buttonRect.anchoredPosition = pointRect.anchoredPosition;
                break; // Stop checking after the first snap
            }
        }
    }

    // Method called when a button is clicked (checks for double-click)
    public void OnButtonClick(GameObject buttonObj)
    {
        float timeSinceLastClick = 0f;

        if (buttonLastClickTime.ContainsKey(buttonObj))
        {
            timeSinceLastClick = Time.time - buttonLastClickTime[buttonObj];
        }
        else
        {
            timeSinceLastClick = Mathf.Infinity;
        }

        if (timeSinceLastClick <= doubleClickTimeLimit)
        {
            // Double-click detected
            RotateButton(buttonObj);
        }

        // Update the last click time
        buttonLastClickTime[buttonObj] = Time.time;
    }

    // Method to rotate the button GameObject by 90 degrees
    void RotateButton(GameObject buttonObj)
    {
        buttonObj.transform.Rotate(0f, 0f, 90f);
    }

    // Method to collect child objects named '1' and '2', and Start_EndPoint objects
    void CollectObjects()
    {
        // Loop through each button and find its child objects named '1' and '2'
        foreach (GameObject button in Buttons)
        {
            Transform[] allChildren = button.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == "1" || child.name == "2")
                {
                    allObjectsList.Add(child.gameObject);
                    objectConnectionStatus[child.gameObject] = false;  // Initialize with false
                }
            }
        }

        // Add all Start_EndPoint objects to the list
        foreach (GameObject endPoint in Start_EndPoint)
        {
            allObjectsList.Add(endPoint);
            objectConnectionStatus[endPoint] = false;  // Initialize with false
        }

        // Check the positions of objects in the list
        CheckObjectPositions();
    }

    // Method to check the distance between each object in the list and others
    void CheckObjectPositions()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        for (int i = 0; i < allObjectsList.Count; i++)
        {
            GameObject currentObject = allObjectsList[i];
            RectTransform currentRect = currentObject.GetComponent<RectTransform>();
            bool isCloseToObject = false;

            for (int j = 0; j < allObjectsList.Count; j++)
            {
                if (i == j) continue;  // Skip itself

                GameObject otherObject = allObjectsList[j];
                RectTransform otherRect = otherObject.GetComponent<RectTransform>();

                // Convert currentRect and otherRect to local points relative to the canvas
                Vector2 currentLocalPoint, otherLocalPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, currentRect.position), canvas.worldCamera, out currentLocalPoint);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, otherRect.position), canvas.worldCamera, out otherLocalPoint);

                // Calculate the distance between the two local points
                float distance = Vector2.Distance(currentLocalPoint, otherLocalPoint);

                // Check if the distance is within the threshold (20f)
                if (distance < 10f)
                {
                    isCloseToObject = true;
                    break;
                }
            }

            // Mark the object as true if it's close to another object, false otherwise
            objectConnectionStatus[currentObject] = isCloseToObject;
        }
    }


    // Method to check if all objects in the list are connected
    void CheckIfAllConnected()
    {
        allConnected = true;

        // Iterate through each button and check if its child objects '1' and '2' are connected
        foreach (GameObject button in Buttons)
        {
            bool buttonConnected = true; // Assume the button's children are connected

            // Get child objects '1' and '2'
            Transform[] allChildren = button.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == "1" || child.name == "2")
                {
                    // If any child object '1' or '2' is not connected, mark the button as not connected
                    if (!objectConnectionStatus[child.gameObject])
                    {
                        buttonConnected = false;
                        Debug.Log(button.name + " 的 " + child.name + " 没有连接 (false)");
                    }
                }
            }

            // If any child of the button is not connected, mark the whole check as false
            if (!buttonConnected)
            {
                allConnected = false;
            }
        }

        // Output the result based on the connection status
        if (allConnected)
        {
            Debug.Log("全连接了");
        }
        else
        {
            Debug.Log("没有全连接");
        }

        
    }
    public void playmusic()
    {





    }

    public void OpenPage()
    {

        HitPage.SetActive(true);



    }
    public void ClosePage()
    {

        HitPage.SetActive(false);



    }
}
