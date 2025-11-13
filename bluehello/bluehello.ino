#define DHT

#include "TimeUtils.h"

#ifdef DHT
#include "Dht22Scheduler.h"
#else
#include <AM2302-Sensor.h>
#endif

#define TEST_PIN 13
#define DHT_PIN 7
// Mega <-> HC-06 Serial1 (TX1=18 -> HC06 RX; RX1=19 <- HC06 TX)

const unsigned long SEND_INTERVAL_MS = 3000; // ~20 Hz
static unsigned long t0 = 0;
#ifdef DHT
Dht22Scheduler dht(DHT_PIN);
#else
AM2302::AM2302_Sensor am2302{DHT_PIN};
#endif



void setup() {
  pinMode(TEST_PIN, INPUT);
  digitalWrite(TEST_PIN, LOW);

  Serial.begin(115200);     // USB debug
  Serial1.begin(9600);      // HC-06 9600 baud

#ifdef DHT
  dht.begin();
#else
 if (am2302.begin()) {
       // this delay is needed to receive valid data,
       // when the loop directly read again
       delay(3000);
    }
    else {
       while (true) {
       Serial.println("Error: sensor check. => Please check sensor connection!");
       delay(10000);
       }
    }
#endif

  Serial.println("BLE demo start");
}

void loop() {
  
#ifdef DHT
  dht.update();
#endif

  if (Serial1.available()) {
    String cmd = Serial1.readStringUntil('\n');
    cmd.trim();
    Serial.print("BT cmd: ");
    Serial.println(cmd);
    // piemēram: ja saņem "PING", atbildi
    if (cmd.equalsIgnoreCase("PING")) {
      Serial1.println("PONG");
    }
    if (cmd.equalsIgnoreCase("LEDON"))
      digitalWrite(TEST_PIN, HIGH);
    if (cmd.equalsIgnoreCase("LEDOFF"))
      digitalWrite(TEST_PIN, LOW);
  }

  if (canSend())
  {
    //sendTime();
    sendTemperature();
  }
  delay(100);

}

bool canSend() {
  unsigned long now = millis();
  if (now - t0 >= SEND_INTERVAL_MS) 
  {
    t0 = now;
    return true;
  }
  return false;
}

void sendTime() {
  
  String tm = formatTimeParts(millis());

  Serial1.print(tm);
  Serial1.print('\n');

  //Debug
  Serial.print("TM: ");
  Serial.println(tm);
}

void sendTemperature() {

#ifdef DHT
   float temperature = dht.data.temperature;
   float humidity = dht.data.humidity;
   int8_t status = dht.data.error;
   String tm = formatTimeParts(dht.data.lastUpdate);
   Serial.println(tm);
#else
    int8_t status = am2302.read();
    float temperature = am2302.get_Temperature();
    float humidity = am2302.get_Humidity();
#endif

  String msg = "TM:" + String(temperature, 1) + ":" + String(humidity, 1) + ":" + String(dht.data.lastValidUpdate);

  Serial1.print(msg);
  Serial1.print('\n');

  //Debug
  msg = "T:" + String(temperature, 1) + "C H:" + String(humidity, 1) + "% S:" + String(status);
  Serial.print("TM: ");
  Serial.println(msg);
}
