using UnityEngine;

public class TeleportOverTime : MonoBehaviour
{
    public Transform[] points; // Assign in Inspector
    public float interval = 5f; // Time between teleports

    private int currentIndex = 0;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;
            currentIndex = (currentIndex + 1) % points.Length;
            transform.position = points[currentIndex].position;
        }
    }
}
