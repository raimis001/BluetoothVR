using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BLEBox : MonoBehaviour
{
    public BluetoothManager bluetooth;
    public GameObject closer;
    public TMP_Text debugText;
    public TMP_Text nameText;
    public TMP_Text connectionText;
    public TMP_Text devicesText;

    string macAddress = "none";
    string bleName = "HC-06";

    private void OnEnable()
    {
        BluetoothManager.OnConnectionStatusChanged += OnConnectionStatusChanged;
        BluetoothManager.OnNewDevice += OnNewDevice;
    }

    private void OnDisable()
    {
        BluetoothManager.OnConnectionStatusChanged -= OnConnectionStatusChanged;
        BluetoothManager.OnNewDevice -= OnNewDevice;
    }

    private void OnNewDevice(string obj)
    {
        devicesText.text += obj + "\n";
        if (obj.Contains("HC-06"))
        {
            macAddress = obj.Split('+')[0];
            bleName = obj.Split('+')[1];
            nameText.text = bleName;
            connectionText.text = "Connecting...";
            OnConnectionClick(null);
        }

    }

    private void OnConnectionStatusChanged(bool obj)
    {
        Log("Status Changed: " + obj);
        connectionText.text = obj ? "Connected" : "Disconnected";
        if (obj)
            StartCoroutine(OpenDisplay());
        else
            StartCoroutine(CloseDisplay());
    }

    public void OnListClick(TouchControll controll)
    {
        if (bluetooth.IsConnected)
            return;

        if (macAddress == "none")
        {
            devicesText.text = "";
            nameText.text = "";
            connectionText.text = "Scanning...";
            Log("Scan devices");
            bluetooth.GetPairedDevices();
        }
        else
        {
            Log("Connect to: " + macAddress);
            connectionText.text = "Connecting...";
            bluetooth.StartConnection(macAddress);
        }
    }

    public void OnConnectionClick(TouchControll controll)
    {
        if (bluetooth.IsConnected)
            return;

        if (macAddress == "none")
        {
            Log("No device found");
            return;
        }

        Log("Connect to: " + macAddress);
        connectionText.text = "Connecting...";
        bluetooth.StartConnection(macAddress);
    }

    public void OnDisconnectionClick(TouchControll controll)
    {
        StartCoroutine(CloseDisplay());
        bluetooth.StopConnection();
    }

    void Log(string message)
    {
        if (!debugText)
            return;

        debugText.text = message + "\n" + debugText.text;
    }

    IEnumerator OpenDisplay()
    {
        float time = 0.75f;
        float t = 0;
        Material m = closer.GetComponent<Renderer>().material;
        if (m.color.a == 0)
            yield break;
        while (t < time)
        {
            t += Time.deltaTime;
            m.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, t / time));
            yield return null;
        }
        m.color = new Color(0, 0, 0, 0);
    }
    IEnumerator CloseDisplay()
    {
        float time = 0.75f;
        float t = 0;
        Material m = closer.GetComponent<Renderer>().material;
        if (m.color.a == 1)
            yield break;
        while (t < time)
        {
            t += Time.deltaTime;
            m.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, t / time));
            yield return null;
        }
        m.color = new Color(0, 0, 0, 1);
    }
}
