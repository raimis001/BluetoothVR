# 📘 BluetoothVR  
Bluetooth connection between **Arduino** and **Unity** on **Meta Quest 3** VR headset.

Supports usage:  
- **with or without Passthrough**  
- **with or without Hand Tracking**

---

## ⭐ Overview
This project demonstrates how to communicate with an Arduino board from Unity running inside the Meta Quest 3 headset using Bluetooth (HC‑06 / HC‑05 / ESP32 BT).

Perfect for custom sensors, DIY controllers, environmental readings

---

## 🙏 Credits
Special thanks to **[bentalebahmed](https://github.com/bentalebahmed)** for the Unity Bluetooth plugin for Android:  
➡ https://github.com/bentalebahmed/BlueUnity

---

## 🧩 Software Stack
**Unity Version:**  
- Unity 3D **6000.2.10f1**

**Render Pipeline:**  
- URP (Universal Render Pipeline)

**Input:**  
- Unity Input System

**Packages Used:**  
- OpenXR Plugin  
- Unity OpenXR Meta  
- XR Hands  

---

## 🔌 Hardware Used
- **Arduino MEGA**  
  (any Arduino or **ESP32** can be used)
- **HC‑06** Bluetooth module  
  (HC‑05 or ESP32 Bluetooth also supported)
- **DHT22** temperature sensor (used for test data)
- **LED** for simple ON/OFF testing via Bluetooth

![Connection board](https://raw.githubusercontent.com/raimis001/BluetoothVR/refs/heads/main/bluehello/BoardBlueHello.png)

---

## 📱 Meta Quest 3 Bluetooth Setup
If you are using **HC‑06**, you *must* pair it with the Quest 3 **before** running the Unity application.

Steps:  
1. Put HC‑06 in pairing mode.  
2. Open *Settings → Bluetooth* on Quest 3.  
3. Add the device (usually named *HC‑06*).  
4. Run the Unity build — connection will work automatically.
