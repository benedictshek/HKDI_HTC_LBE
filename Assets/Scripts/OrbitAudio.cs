using UnityEngine;

public class OrbitAudio : MonoBehaviour
{
    public Transform center;
    public float radius = 2f;
    public float speed = 10f; // degrees per second

    public float angle = 0f;

    void Update()
    {
        angle -= speed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * radius;
        transform.position = center.position + offset;
    }
}
