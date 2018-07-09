
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <Wire.h>
#include <WiFiUDP.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BNO055.h>
#include <utility/imumaths.h>

int ch1 =0;
int ch2 =1;
int ch3 =2;

const char* ssid =/*"iPhone";*/ "dlink";
const char* password =/* "19940625";*/"468255000";
String PoseX, PoseY, PoseZ, PoseX2, PoseY2, PoseZ2, PoseX3, PoseY3, PoseZ3;
 /* Set the delay between fresh samples */
#define BNO055_SAMPLERATE_DELAY_MS (100)

Adafruit_BNO055 bno = Adafruit_BNO055(55);
Adafruit_BNO055 bno2 = Adafruit_BNO055(55);
Adafruit_BNO055 bno3 = Adafruit_BNO055(55);

WiFiServer server(27);
WiFiUDP Client;

/**************************************************************************/
/*
    Displays some basic information on this sensor from the unified
    sensor API sensor_t type (see Adafruit_Sensor for more information)
*/
/**************************************************************************/
void displaySensorDetails(void)
{
  sensor_t sensor;
  bno.getSensor(&sensor);
  Serial.println("------------------------------------");
  Serial.print  ("Sensor:       "); Serial.println(sensor.name);
  Serial.print  ("Driver Ver:   "); Serial.println(sensor.version);
  Serial.print  ("Unique ID:    "); Serial.println(sensor.sensor_id);
  Serial.print  ("Max Value:    "); Serial.print(sensor.max_value); Serial.println(" xxx");
  Serial.print  ("Min Value:    "); Serial.print(sensor.min_value); Serial.println(" xxx");
  Serial.print  ("Resolution:   "); Serial.print(sensor.resolution); Serial.println(" xxx"); 
  Serial.println("------------------------------------");
  Serial.println("");
  delay(500);
}
 
void setup() {
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);//WIFI_STA : Client 模式
  Serial.println("Orientation Sensor Test"); Serial.println("");
  Wire.begin();
  WiFi.begin(ssid,password);
  Serial.println("Connecting");
 
  while(WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
 
  Serial.print("Connected to "); 
  Serial.println(ssid);
  
  Serial.print("IP Address: "); 
  Serial.println(WiFi.localIP());
 
  // Start the UDP client
  Client.begin(27);

   /* Initialise the sensor */
    enableMuxPort(ch1); //Tell mux to connect to port X
  if(!bno.begin())
  {
    /* There was a problem detecting the BNO055 ... check your connections */
    Serial.print("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");
    while(1);
  }
    disableMuxPort(ch1);
    
  //Initialize the sensor2
    enableMuxPort(ch2); //Tell mux to connect to port X
  if(!bno2.begin())
  {
    /* There was a problem detecting the BNO055 ... check your connections */
    Serial.print("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");
    while(1);
  }
    disableMuxPort(ch2);

     //Initialize the sensors3
  //  enableMuxPort(ch3); //Tell mux to connect to port X
//  if(!bno3.begin())
 // {
    /* There was a problem detecting the BNO055 ... check your connections */
 //   Serial.print("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");
 //   while(1);
 // }
  //  disableMuxPort(ch3);
    
    /* Use external crystal for better accuracy */
  bno.setExtCrystalUse(true);
   
  /* Display some basic information on this sensor */
  displaySensorDetails();
  
}
 
