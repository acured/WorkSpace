// # Editor    : Jiang from DFRobot
// # Data      : 24.07.2012
 
// # Product name:ultrasonic scanner Kit
// # Product SKU:SEN0001
// # Version :  0.2
 
// # Description:
// # The Sketch for scanning 180 degree area 4-500cm detecting range
 
// # Connection:
// #       Pin 1 VCC (URM V3.2) -> VCC (Arduino)
// #       Pin 2 GND (URM V3.2) -> GND (Arduino)
// #       Pin 4 PWM (URM V3.2) -> Pin 3 (Arduino)
// #       Pin 6 COMP/TRIG (URM V3.2) -> Pin 5 (Arduino)
// # Pin mode: PWM
// # Working Mode: PWM passive control mode.
// # If it is your first time to use it,please make sure the two jumpers to the right hand
// # side of the device are set to TTL mode. You'll also find a secondary jumper on 
// # the left hand side, you must break this connection or you may damage your device. 
 
#include <Servo.h>                                  // Include Servo library
Servo myservo;                                      // create servo object to control a servo 
 
int pos=0;                                          // variable to store the servo position
int URPWM=3;                                        // PWM Output 0-25000us,every 50us represent 1cm
int URTRIG=5;                                       // PWM trigger pin
boolean up=true;                                    // create a boolean variable
unsigned long time;                                 // create a time variable
unsigned long urmTimer = 0;                          // timer for managing the sensor reading flash rate
 
unsigned int Distance=0;
uint8_t EnPwmCmd[4]={0x44,0x22,0xbb,0x01};          // distance measure command
 
void setup(){                                      // Serial initialization
  Serial.begin(9600);                              // Sets the baud rate to 9600
  myservo.attach(9);                                // Pin 9 to control servo
  PWM_Mode_Setup();
}
 
void loop(){   
   if(millis()-time>=20){                           // interval 0.02 seconds
     time=millis();                                 // get the current time of programme
     if(up){                                        // judge the condition
      if(pos>=0 && pos<=179){                   
        pos=pos+1;                                  // in steps of 1 degree 
        myservo.write(pos);                         // tell servo to go to position in variable 'pos' 
      }
      if(pos>179)  up= false;                       // assign the variable again
    }
     else {                                       
      if(pos>=1 && pos<=180){    
        pos=pos-1;
        myservo.write(pos);
      }
      if(pos<1)  up=true;
    }
   }
    
   if(millis()-urmTimer>50){
     urmTimer=millis(); 
     PWM_Mode();
   }
 }
 
 void PWM_Mode_Setup(){ 
  pinMode(URTRIG,OUTPUT);                            // A low pull on pin COMP/TRIG
  digitalWrite(URTRIG,HIGH);                         // Set to HIGH
  
  pinMode(URPWM, INPUT);                             // Sending Enable PWM mode command
  
  for(int i=0;i<4;i++){
      Serial.write(EnPwmCmd[i]);
   } 
}
 
void PWM_Mode(){                                     // a low pull on pin COMP/TRIG  triggering a sensor reading
    digitalWrite(URTRIG, LOW); 
    digitalWrite(URTRIG, HIGH);                      // reading Pin PWM will output pulses
     
    unsigned long DistanceMeasured=pulseIn(URPWM,LOW);
     
    if(DistanceMeasured==50000){                     // the reading is invalid.
      Serial.print("Invalid");    
   }
    else{
      Distance=DistanceMeasured/50;                  // every 50us low level stands for 1cm
   }
  Serial.print("Distance=");
  Serial.print(Distance);
  Serial.println("cm");
}

