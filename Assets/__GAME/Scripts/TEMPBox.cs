using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TEMPBox : MonoBehaviour
{
    public BluetoothManager bluetooth;

    public GameObject closer;
    public GameObject led;
    public TMP_Text beepText;

    public TMP_Text cTempText;
    public TMP_Text fTempText;
    public TMP_Text kTempText;

    public TMP_Text hummText;

    private void OnEnable()
    {
        BluetoothManager.OnConnectionStatusChanged += OnConnectionStatusChanged;
        BluetoothManager.OnDataRead += OnDataRead;
    }

    private void OnDataRead(string obj)
    {
        string[] s = obj.Split(":");
        if (s.Length == 0)
            return;

        if (s[0] == "TM")
        {
            beepText.text = beepText.text == "" ? "." : "";
            if (float.TryParse(s[1], out float t))
            {
                cTempText.text = t.ToString("0.00");
                fTempText.text = Farenheit(t);
                kTempText.text = Kelvin(t);
            }
            if (float.TryParse(s[2], out float h))
            {
                hummText.text = h.ToString("0.00") + "%";
            }
        }
    }

    private void OnConnectionStatusChanged(bool obj)
    {
        if (obj)
            StartCoroutine(OpenDisplay());
        else
        {
            //Material m = led.GetComponent<Renderer>().material;
            //m.DisableKeyword("_EMISSION");
            StartCoroutine(CloseDisplay());
        }
    }

    public void OnLedOn(TouchControll controll)
    {
        if (!bluetooth.IsConnected)
            return;

        Material m = led.GetComponent<Renderer>().material;
        m.EnableKeyword("_EMISSION");
        bluetooth.WriteData("LEDON");
    }
    public void OnLedOff(TouchControll controll)
    {
        if (!bluetooth.IsConnected) 
           return;

        Material m = led.GetComponent<Renderer>().material;
        m.DisableKeyword("_EMISSION");
        bluetooth.WriteData("LEDOFF");
    }

    string Farenheit(float celsius)
    {
        float f = celsius * (9f / 5f) + 32f; 

        return f.ToString("0.0");
    }

    string Kelvin(float celsius) 
    {
        float k = celsius + 273.15f;
        return k.ToString("0.0");
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
