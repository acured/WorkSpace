# include<Motor.h>
# include<MotorRight.h>
//# include<DFSensor.h>
# include<Math.h>
# include <NewPing.h>

int count = 0;
Motor motorLeft(false,2,8,4,5);
MotorRight motorRight(true,3,9,7,6);//Motor(boolean isClock,byte encoder0pinA,byte encoder0pinB,int motorPinClock,int speedPin);  

double expectDistance=70;
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
double Kp=0.002;
double Ki=0.00008;//0.0001;
double Kd=0.0005;//0.00005;

int leftSpeed =0;
int rightSpeed=0;

double leftRate = 0.5;
double rightRate = 0.5;

double distanceLR = 0;
double lastDistanceLR = 0;
double distanceLF = 0;
double lastDistanceLF = 0;
double distanceRR = 0;
double lastDistanceRR = 0;
double distanceRF = 0;
double lastDistanceRF = 0;


double _Pk = 200;
double _Xk = 0;
/*
DFSensor sensorFront(8,13,A0,1);//0 is useing analog mode! Sensor(int URTRIG,int URECHO,uint8_t sensorPin,int Mode);
DFSensor sensorRear(12,11,A1,1);

DFSensor sensorLF(8,13,A1,0);//0 is useing analog mode! Sensor(int URTRIG,int URECHO,uint8_t sensorPin,int Mode);
DFSensor sensorLR(8,0,A3,0);
DFSensor sensorRF(8,11,A0,0);
DFSensor sensorRR(8,12,A2,0);
*/
#define TRIGGER_PIN  10  // Arduino pin tied to trigger pin on the ultrasonic sensor.
#define ECHO_PIN     11  // Arduino pin tied to echo pin on the ultrasonic sensor.
#define MAX_DISTANCE 200 // Maximum distance we want to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.

#define TRIGGER_PIN_2  12  // Arduino pin tied to trigger pin on the ultrasonic sensor.
#define ECHO_PIN_2     13  // Arduino pin tied to echo pin on the ultrasonic sensor.
#define MAX_DISTANCE_2 200 // Maximum distance we want to ping for (in centimeters). Maximum sensor distance is rated at 400-500cm.

NewPing sensorLF(TRIGGER_PIN, ECHO_PIN, MAX_DISTANCE); // NewPing setup of pins and maximum distance.
NewPing sensorLR(TRIGGER_PIN_2, ECHO_PIN_2, MAX_DISTANCE_2); // NewPing setup of pins and maximum distance.

//NewPing sensorLF(10, 11, 200); // NewPing setup of pins and maximum distance.
//NewPing sensorLR(12, 13, 200); // NewPing setup of pins and maximum distance.

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
  //sensorLF.start();
  //sensorLR.start();
  //sensorRF.start();
  //sensorRR.start();
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
  delay(50);
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

  
    motorLeft.redressSpeed(leftSpeed);
    motorRight.redressSpeed(rightSpeed);
}
void goWallFollowing()
{   
  //sensorLF.trig();
  
  lastDistanceLR = distanceLR;
  lastDistanceLF = distanceLF;
  //lastDistanceRR = distanceRR;
  //lastDistanceRF = distanceRF;
   
  distanceLR = sensorLR.ping()/ US_ROUNDTRIP_CM;  
  delay(50);
  distanceLF = sensorLF.ping()/ US_ROUNDTRIP_CM;
  //distanceRR = sensorRR.echo();  
  //distanceRF = sensorRF.echo();
  if(distanceLF==0&&(distanceLR-lastDistanceLF)>0&&distanceLR>100)
  {
      stopWallFollowing();  
      start = false;
      return;
  }
  
  double dis_L = 0;
  //double dis_R = 0;
  dis_L = min(distanceLR,distanceLF);

  if(dis_L==0)
      dis_L = max(distanceLR,distanceLF);
  
  //dis_R = min(distanceRR,distanceRF);


  if(true)
  {
    double dis = dis_L;
    //dis = getAndUpdateG(dis);
  
    /*kalman filter*/
    //double dis_temp = dis - expectDistance;
    double q = 100;
    double r = 10;
    double Pk = _Pk+q;
    double Kk = 0;
  
    Kk = Pk/(Pk+r);
    _Xk = _Xk + Kk * (dis - _Xk);
    _Pk = (1-Kk)*Pk;

    dis = _Xk;


    /*PIC control*/
    double error = dis - expectDistance;
    dd = error;
    integral = integral+dd*dt;
    derivative = (dd-previous_dd)/dt;
    double output = Kp*dd+Ki*integral+Kd*derivative;
    previous_dd=dd;
   
    Serial.print(distanceLF);
    Serial.print(",");
    Serial.print(distanceLR);
    //Serial.print(",");
    //Serial.print(distanceRF);
    //Serial.print(",");
    //Serial.print(distanceRR);
    Serial.print("!");

    //Serial.println(output);
    
    if(output<0)
    {
      changeSpeed(abs(output),0);
      delay(50);
      go();
    }
    else if(output>0)
    {
      changeSpeed(0,abs(output));
      delay(50);
      go();
    }
    else 
    {
      changeSpeed(0,0);  
      delay(50);
      //go();
    }
    
  }
}
void startWallFollowing()
{
  motorLeft.go();
  motorRight.go();

  integral=0;
  _Pk=1;
  _Xk=0;
}
void stopWallFollowing()
{      
  motorLeft.stop();
  motorRight.stop();
}

