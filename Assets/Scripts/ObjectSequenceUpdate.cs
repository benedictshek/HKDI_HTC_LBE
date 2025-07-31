using UnityEngine;
using System.Collections;

public class ObjectSequencerUpdate : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;

    public float delay = 30f;

    void Start()
    {
        // Start the sequence when the game begins
        StartCoroutine(SequenceObjects());
    }

    IEnumerator SequenceObjects()
    {
        // Step 1: Show only object1
        object1.SetActive(true);
        object2.SetActive(false);
        object3.SetActive(false);
        yield return new WaitForSeconds(delay);

        // Step 2: Show object2, hide object1
        object1.SetActive(false);
        object2.SetActive(true);
        object3.SetActive(false);
        yield return new WaitForSeconds(delay);

        // Step 3: Show object3, hide object2
        object1.SetActive(false);
        object2.SetActive(false);
        object3.SetActive(true);
    }
}
