#include <TimerOne.h>
const int fanPin = 9;

void setup(void)
{
  Timer1.initialize(20000);  // 40 us = 25 kHz
  Serial.begin(9600);

   Timer1.pwm(fanPin, 120);
}

void loop(void)
{
  //slowly increase the PWM fan speed
  //
 for(float dutyCycle=128.0;dutyCycle>0;dutyCycle-=1){
  Timer1.pwm(fanPin,dutyCycle);
    Serial.print("PWM Fan, Duty Cycle = ");
    Serial.println(dutyCycle);
   
    delay(300);}
}
