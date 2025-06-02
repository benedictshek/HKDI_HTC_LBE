using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferRotation : MonoBehaviour
{
    public GameObject sourceObject; // Assign GameObject 1 in the Inspector
    public GameObject targetObject; // Assign GameObject 2 in the Inspector

    void Update()
    {
        if (sourceObject != null && targetObject != null)
        {
            targetObject.transform.rotation = sourceObject.transform.rotation;
        }
    }
}