
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

const char* ssid ="AndroidAP19E7";// "dlink";
const char* password = "avrw4684";//"468255000";
String PoseX, PoseY, PoseZ, PoseX2, PoseY2, PoseZ2, accX, accY, accZ, accX2, accY2, accZ2;
 /* Set the delay between fresh samples */
#define BNO055_SAMPLERATE_DELAY_MS (100)

Adafruit_BNO055 bno = Adafruit_BNO055(55);
Adafruit_BNO055 bno2 = Adafruit_BNO055(55);
Adafruit_BNO055 bno3 = Adafruit_BNO055(55);

const int FLEX_PIN = A0; // Pin connected to voltage divider output

// Measure the voltage at 5V and the actual resistance of your
// 47k resistor, and enter them below:
const float VCC = 3.3; // Measured voltage of Ardunio 5V line
const float R_DIV = 99600.0; // Measured resistance of 3.3k resistor

// Upload the code, then try to adjust these values to more
// accurately calculate bend degree.
const float STRAIGHT_RESISTANCE = 28300.0; // resistance when straight
const float BEND_RESISTANCE = 60000.0; // resistance at 90 deg
int Filter_Value;

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
  pinMode(FLEX_PIN, INPUT);
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

    Filter_Value = Filter()+30;       // 獲得濾波器輸出值
   // Serial.print(Filter_Value);Serial.print(";");
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
  imu::Vector<3> acc = bno.getVector(Adafruit_BNO055::VECTOR_LINEARACCEL);
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
//
  Serial.print((int)euler.y());Serial.print(";");
//
  Serial.print((int)euler.z());Serial.print("\t");
  accX = (int)acc.x();
  accY = (int)acc.y();
  accZ = (int)acc.z();
//  Serial.print((int)acc.x()+(int)acc.y());
//  Serial.print(",");
//  Serial.print((int)acc.x());
//  Serial.print(",");
  //Serial.print((int)acc.y());
//  Serial.print(",");
 // Serial.println((int)acc.z());

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
   imu::Vector<3> acc2 = bno2.getVector(Adafruit_BNO055::VECTOR_LINEARACCEL);
   /* The processing sketch expects data as roll, pitch, heading */
//  accX2 = (int)acc2.x();
//  accY2 = (int)acc2.y();
 // accZ2 = (int)acc2.z();
 /* Serial.print((int)acc2.x()+(int)acc2.y());
  Serial.print(",");
  Serial.print((int)acc2.x());
  Serial.print(",");
  Serial.print((int)acc2.y());
  Serial.print(",");
  Serial.println((int)acc2.z());*/
  Serial.print((int)euler2.x());Serial.print(";");
//
  Serial.print((int)euler2.y());Serial.print(";");
//
  Serial.println((int)euler2.z());Serial.print("\t");
  PoseX2 = (int)euler2.x();
    PoseY2 = (int)euler2.y();
      PoseZ2 = (int)euler2.z();
  accX2 = (int)acc2.x();
  accY2 = (int)acc2.y();
  accZ2 = (int)acc2.z();
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
  //設定接收端ip與port
  const char ip[]="192.168.43.89";
  Client.beginPacket(ip,27);
  Client.println(PoseX+";"+PoseY+";"+PoseZ+";"+PoseX2+";"+PoseY2+";"+PoseZ2+";"+accX+";"+accY+";"+accZ+";"+accX2+";"+accY2+";"+accZ2);
  //Client.println(PoseX+";"+PoseY+";"+PoseZ);
  Client.endPacket();
  delay(BNO055_SAMPLERATE_DELAY_MS);
 } 

 /*
A、名稱：加權遞推平均濾波法
B、方法：
    是針對遞推平均濾波法的改進．即不同時刻的數據加以不同的權重
    通常是，越靠近現時刻的數據，權取得越大。
    給予新採樣值得權係數越大，則靈敏度越高，但信號平滑度越低。
C、優點：
    適用於有效大純滯後時間常數的對象，和採樣周期較短的系統。
D、缺點：
    對於純滯後時間長數較小、採樣周數較長、變化緩慢的信號；
    不能迅速反應系統當前所受干擾的嚴重程度，濾波效果差。
*/
// 加權遞推平均濾波法
#define FILTER_N 12
int coe[FILTER_N] = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12};    // 加權係數表
int sum_coe = 1 + 2 + 3 + 4 + 5 + 6 + 7 + 8 + 9 + 10 + 11 + 12; // 加權係數和
int filter_buf[FILTER_N + 1];
int Filter() {
  // Read the ADC, and calculate voltage and resistance from it
  int flexADC = analogRead(FLEX_PIN);
  float flexV = flexADC * VCC / 1023.0;
  float flexR = R_DIV * (VCC / flexV - 1.0);
 // Serial.println("Resistance: " + String(flexR) + " ohms");

  // Use the calculated resistance to estimate the sensor's
  // bend angle:
  float angle = map(flexR, STRAIGHT_RESISTANCE, BEND_RESISTANCE,
                   0, 90.0);
  int i;
  int filter_sum = 0;
  filter_buf[FILTER_N] = angle;
  for(i = 0; i < FILTER_N; i++) {
    filter_buf[i] = filter_buf[i + 1]; // 所有數據左移，低位仍掉
    filter_sum += filter_buf[i] * coe[i];
  }
  filter_sum /= sum_coe;
  return filter_sum;
}
