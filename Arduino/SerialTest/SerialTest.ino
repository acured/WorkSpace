#include <SoftwareSerial.h>
#include <stdlib.h>
# include<Motor.h>
# include<MotorRight.h>


Motor motorLeft(false,2,8,4,5);
MotorRight motorRight(true,3,9,7,6);//Motor(boolean isClock,byte encoder0pinA,byte encoder0pinB,int motorPinClock,int speedPin);  

String str;
double dt = 100;
double unitMaxSpeed = 2;
double maxspeed = 0;
static bool clockdir = true;
static bool counterclockdir = false;
bool leftDir = counterclockdir;
bool rightDir = clockdir;
SoftwareSerial mySerial(10, 11); // RX, TX
void setup() {
  // put your setup code here, to run once:
  mySerial.begin(9600);
  mySerial.setTimeout(10);
  maxspeed = unitMaxSpeed*dt;
  motorLeft.setSpeed(0.5);
  motorRight.setSpeed(0.5);
  motorLeft.go();
  motorRight.go();
}

void loop() {
  if (mySerial.available()) {
    str = mySerial.readString();
    long int values = str.toInt();
    int speedvalue = (values%10000);
    mySerial.println(values);
    mySerial.println(speedvalue);
    int dir = (values/10000);
        mySerial.println(dir);
    double l = (speedvalue/100);
    double r = (speedvalue%100);
    if(dir/10)
      leftDir = counterclockdir;
    else
      leftDir = clockdir;
      
    if(dir%10)
      rightDir = counterclockdir;
    else
      rightDir = clockdir;

      
    l = l/100;
    r = r/100;
    speedChange(leftDir,rightDir,l,r);
  }
  Go();
  delay(dt);
}

void Go()
{
    int leftSpeed = motorLeft.getSpeed();  
    int rightSpeed = motorRight.getSpeed();  

    mySerial.print(leftSpeed);
    mySerial.print("    ");
    mySerial.println(rightSpeed);
    
    motorLeft.redressSpeed(leftSpeed);
    motorRight.redressSpeed(rightSpeed);
}

void speedChange(bool dirl,bool dirr,double l,double r)
{
  double left = l;
  double right = r;

  if(left>1)
  {
    left=1;
  }
  if(right>1)
  {
    right=1;
  }
  motorLeft.changeDir(dirl);
  motorRight.changeDir(dirr);
  motorLeft.setSpeed(left);
  motorRight.setSpeed(right);
}

