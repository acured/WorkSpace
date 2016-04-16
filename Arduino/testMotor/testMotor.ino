#include <SoftwareSerial.h>
SoftwareSerial mySerial(10, 11); // RX=>serial tx pin, TX=>serial rx pin
byte _encoder0pinAR = 3;
byte _encoder0pinBR = 9;
int _motorPinClockR = 7;
int _speedPinR = 6;

byte _encoder0pinAL = 2;
byte _encoder0pinBL = 8;
int _motorPinClockL = 4;
int _speedPinL = 5;

byte _encoder0PinAlastR;
double _durationR;
boolean _DirectionR;

byte _encoder0PinAlastL;
double _durationL;
boolean _DirectionL;

double expectspeedL = 0;
double expectspeedR = 0;
double dt = 10;//mm
double maxspeed = 0;
double _rateL = 0;
double _rateR = 0;

bool clockL = false;
bool clockR = true;

String cmd = "";

void setup() {
  // put your setup code here, to run once:
    Serial.begin(9600);
    mySerial.begin(9600);
    mySerial.setTimeout(2);
      
    pinMode(_motorPinClockR, OUTPUT);
    pinMode(_speedPinR, OUTPUT);
    digitalWrite(_motorPinClockR, HIGH);//clock dir

    pinMode(_motorPinClockL, OUTPUT);
    pinMode(_speedPinL, OUTPUT);
    digitalWrite(_motorPinClockL, LOW);//conterclock dir

    maxspeed = 50;

    //Serial.println(maxspeed);

    EncoderInit();
}
void wheelSpeedL()
{
  int LstateL = digitalRead(_encoder0pinAL);
  if((_encoder0PinAlastL==LOW)&&LstateL==HIGH)
  {
    int valL =  digitalRead(_encoder0pinBL);
    if(valL==LOW&&_DirectionL)
    {
      _DirectionL = false;
    }
    else if(valL==HIGH&&!_DirectionL)
    {
      _DirectionL=true;
    }
  }
  _encoder0PinAlastL = LstateL;
  if(!_DirectionL)
  {
    _durationL++;
  }
  else _durationL--;
}
void wheelSpeedR()
{
  int Lstate = digitalRead(_encoder0pinAR);
  if((_encoder0PinAlastR==LOW)&&Lstate==HIGH)
  {
    int val =  digitalRead(_encoder0pinBR);
    if(val==LOW&&_DirectionR)
    {
      _DirectionR = false;
    }
    else if(val==HIGH&&!_DirectionR)
    {
      _DirectionR=true;
    }
  }
  _encoder0PinAlastR = Lstate;
  if(!_DirectionR)
  {
    _durationR++;
  }
  else _durationR--;
}

void EncoderInit()
{
      pinMode(_encoder0pinBR,INPUT);
      attachInterrupt(digitalPinToInterrupt(_encoder0pinAR),wheelSpeedR,CHANGE);
      pinMode(_encoder0pinBL,INPUT);
      attachInterrupt(digitalPinToInterrupt(_encoder0pinAL),wheelSpeedL,CHANGE);
}

void checkspeedR(double realspeedR)
{
  if(_rateR==0)
  {
    digitalWrite(_motorPinClockR, LOW);
    analogWrite(_speedPinR,0 );
    return;
  }
  realspeedR = abs(realspeedR);

  double detaR = (realspeedR/maxspeed) - _rateR;
  expectspeedR = expectspeedR-40*detaR;
  if(expectspeedR>255)expectspeedR=255;
  if(expectspeedR<0)expectspeedR=0;

  analogWrite(_speedPinR,expectspeedR );
}

void checkspeedL(double realspeed)
{
  if(_rateL==0)
  {
    digitalWrite(_motorPinClockL, LOW);
    analogWrite(_speedPinL,0 );
    return;
    }
  realspeed = abs(realspeed);

  double detaL = (realspeed/maxspeed) - _rateL;
  expectspeedL = expectspeedL-40*detaL;
  if(expectspeedL>255)expectspeedL=255;
  if(expectspeedL<0)expectspeedL=0;
  
  analogWrite(_speedPinL,expectspeedL );
}
void startCheck()
{
  mySerial.println("motor"); 
}
void stopspeed()
{
  digitalWrite(_speedPinR, 0);
  if(clockL)
  {  
    digitalWrite(_motorPinClockL, LOW);
    digitalWrite(_speedPinL, 0);
  }
  else
  {
    digitalWrite(_motorPinClockL, HIGH);
    digitalWrite(_speedPinL, 0);
    digitalWrite(_motorPinClockL, LOW);
  }
  
  if(clockR)
  {
    digitalWrite(_motorPinClockR, LOW);
    digitalWrite(_speedPinR, 0);
  }    
  else
  {
    digitalWrite(_motorPinClockR, HIGH);
    digitalWrite(_speedPinR, 0);
    digitalWrite(_motorPinClockR, LOW);
  }
  expectspeedL = 0;
  expectspeedR = 0;
  mySerial.println("ok");
}

