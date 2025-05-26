using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
	public UnityEvent enterEvent, exitEvent;
	public GameObject TriggerObject;

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject == TriggerObject)
			enterEvent.Invoke();
	}
	public void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject == TriggerObject)
			exitEvent.Invoke();
	}
}
