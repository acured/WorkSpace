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
// #       Vcc (Arduino)    -> Pin 1 VCC (URM V4.0)
// #       GND (Arduino)    -> Pin 2 GND (URM V4.0)
// #       Pin 3 (Arduino)  -> Pin 4 ECHO (URM V4.0)
// #       Pin 5 (Arduino)  -> Pin 6 COMP/TRIG (URM V4.0)
// #       Pin A0 (Arduino) -> Pin 7 DAC (URM V4.0)
// # Working Mode: PWM trigger pin  mode.
 
#define  Measure  1     //Mode select
int URECHO = 13;         // PWM Output 0-25000US,Every 50US represent 1cm
int URTRIG = 8;         // PWM trigger pin
int sensorPin = A0;     // select the input pin for the potentiometer
int sensorValue = 0;    // variable to store the value coming from the sensor
 
unsigned int DistanceMeasured= 0;

int URECHO2 = 11;         // PWM Output 0-25000US,Every 50US represent 1cm
int URTRIG2 = 12;         // PWM trigger pin
int sensorPin2 = A4;     // select the input pin for the potentiometer
int sensorValue2 = 0;    // variable to store the value coming from the sensor
 
unsigned int DistanceMeasured2= 0;
 
void setup() 
{
  //Serial initialization
  Serial.begin(9600);                        // Sets the baud rate to 9600
  pinMode(URTRIG,OUTPUT);                    // A low pull on pin COMP/TRIG
  digitalWrite(URTRIG,HIGH);                 // Set to HIGH 
  pinMode(URECHO, INPUT);                    // Sending Enable PWM mode command
 
  pinMode(URTRIG2,OUTPUT);                    // A low pull on pin COMP/TRIG
  digitalWrite(URTRIG2,HIGH);                 // Set to HIGH 
  pinMode(URECHO2, INPUT);                    // Sending Enable PWM mode command
  delay(500);
  Serial.println("Init the sensor");
 
 }
void loop()
{
  PWM_Mode();
  PWM_Mode2();
  delay(100);
} 

void PWM_Mode2()                              // a low pull on pin COMP/TRIG  triggering a sensor reading
{ 
  Serial.print("   Distance Measured=");
  digitalWrite(URTRIG2, LOW);
  digitalWrite(URTRIG2, HIGH);               // reading Pin PWM will output pulses  
  if( Measure)
  {
    unsigned long LowLevelTime = pulseIn(URECHO2, LOW) ;
    if(LowLevelTime>=45000)                 // the reading is invalid.
    {
      Serial.println("Invalid");
    }
    else{
    DistanceMeasured2 = LowLevelTime /50;   // every 50us low level stands for 1cm
    Serial.print(DistanceMeasured2);
    Serial.println("cm");
  }
 
  }
  else {
    sensorValue2 = analogRead(sensorPin2); 
    if(sensorValue2<=10)                   // the reading is invalid.
    {
      Serial.println("Invalid");
    }
    else {
    sensorValue2 = sensorValue2*0.718;      
    Serial.print(sensorValue2);
    Serial.println("cm");
    }
  } 
}
 
void PWM_Mode()                              // a low pull on pin COMP/TRIG  triggering a sensor reading
{ 
  Serial.print("Distance Measured=");
  digitalWrite(URTRIG, LOW);
  digitalWrite(URTRIG, HIGH);               // reading Pin PWM will output pulses  
  if( Measure)
  {
    unsigned long LowLevelTime = pulseIn(URECHO, LOW) ;
    if(LowLevelTime>=45000)                 // the reading is invalid.
    {
      Serial.print("Invalid");
    }
    else{
    DistanceMeasured = LowLevelTime /50;   // every 50us low level stands for 1cm
    Serial.print(DistanceMeasured);
    Serial.print("cm");
  }
 
  }
  else {
    sensorValue = analogRead(sensorPin); 
    if(sensorValue<=10)                   // the reading is invalid.
    {
      Serial.print("Invalid");
    }
    else {
    sensorValue = sensorValue*0.718;      
    Serial.print(sensorValue);
    Serial.print("cm");
    }
  } 
}
