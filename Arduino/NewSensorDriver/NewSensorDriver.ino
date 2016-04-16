#include <SoftwareSerial.h>

SoftwareSerial mySerial(10, 11); // RX=>serial tx pin, TX=>serial rx pin

double sensorLF=9999;
double sensorLR=9999;
double sensorRF=9999;
double sensorRR=9999;
double sensorF=9999;

String result;

void setup() {
  // put your setup code here, to run once:
  mySerial.begin(9600);
  mySerial.setTimeout(10);
  
  result = String("");
}

void loop() {
  // put your main code here, to run repeatedly:
  if (mySerial.available()) {
    String str = mySerial.readString();

    if(str=="0")
    {
      startCheck();     
    }
    else
    {
      result = result + sensorLF + ";"+ sensorLR+";"+sensorRF+";"+sensorRR;    
      mySerial.println(result);
      result = "";
    }
  }
  updateRang();
}

void updateRang()
{
  sensorLF =  rand() % 255+1;
  sensorLR =  rand() % 255+1;
  sensorRF =  rand() % 255+1;
  sensorRR =  rand() % 255+1;
}

void startCheck()
{
  mySerial.println("sensor"); 
}

