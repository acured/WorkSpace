// # Editor     : ZRH from DFRobot
// # Date       : 29.08.2014
 
// # Product name: URM V4.0 ultrasonic sensor
// # Product SKU : SEN0001
// # Version     : 1.0
 
// # Description:
// # The Sketch for scanning 180 degree area 3-500cm detecting range
// # The sketch for using the URM37 PWM trigger pin mode from DFRobot  
// #   and writes the values to the serialport
// # Connection:
// #       Vcc (Arduino 5V)    -> Pin 1 VCC (URM V4.0)
// #       GND (Arduino)    -> Pin 2 GND (URM V4.0)
// #       Pin 3 (Arduino)  -> Pin 4 ECHO (URM V4.0)
// #       Pin 5 (Arduino)  -> Pin 6 COMP/TRIG (URM V4.0)

// #       Vcc (Arduino 3.3V)    -> Pin 1 VCC (Old Sensor)
// #       GND (Arduino)    -> Pin 4 GND (Old Sensor)
// #       Pin 6 (Arduino)  -> Pin 3 ECHO (Old Sensor)
// #       Pin 9 (Arduino)  -> Pin 2 COMP/TRIG (Old Sensor)
// # Working Mode: PWM trigger pin  mode.
 
#define  Measure  1     //Mode select

typedef struct Sensor{
  int URECHO;
  int URTRIG;
  int sensorValue=0;
  }Sensor;
const int SENSOR_COUNT=1;
int current_sensor;
unsigned int  DistanceMeasured[SENSOR_COUNT];

  Sensor sensors[2];
  
  
void setup() 
{
 sensors[0].URTRIG=3;
  sensors[0].URECHO=5;
  //Serial initialization
  Serial.begin(9600);  
  // Sets the baud rate to 9600
  for(int i=0;i<SENSOR_COUNT;i++)
  {
  pinMode(sensors[i].URTRIG,OUTPUT);                    // A low pull on pin COMP/TRIG
  digitalWrite(sensors[i].URTRIG,HIGH);                 // Set to HIGH 
  pinMode(sensors[i].URECHO, INPUT); 
  }
  // Sending Enable PWM mode command
  delay(500);
  Serial.println("Init the sensor");
 current_sensor=0;
 }
void loop()
{
  Detect(current_sensor);
  delay(100);
  current_sensor=(current_sensor+1)%SENSOR_COUNT;
  for(int i =0;i<SENSOR_COUNT;i++)
  {
    Serial.print( DistanceMeasured[i]); Serial.print( "\t");
  }
  Serial.println();
} 
void Detect(int index)                              // a low pull on pin COMP/TRIG  triggering a sensor reading
{ 
  digitalWrite(sensors[index].URTRIG, LOW);
  digitalWrite(sensors[index].URTRIG, HIGH);               // reading Pin PWM will output pulses  
 
    unsigned long LowLevelTime = pulseIn(sensors[index].URECHO, LOW) ;
    if(LowLevelTime>=45000)                 // the reading is invalid.
    {
       DistanceMeasured[index]=-1;
    }
    else{
    DistanceMeasured[index] = LowLevelTime /50;   // every 50us low level stands for 1cm

  }
 
  
}
