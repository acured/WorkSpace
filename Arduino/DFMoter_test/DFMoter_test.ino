# include<Motor.h>
# include<MotorRight.h>

Motor motorLeft(true,2,4,5,6);
MotorRight motorRight(false,3,7,9,10);

void setup()
{  
  Serial.begin(9600);//Initialize the serial port
  motorLeft.setSpeed(0.5);
  motorLeft.go();
}

void loop()
{
  int leftSpeed = motorLeft.getSpeed();   
  Serial.println(leftSpeed);
  motorLeft.redressSpeed(leftSpeed);
  delay(200);
}
