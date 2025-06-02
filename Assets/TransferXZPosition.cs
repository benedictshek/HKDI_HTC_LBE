using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferXZPosition : MonoBehaviour
{
    public GameObject sourceObject; // Assign GameObject 1 in the Inspector
    public GameObject targetObject; // Assign GameObject 2 in the Inspector

    void Update()
    {
        if (sourceObject != null && targetObject != null)
        {
            Vector3 sourcePosition = sourceObject.transform.position;
            Vector3 targetPosition = targetObject.transform.position;

            // Set the X position of targetObject to the X position of sourceObject
            targetPosition.x = sourcePosition.x;
            targetObject.transform.position = targetPosition;
			
			// Set the Z position of targetObject to the Z position of sourceObject
            targetPosition.z = sourcePosition.z;
            targetObject.transform.position = targetPosition;
        }
    }
}