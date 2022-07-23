
#define WIFI_SSID "paes"
#define WIFI_PASS "garfield laranja"
const char* mqttserver = "192.168.0.164";
const int mqttserverport = 1883;
const char* mqttuser = "robertocpaes";
const char* mqttpass = "teste";
const char* MQTTQUEUEINPUT = "topic/ventilador/arduino_in";
const char* MQTTQUEUEOUTPUT = "topic/ventilador/arduino_out";
long interval = 5000;
IPAddress ip(192,168,0,155);
IPAddress gateway(192,168,0,1);
IPAddress subnet(255,255,255,0);
IPAddress dns(192,168,0,1);
