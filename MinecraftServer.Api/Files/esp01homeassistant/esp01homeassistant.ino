#include <PubSubClient.h>
#include <ESP8266WiFi.h>
#include <ArduinoJson.h>
#include "config.h"

long lastMessage = 0;
WiFiClient wifiClient;
PubSubClient client(wifiClient);

void callback(char* topic, byte* payload, unsigned int length) {
  for (int i=0;i<length;i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
}

void reconnect() {
   client.setServer(mqttserver, mqttserverport);
   client.setCallback(callback);
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    if (client.connect("esp8266",mqttuser,mqttpass)) {
      Serial.println("connected");
      client.subscribe(MQTTQUEUEINPUT);
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}


void setup() {
  Serial.begin(9600);
  Serial.println();
  Serial.print("MAC: ");
  Serial.println(WiFi.macAddress());
  WiFi.config(ip, dns, gateway, subnet);
  WiFi.begin(WIFI_SSID, WIFI_PASS);
  delay(5000);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected! IP address: ");
  Serial.println(WiFi.localIP());

  if (!client.connected()) {
    reconnect();
  }
}

void loop() {

  if (!client.connected()) {
    reconnect();
  }
  client.loop();
  if (Serial.available() > 0) 
   {
      long now = millis();
      if (now - lastMessage > interval)
      {
        lastMessage = now;
        StaticJsonDocument<256> doc;
        JsonObject root = doc.to<JsonObject>();
        DeserializationError err = deserializeJson(doc, Serial);
        if(err) 
         {
          Serial.flush();
          return;
         }
         char telemetryAsJson[256];
         serializeJson(doc, telemetryAsJson);
         client.publish(MQTTQUEUEOUTPUT, telemetryAsJson);
      }
   }
 
}
