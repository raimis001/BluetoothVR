using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class XRHand : MonoBehaviour
{
    public static XRHand Left;
    public static XRHand Right;

    public XRNode kind;

    [SerializeField]
    InputActionReference triggerRefence;
    [SerializeField]
    InputActionReference joystickReference;

    public Vector2 joystick => joystickReference.action.ReadValue<Vector2>();

    public bool triggerDown => triggerRefence.action.triggered;
    public bool triggerHold => triggerRefence.action.IsPressed();

    private void OnEnable()
    {
        triggerRefence?.action.Enable();
        joystickReference?.action.Enable();
    }
    private void OnDisable()
    {
        triggerRefence?.action.Disable();
        joystickReference?.action.Disable();
    }

    private void Update()
    {

            
    }

    private void Awake()
    {
        if (kind == XRNode.LeftHand)
            Left = this;
        else if (kind == XRNode.RightHand)
            Right = this;
    }
}
