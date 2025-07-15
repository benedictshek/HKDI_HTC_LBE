using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        transform.LookAt(cameraTransform.position);
    }
}