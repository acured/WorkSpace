# include<Motor.h>
# include<MotorRight.h>
/*
Pay attention to the interrupt pin,please check which microcontroller you use.
http://arduino.cc/en/Reference/AttachInterrupt
*/

Motor motorLeft(false,2,8,4,5);
MotorRight motorRight(true,3,9,7,6);//Motor(boolean isClock,byte encoder0pinA,byte encoder0pinB,int motorPinClock,int speedPin);   

int dt = 50;

double leftRate = 0.5;
double rightRate = 0.5;

void changeSpeed(double detaLeftRate,double detaRightRate)
{
  double left = leftRate + detaLeftRate;
  double right = rightRate+detaRightRate;

  if(left>1)
  {
    left=1;
  }
  if(right>1)
  {
    right=1;
  }
  
  motorLeft.setSpeed(left);
  motorRight.setSpeed(right);
}

void setup()
{  
  Serial.begin(9600);//Initialize the serial port
  motorLeft.setSpeed(leftRate);
  motorRight.setSpeed(rightRate);

  motorLeft.go();
  motorRight.go();
}

int count=0;
void loop()
{
  if(count==50)
  changeSpeed(0.5,0);
  else if(count==100)
  changeSpeed(0,0.5);
  else
  go();
  delay(dt);
  count++;
  
}

   
void go()
{
    int leftSpeed = motorLeft.getSpeed();  
    int rightSpeed = motorRight.getSpeed();  

    Serial.print(leftSpeed);
    Serial.print("    ");
    Serial.println(rightSpeed);
    
    motorLeft.redressSpeed(leftSpeed);
    motorRight.redressSpeed(rightSpeed);
}
