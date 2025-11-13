using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class XRManager : MonoBehaviour
{
    public static Action OnXrStarted;
    public static bool XrStarted = false;
    public static bool XrPassthrough = true;

    [SerializeField]
    bool startWithPassthrough = true;

    [SerializeField]
    InputAction trackingAction;


    public static UnityEngine.XR.InputDevice GetDevice(XRNode kind)
    {
        List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        InputDevices.GetDevicesAtXRNode(kind, devices);
        return devices.Count > 0 ? devices[0] : new UnityEngine.XR.InputDevice();
    }

    private void OnEnable()
    {
        trackingAction.Enable();
    }

    IEnumerator Start()
    {
        while (!XrStarted)
        {
            yield return null;
            if (trackingAction.ReadValue<int>() > 0)
                XrStarted = true;
        }

        XrPassthrough = startWithPassthrough;
        SetPassthrough();
        yield return null;
        OnXrStarted.Invoke();


    }

    public void SwitchPassthrough()
    {
        XrPassthrough = !XrPassthrough;
        SetPassthrough();
    }

    void SetPassthrough()
    {
        ARCameraManager arCamera = GetComponentInChildren<ARCameraManager>();
        if (!arCamera)
            return;

        Camera.main.backgroundColor = new(0, 0, 0, 0);

        Camera.main.clearFlags = XrPassthrough ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
        arCamera.enabled = XrPassthrough;
        Debug.Log($"[XRManager] Set passthrough to {XrPassthrough}");

    }
}
