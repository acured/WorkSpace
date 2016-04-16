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
int URECHO_F = 13;         // PWM Output 0-25000US,Every 50US represent 1cm
int URTRIG_F = 8;         // PWM trigger pin
int sensorPin_F = A0;     // select the input pin for the potentiometer
int sensorValue_F = 0;    // variable to store the value coming from the sensor

int URECHO_R = 11;         // PWM Output 0-25000US,Every 50US represent 1cm
int URTRIG_R = 12;         // PWM trigger pin
int sensorPin_R = A4;     // select the input pin for the potentiometer
int sensorValue_R = 0;    // variable to store the value coming from the sensor
 
unsigned int DistanceMeasured_F= 0;
unsigned int DistanceMeasured_R= 0;

int dis_F = 0;
int dis_R = 0;
 
void setup() 
{
  //Serial initialization
  Serial.begin(9600);                        // Sets the baud rate to 9600
  pinMode(URTRIG_F,OUTPUT);                    // A low pull on pin COMP/TRIG
  digitalWrite(URTRIG_F,HIGH);                 // Set to HIGH 
  pinMode(URECHO_F, INPUT);                    // Sending Enable PWM mode command

  
  pinMode(URTRIG_R,OUTPUT);                    // A low pull on pin COMP/TRIG
  digitalWrite(URTRIG_R,HIGH);                 // Set to HIGH 
  pinMode(URECHO_R, INPUT);                    // Sending Enable PWM mode command
  
  delay(500);
  Serial.println("Init the sensor");
 
 }
void loop()
{
  dis_F = MeasureF();
  delay(50);
  dis_R = MeasureR();

  char str[15];
  sprintf(str, "%d  %d", dis_F,dis_R);
  //sprintf(str, "%d", dis_F);
  
  Serial.println(str);  
  delay(50);
} 
 
int MeasureF()                              // a low pull on pin COMP/TRIG  triggering a sensor reading
{ 
  digitalWrite(URTRIG_F, LOW);
  digitalWrite(URTRIG_F, HIGH);               // reading Pin PWM will output pulses  
  
  if( Measure)
  {
    unsigned long LowLevelTime_F = pulseIn(URECHO_F, LOW) ;
    if(LowLevelTime_F>=45000)                 // the reading is invalid.
    {
      /*
      Serial.print("Distance Measured=");
      Serial.print("Invalid");*/
      return 9999;
    }
    else{
    DistanceMeasured_F = LowLevelTime_F /50;   // every 50us low level stands for 1cm
      /*Serial.print("Distance front Measured=");
      Serial.print(DistanceMeasured_F);
      Serial.print("cm");*/
      return DistanceMeasured_F;
    } 
  }
  else {
    sensorValue_F = analogRead(sensorPin_F); 
    if(sensorValue_F<=10)                   // the reading is invalid.
    {
      Serial.print("Distance front Measured=");
      Serial.print("Invalid");
    }
    else {
    sensorValue_F = sensorValue_F*0.718;      
    /*Serial.print("Distance front Measured=");
    Serial.print(sensorValue_F);
    Serial.print("cm");*/
    }  
  } 

  return sensorValue_F;
}

int MeasureR()                              // a low pull on pin COMP/TRIG  triggering a sensor reading
{ 
  digitalWrite(URTRIG_R, LOW);
  digitalWrite(URTRIG_R, HIGH);               // reading Pin PWM will output pulses  
  if( Measure)
  {
    unsigned long LowLevelTime_R = pulseIn(URECHO_R, LOW) ;
    if(LowLevelTime_R>=45000)                 // the reading is invalid.
    {
      /*Serial.print("  Distance rear Measured=");
      Serial.println("Invalid");*/
      return 9999;
    }
    else{
    DistanceMeasured_R = LowLevelTime_R /50;   // every 50us low level stands for 1cm
      /*Serial.print("  Distance rear Measured=");
      Serial.print(DistanceMeasured_R);
      Serial.println("cm");*/
      return DistanceMeasured_R;
    } 
  }
  else {
    sensorValue_R = analogRead(sensorPin_R); 
    if(sensorValue_R<=10)                   // the reading is invalid.
    {
      Serial.print("  Distance rear Measured=");
      Serial.println("Invalid");
    }
    else {
    sensorValue_R = sensorValue_R*0.718;      
    /*Serial.print("  Distance rear Measured=");
    Serial.print(sensorValue_R);
    Serial.println("cm");*/
    }
  }

  return sensorValue_R;
}