void loop() {
   // Possible vector values can be:
  // - VECTOR_ACCELEROMETER - m/s^2
  // - VECTOR_MAGNETOMETER  - uT
  // - VECTOR_GYROSCOPE     - rad/s
  // - VECTOR_EULER         - degrees
  // - VECTOR_LINEARACCEL   - m/s^2
  // - VECTOR_GRAVITY       - m/s^2
  // Listen for connecting clients


/* Get a new sensor event */
  enableMuxPort(ch1); //Tell mux to connect to this port, and this port only
  sensors_event_t event;
  bno.getEvent(&event);
  imu::Vector<3> euler = bno.getVector(Adafruit_BNO055::VECTOR_EULER);
  /* Board layout:
         +----------+
         |         *| RST   PITCH  ROLL  HEADING
     ADR |*        *| SCL
     INT |*        *| SDA     ^            /->
     PS1 |*        *| GND     |            |
     PS0 |*        *| 3VO     Y    Z-->    \-X
         |         *| VIN
         +----------+
  */

  /* The processing sketch expects data as roll, pitch, heading */

  Serial.print((int)euler.x());Serial.print(";");

  Serial.print((int)euler.y());Serial.print(";");

  Serial.print((int)euler.z());Serial.print("\t");

  PoseX = (int)euler.x();
    PoseY = (int)euler.y();
      PoseZ = (int)euler.z();

  /* Also send calibration data for each sensor. */
  uint8_t sys, gyro, accel, mag = 3;
  bno.getCalibration(&sys, &gyro, &accel, &mag);

  disableMuxPort(ch1); //Tell mux to disconnect from this port
//
   enableMuxPort(ch2); //Tell mux to connect to this port, and this port only
   bno2.getEvent(&event);
   imu::Vector<3> euler2 = bno2.getVector(Adafruit_BNO055::VECTOR_EULER);
   /* The processing sketch expects data as roll, pitch, heading */

  Serial.print((int)euler2.x());Serial.print(";");

  Serial.print((int)euler2.y());Serial.print(";");

  Serial.print((int)euler2.z());Serial.print("\t");
  PoseX2 = (int)euler2.x();
    PoseY2 = (int)euler2.y();
      PoseZ2 = (int)euler2.z();
  /* Also send calibration data for each sensor. */
  bno2.getCalibration(&sys, &gyro, &accel, &mag);

  disableMuxPort(ch2); //Tell mux to disconnect from this port

//enableMuxPort(ch3); //Tell mux to connect to this port, and this port only
    // Possible vector values can be:
  // - VECTOR_ACCELEROMETER - m/s^2
  // - VECTOR_MAGNETOMETER  - uT
  // - VECTOR_GYROSCOPE     - rad/s
  // - VECTOR_EULER         - degrees
  // - VECTOR_LINEARACCEL   - m/s^2
  // - VECTOR_GRAVITY       - m/s^2
  /* Get a new sensor event */
 // bno3.getEvent(&event);
  /* Board layout:
         +----------+
         |         *| RST   PITCH  ROLL  HEADING
     ADR |*        *| SCL
     INT |*        *| SDA     ^            /->
     PS1 |*        *| GND     |            |
     PS0 |*        *| 3VO     Y    Z-->    \-X
         |         *| VIN
         +----------+
  */

  /* The processing sketch expects data as roll, pitch, heading */
  //Serial.print(F("Orientation: "));
//  Serial.print((float)event.orientation.x);Serial.print(";");
 // Serial.print(F(" "));
//  Serial.print((float)event.orientation.y);Serial.print(";");
 // Serial.print(F(" "));
 // Serial.print((float)event.orientation.z);Serial.print("\t");
 // Serial.println(F(""));
 // PoseX3 = (float)event.orientation.x;
  //  PoseY3 = (float)event.orientation.y;
   //   PoseZ3 = (float)event.orientation.z;
 
  /* Also send calibration data for each sensor. */
//  bno3.getCalibration(&sys, &gyro, &accel, &mag);
 /* Serial.print(F("Calibration: "));
  Serial.print(sys, DEC);
  Serial.print(F(" "));
  Serial.print(gyro, DEC);
  Serial.print(F(" "));
  Serial.print(accel, DEC);
  Serial.print(F(" "));
  Serial.println(mag, DEC);*/
  Serial.print("\r\n");
//   disableMuxPort(ch3); //Tell mux to disconnect from this port

       // Send the distance to the client, along with a break to separate our messages
  const char ip[]="192.168.1.113";
  Client.beginPacket(ip,27);
  Client.println(PoseX+";"+PoseY+";"+PoseZ+";"+PoseX2+";"+PoseY2+";"+PoseZ2);
  //Client.println(PoseX+";"+PoseY+";"+PoseZ);
  Client.endPacket();
  delay(BNO055_SAMPLERATE_DELAY_MS);
 } 
