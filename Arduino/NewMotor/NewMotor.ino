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
    //digitalWrite(_motorPinClockR, LOW);
    //analogWrite(_speedPinR,0 );
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
    //digitalWrite(_motorPinClockL, LOW);
    //analogWrite(_speedPinL,0 );
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
void setDirection(int ld,int rd)
{
    Serial.print(ld);
    Serial.print(rd);
    
    //pinMode(_motorPinClockL, OUTPUT);    
    //pinMode(_motorPinClockR, OUTPUT);
    if(ld)
    {
        digitalWrite(_motorPinClockL, HIGH);
        clockL = true;
    }
    else
    { 
        digitalWrite(_motorPinClockL, LOW);
        clockL = false;
    }
       
    if(rd)
    {
        digitalWrite(_motorPinClockR, HIGH);
        clockR = true;
    }
    else
    {
        digitalWrite(_motorPinClockR, LOW); 
        clockR = false; 
    }
    delay(10);
}
void stopspeed()
{
  int dl = 0;
  int dr = 0;
  if(clockL)
  {  
      dl = 0;
  }
  else
  {
      dl = 1;
  }
  
  if(clockR)
  {
      dr = 0;
  }    
  else
  {
      dr = 1;
  }
  setDirection(dl,dr);
  setspeed(0,0); 
  setDirection(0,0);
  expectspeedL = 0;
  expectspeedR = 0;
  _rateL=0;
  _rateR=0;
  mySerial.println("ok");
}

void setspeed(int ls,int rs)
{
    double l = ((double)(ls)/100)*255;
    double r = ((double)(rs)/100)*255;
    analogWrite(_speedPinL, l);
    analogWrite(_speedPinR, r);
}

void changespeed(int ls,int rs)
{
    setspeed(ls,rs);
    double l = ((double)(ls)/100)*255;
    double r = ((double)(rs)/100)*255;
    //analogWrite(_speedPinL, l);
    //analogWrite(_speedPinR, r);
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
  
    //if has command message or speed message
    if (mySerial.available()) 
    {
        str = mySerial.readString();
    
        if(str=="wallfollow")
        {
            //setDirection(0,1);
            cmd = "wallfollow";
        }
        if(str=="0")
        {
            startCheck();     
        }
        if(str=="go")
        {
            //setDirection(0,1);
            cmd = "go";
        }
        if(str=="done")      
        {
            cmd = "";
        }
        else
        {
        //do action:
        if(cmd == "wallfollow")//wallfollowing
        {
            
            long int values = str.toInt();
            //Serial.println(values);
            int speedvalue = (values%10000);
            int dir = (values/10000);
            if(speedvalue == 0)
            {
                //Serial.println("stop");
                stopspeed();
            }
            else
            {
                int ls = speedvalue/100;
                int rs = speedvalue%100;
                int ld = dir/10;
                int rd = dir%10;
                setDirection(0,1);
                changespeed(ls,rs);
            }
        }   
        else if(cmd == "turn")//turn
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
                int ls = speedvalue/100;
                int rs = speedvalue%100;
                int ld = dir/10;
                int rd = dir%10;
                setDirection(0,1);
                setspeed(ls,rs);
            }
        }
        else if(cmd == "go")//go
        {
            long int values = str.toInt();
            int speedvalue = (values%10000);
            if(speedvalue == 0)
            {
                stopspeed();
                //cmd = "";
            }
            else
            {                
                int dir = (values/10000);
                int ls = speedvalue/100;
                int rs = speedvalue%100;
                int ld = dir/10;
                int rd = dir%10;
                expectspeedL = ((double)ls/100)*255;
                expectspeedR = ((double)rs/100)*255;
                _rateL = expectspeedL/255;
                _rateR = expectspeedR/255;
                setDirection(0,1);
                setspeed(ls,rs);   
            }
        }
    }
    }
    else if(cmd=="go")//if no cmd msg or speed cmd
    {
      if(_rateL!=0||_rateR!=0)
      {  
        int duR = _durationR;
        int duL = _durationL;
      
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
        if(abs(duR)<200)
        {
            checkspeedR(duR);
        }
        else
        {
            mySerial.print("wrong data!");
        }
      }
    }
    delay(dt);
}
