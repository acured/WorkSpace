# include<Motor.h>
# include<MotorRight.h>
# include<Sensor.h>
# include<Sensor1.h>

int count = 0;
Motor motorLeft(true,2,4,5,6);
MotorRight motorRight(false,3,7,9,10);//Motor(boolean isClock,byte encoder0pinA,byte encoder0pinB,int motorPinClock,int motorPinCounterClock);

double expectDistance=40;
double integral = 0;
double derivative=0;
double previous_dd=0;
double dd=0;
static double dt=0.01;
static double Ku=0.01;
static double Tu=2;
//double Kp=Ku*0.6;
//double Ki=(Ku*2)/Tu;
//double Kd=(Ku*Tu)/8;
double Kp=0.004;
double Ki=0.00008;//0.0001;
double Kd=0.0005;//0.00005;

int leftSpeed =0;
int rightSpeed=0;

double leftRate = 0.52;
double rightRate = 0.5;

double distanceRear = 0;
double lastDistanceRear = 0;
double distanceFront = 0;
double lastDistanceFront = 0;

Sensor sensorFront(11,8,A0,0);//0 is useing analog mode! Sensor(int URECHO,int URTRIG,uint8_t sensorPin,int Mode);
Sensor1 sensorRear(11,12,A1,0);//0 is useing analog mode! Sensor(int URECHO,int URTRIG,uint8_t sensorPin,int Mode);
// # Connection:
// #       Vcc (Arduino)    -> Pin 1 VCC (URM V4.0)
// #       GND (Arduino)    -> Pin 2 GND (URM V4.0)
// #       Pin 11 (Arduino)  -> Pin 4 ECHO (URM V4.0)
// #       Pin 6 (Arduino)  -> Pin 6 COMP/TRIG (URM V4.0)
// #       Pin A0 (Arduino) -> Pin 7 DAC (URM V4.0)
// # Working Mode: PWM trigger pin  mode.

int cmd = 0;   // for incoming serial data
bool start = false;

bool flag = true;

void setup() {
  Serial.begin(9600);
  // put your setup code here, to run once:
  motorLeft.setSpeed(leftRate);
  motorRight.setSpeed(rightRate);
  motorLeft.go();
  motorRight.go();
  sensorFront.start();
  sensorRear.start();
  //Serial.println("end init!");
}

void loop() {
  if(start)
  {  
    // put your main code here, to run repeatedly:
    goWallFollowing();
  }
  if (Serial.available() > 0) {
    // read the incoming byte:
    cmd = Serial.read();
    // say what you got:
    //Serial.println(cmd, DEC);
    if(cmd==2||cmd ==50)
    {
      startWallFollowing();  
      start = true;
      //Serial.print("WallFollowing start!");
      //Serial.println(cmd, DEC);
    }
    else if(cmd==59)
    {
      changeSpeed(0.2,0);
      startWallFollowing();  
      Serial.print("Test ");
      Serial.println(cmd, DEC);
    }
    else if(cmd==58)
    {
      changeSpeed(0,0.2); 
      startWallFollowing();  
      Serial.print("Test ");
      Serial.println(cmd, DEC);
    } 
    else if(cmd==57)
    {
      changeSpeed(0,0);
      startWallFollowing();  
      Serial.print("Test ");
      Serial.println(cmd, DEC);
    }
    else if(cmd==3||cmd==51)
    {
      stopWallFollowing();  
      start = false;
      //Serial.print("WallFollowing stop!");
      //Serial.println(cmd, DEC);
    }
  }
  delay(100);
}
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
void go()
{
    leftSpeed = motorLeft.getSpeed();  
    rightSpeed = motorRight.getSpeed();  
        
    /*Serial.print("Left Speed:");
    Serial.print(abs(leftSpeed)); 
    Serial.print("(");
    Serial.print(motorLeft.getRate());
    Serial.print(")");
    Serial.print("    Right Speed:");
    Serial.print(abs(rightSpeed)); 
    Serial.print("(");
    Serial.print(motorRight.getRate());
    Serial.println(")");*/

    motorLeft.redressSpeed(leftSpeed);
    motorRight.redressSpeed(rightSpeed);
}
void goWallFollowing()
{   
  //go();
  //Serial.println("**********************************************************************");  
  
  lastDistanceRear = distanceRear;
  lastDistanceFront = distanceFront; 
  distanceRear = sensorRear.getDistance();  
  distanceFront = sensorFront.getDistance();
  if(distanceRear<=10||distanceFront<=10)
  {
   // Serial.print("bad sensro data!!!");  
    return;
  }
  /*if((distanceRear<=20&&lastDistanceRear>40)||(distanceFront<=20&&lastDistanceFront>40))
  {
    return;
  }*/
  double dis = 0;   
  
  /*Serial.print("Distance front:");     
  Serial.print(lastDistanceFront); 
  Serial.print("->");
  Serial.print(distanceFront);
  Serial.print("      Distance rear:");        
  Serial.print(lastDistanceRear); 
  Serial.print("->");   
  Serial.println(distanceRear);*/

  if(distanceFront>distanceRear)
  {
    dis = distanceRear;
  }
  else 
  {
    dis = distanceFront;
  }

  if(distanceFront>120&&distanceRear>120)
  {  
   // Serial.print("Out distance! stop!!");  
    
    start = false;  
    stopWallFollowing();
    return;
  }

 
  double error = dis-expectDistance;
  dd = error;
  integral = integral+dd*dt;
  derivative = (dd-previous_dd)/dt;
  double output = Kp*dd+Ki*integral+Kd*derivative;
  previous_dd=dd;
  
 /* Serial.print("E:");      
  Serial.print(dd*Kp);
  Serial.print("     Integral:");      
  Serial.print(integral*Ki);
  Serial.print("     Derivative:");      
  Serial.print(derivative*Kd);
  Serial.print("     Output:");      */
  Serial.print(output);
  
  Serial.print(",");
  Serial.print(dis);
  Serial.print(",");
  Serial.print(derivative);
  Serial.print("!");
  
  if(output<0)
  {
    changeSpeed(abs(output),0);
    delay(100);
    go();
   // Serial.println("    Turn Right!");
  }
  else if(output>0)
  {
    changeSpeed(0,abs(output));
    delay(100);
    go();
    //Serial.println("    Turn Left!");
  }
  else 
  {
    changeSpeed(0,0);  
    delay(100);
    go();
   // Serial.println("    Go!");
  }
}
void startWallFollowing()
{
  motorLeft.go();
  motorRight.go();
  sensorFront.start();
  sensorRear.start();
  integral=0;
}
void stopWallFollowing()
{      
  motorLeft.stop();
  motorRight.stop();
  sensorFront.stop();
  sensorRear.stop();
}

