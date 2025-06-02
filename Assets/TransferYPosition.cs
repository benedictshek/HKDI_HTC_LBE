using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferYPosition : MonoBehaviour
{
    public GameObject sourceObject; // Assign GameObject 1 in the Inspector
    public GameObject targetObject; // Assign GameObject 2 in the Inspector

    void Update()
    {
        if (sourceObject != null && targetObject != null)
        {
            Vector3 sourcePosition = sourceObject.transform.position;
            Vector3 targetPosition = targetObject.transform.position;

            // Set the Y position of targetObject to the Y position of sourceObject
            targetPosition.y = sourcePosition.y;
            targetObject.transform.position = targetPosition;
        }
    }
}