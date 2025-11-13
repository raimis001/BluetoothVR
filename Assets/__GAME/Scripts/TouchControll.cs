using UnityEngine;
using UnityEngine.Events;

public class TouchControll : MonoBehaviour
{
    public UnityEvent<TouchControll> OnTouchStart;
    public UnityEvent<TouchControll> OnTouchEnd;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TouchControll: OnTriggerEnter with " + other.name);
        OnTouchStart?.Invoke(this);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TouchControll: OnTriggerExit with " + other.name);
        OnTouchEnd?.Invoke(this);
    }

}