void changespeed(int ls,int rs)
{


   double l = ((double)(ls)/100)*255;
   double r = ((double)(rs)/100)*255;
      
    digitalWrite(_motorPinClockL, LOW);
    digitalWrite(_motorPinClockR, HIGH);
    delay(10);
    analogWrite(_speedPinL, l);
    analogWrite(_speedPinR, r);
    //Serial.println("----------------------------------------------");

    rs = rs+5;
    while(true)
    {    
      int duR = _durationR;
      int duL = _durationL;  
      _durationL=0;
      _durationR=0;

      int detav = l - r;
      double dv = (double)ls/(double)rs;
      double ds = (double)duL/(double)duR;
      
      int detarealv = abs(duL)-abs(duR);
      /*
      Serial.println("**********************************************************");
      
      Serial.print(ls);
      Serial.print(" ");
      Serial.println(rs);
      
      Serial.print(duL);
      Serial.print(" ");
      Serial.println(duR);

      Serial.print(1/dv);
      Serial.print(" ");
      Serial.println(1/ds);
      */
      if(detav>25)
      {
        if(abs(duL)>abs(duR))
          break;
      }
      else if(detav<-25)
      {
        if(abs(duL)<abs(duR))
          break;
      }
      else break;
      
      delay(10);
    }
    
    
    mySerial.println("ok");
}
void loop() {
  // put your main code here, to run repeatedly:
  String str;
  
  //Serial.println(cmd);
  if (mySerial.available()) 
  {
     str = mySerial.readString();
     
     //Serial.print(str);
     //Serial.print(" ");
     //Serial.println(cmd);
     
     if(str=="wallfollow")
     {
        cmd = "wallfollow";
     }
     if(str=="0")
     {
        startCheck();     
     }
     if(str=="go")
     {
      cmd = "go";
     }
     if(str=="done")      
     {
      cmd = "";
      //Serial.println("stop");
     }

  if(cmd == "wallfollow")
  {
    //check the cmd is working.
    
    Serial.println(str);
    long int values = str.toInt();
    int speedvalue = (values%10000);
    if(speedvalue == 0)
    {
      Serial.println("stop");
       stopspeed();
    }
    else
    {
      int dir = (values/10000);
      int ls = speedvalue/100;
      int rs = speedvalue%100;

      changespeed(ls,rs);
      //if(speedvalue/100>50)
      //  Serial.print(speedvalue/100);
      //else Serial.print("  ");
      //Serial.print(" ");
      //if(speedvalue%100>45)
      //  Serial.println(speedvalue%100);
      //else Serial.println("  ");

    }
  }
  else if(cmd == "turn")
  {
    long int values = str.toInt();
    int speedvalue = (values%10000);
    if(speedvalue == 0)
    {
       stopspeed();
    }
    else
    {
      int dir = (values/10000);
      double l = ((double)(speedvalue/100)/100)*255;
      double r = ((double)(speedvalue%100)/100)*255;
      digitalWrite(_motorPinClockL, LOW);
      digitalWrite(_motorPinClockR, HIGH);
      analogWrite(_speedPinL, l);
      analogWrite(_speedPinR, r);
    }
  }
  else if(cmd == "go")
  {
    //Serial.println("do go");
    long int values = str.toInt();
    int speedvalue = (values%10000);
    if(speedvalue == 0)
    {
      _rateL = 0;
      _rateR = 0;
            
      digitalWrite(_speedPinL, 0);
      digitalWrite(_speedPinR, 0);
                        
    }
    else
    {
      int dir = (values/10000);
      double l = ((double)(speedvalue/100)/100)*255;
      double r = ((double)(speedvalue%100)/100)*255;
      expectspeedL = l;
      expectspeedR = r;
      _rateL = l/255;
      _rateR = r/255;
          
      if(dir/10)
      {
        clockL = true;
        digitalWrite(_motorPinClockL, HIGH);
      }
      else
      {
        clockL = false;
        digitalWrite(_motorPinClockL, LOW);
      }
  
      if(dir%10)
      {
        clockR = true;
        digitalWrite(_motorPinClockR, HIGH);
      }
      else
      {
        clockR = false;
        digitalWrite(_motorPinClockR, LOW);
      }
  
      analogWrite(_speedPinL, l);
      analogWrite(_speedPinR, r);     
  }
  }
  }
  else if(cmd=="go")
  {
    int duR = _durationR;
    int duL = _durationL;
    //Serial.println("------------------------------------");
    //Serial.print(duL);
    //Serial.print(" ");
    //Serial.println(duR);
  
    _durationL=0;
    _durationR=0;
    
    if(abs(duL)<200)
    {
      checkspeedL(duL);
    }
    else
    {
      mySerial.print("wrong data!");
    }
  
    //Serial.print(" ");
    if(abs(duR)<200)
    {
      checkspeedR(duR);
    }
    else
    {
      mySerial.print("wrong data!");
    }
    //Serial.println("");
  }
  //else if()
  
  delay(dt);
}
