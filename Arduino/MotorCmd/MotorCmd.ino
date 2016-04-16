/*
Pay attention to the interrupt pin,please check which microcontroller you use.
http://arduino.cc/en/Reference/AttachInterrupt
*/
  
//The sample code for driving one way motor encoder
const byte encoder0pinA_motive = 3;//A pin -> the interrupt pin 2
const byte encoder0pinB_motive = 5;//B pin -> the digital pin 4

const byte encoder0pinA = 2;//A pin -> the interrupt pin 2
const byte encoder0pinB = 4;//B pin -> the digital pin 4
byte encoder0PinALast;
int duration;//the number of the pulses
boolean Direction;//the rotation direction 
   
   
void setup()
{  
  Serial.begin(9600);//Initialize the serial port    EncoderInit();//Initialize the module
  EncoderInit();//Initialize the module
  
  pinMode(3, OUTPUT);
  TCCR2A = _BV(COM2A1) | _BV(COM2B1) | _BV(WGM20);
  TCCR2B = _BV(CS22);
  OCR2A = 180;
  OCR2B = 100;
}
   
void loop()
{
   Serial.print("Pulse:");
   Serial.println(duration);
   duration = 0;
   delay(100); 
}

void EncoderInit()
{
  Direction = true;//default -> Forward  
  pinMode(encoder0pinB,INPUT);  
  attachInterrupt(0, wheelSpeed, CHANGE);//int.0 
}

void wheelSpeed()
{
  int Lstate = digitalRead(encoder0pinA);
  if((encoder0PinALast == LOW) && Lstate==HIGH)
  {
    int val = digitalRead(encoder0pinB);
    if(val == LOW && Direction)
    {
      Direction = false; //Reverse
    }
    else if(val == HIGH && !Direction)
    {
      Direction = true;  //Forward
    }
  }
  encoder0PinALast = Lstate;
   
  if(!Direction)  duration++;
  else  duration--;
}
