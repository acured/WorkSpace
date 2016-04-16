#include "Arduino.h"
#include <SoftwareSerial.h>

SoftwareSerial mySerial(10, 11); // RX=>serial tx pin, TX=>serial rx pin
#define TriggerPin 2
#define SerialPort Serial
#define printByte(args) Serial.write(args)
#define urmAccount 2   // Init the account of the URM04 sensor

byte firstAddr = 0x11;
byte secondAddr = 0x12;
//byte thirdAddr = 0x13;
//byte RFaddr = 0x14;
//byte RRaddr = 0x15;
byte cmdst[10];
byte urmID[urmAccount];
unsigned int urmData[urmAccount];
#define CommMAXRetry 20

String str;


double sensorLF=9999;
double sensorLR=9999;
double sensorRF=9999;
double sensorRR=9999;
double sensorF=9999;

String result;

static unsigned long timers = 0;
static unsigned long timere = 0;

void setup() {
  // put your setup code here, to run once:
  pinMode(TriggerPin,OUTPUT);  // TTL -> RS485 chip driver pin
  digitalWrite(TriggerPin,LOW);
  SerialPort.begin(19200);  
  
  for(int i = 0 ;i < 10; i++)  cmdst[i] = 0;  //init the URM04 protocol
  for(int i = 0 ;i < urmAccount; i++)  urmData[i] = 0;  //init the URM04 protocol
  urmID[0] = firstAddr;
  urmID[1] = secondAddr;
  //urmID[2] = thirdAddr;
  //urmID[3] = RFaddr;
  //urmID[4] = RRaddr;

  //SerialPort.println("ready!");  

  mySerial.begin(9600);
  mySerial.setTimeout(2);
  
  result = String("");
}

void  setAddr(){  // The function is used to set sensor's add
  //digitalWrite(TriggerPin, HIGH);
  cmdst[0]=0x55;  
  cmdst[1]=0xaa;
  cmdst[2] = 0xab;
  cmdst[3] = 0x01;
  cmdst[4] = 0x55;
  cmdst[5] = 0x14;
  transmitCommands_setAdd();
}

void transmitCommands_setAdd(){  // Send protocol via RS485 interface
  cmdst[6]=cmdst[0]+cmdst[1]+cmdst[2]+cmdst[3]+cmdst[4]+cmdst[5];
  delay(1);
  for(int j = 0; j < 7; j++){
    printByte(cmdst[j]);
  }
  delay(2);
}


void  urmTrigger(byte addr){  // The function is used to trigger the measuring
  cmdst[0]=0x55;  
  cmdst[1]=0xaa;
  cmdst[2] = addr;
  cmdst[3] = 0x00;
  cmdst[4] = 0x01;
  transmitCommands();
}

void urmReader(byte addr){  // The function is used to read the distance
  cmdst[0]=0x55;  
  cmdst[1]=0xaa;
  cmdst[2] = addr;
  cmdst[3]=0x00;
  cmdst[4]=0x02;
  transmitCommands();
}

void transmitCommands(){  // Send protocol via RS485 interface
  cmdst[5]=cmdst[0]+cmdst[1]+cmdst[2]+cmdst[3]+cmdst[4];
  delay(1);
  for(int j = 0; j < 6; j++){
    printByte(cmdst[j]);
  }
  delay(2);
}

void analyzeUrmData(byte cmd[],int addr ){   
  //SerialPort.print("distance:");
  byte sumCheck = 0;
  for(int h = 0;h < 7; h ++)  sumCheck += cmd[h];
   
  if(sumCheck == cmd[7] && cmd[3] == 2 && cmd[4] == 2){     
     urmData[addr] = cmd[5] * 256 + cmd[6];     
  }
  else if(cmd[3] == 2 && cmd[4] == 2){
    SerialPort.print("Sum error");
  }   
}
void decodeURM4(int addr){
 // mySerial.println(addr);
 if(SerialPort.available()){
  byte cmdrd[10];
  for(int i = 0 ;i < 10; i++)  cmdrd[i] = 0;
  
       // SerialPort.println("get dis");
  boolean flag = true;
  boolean valid = false;
  byte headerNo = 0;
  
  int i=0;
  int RetryCounter = 0;
  while(true)
  {   //timer = millis();   
      if(SerialPort.available()){
        cmdrd[i]= SerialPort.read();
        
        //SerialPort.println(cmdrd[i]);
        //SerialPort.print(" ");
        //SerialPort.print(timer);
        i++;
        
      }
      else{
        
        //SerialPort.println("not get dis");
        if(cmdrd[1] == 0xAA)
          break;
        else
        { 
          //SerialPort.print(cmdrd[1]);      
          return;
        }
      }
    }
    //SerialPort.println("");
    analyzeUrmData(cmdrd,addr);   
    //mySerial.println( urmData[addr]);
    //SerialPort.print(urmData[0]);
    //SerialPort.print(" ");
    //SerialPort.println(urmData[1]);
  }
  else
  {
    urmData[addr] = 9999;
       // SerialPort.println("not get dis");
    }
}

void updateDis()
{
  for(int i=0;i<urmAccount;i++)
  {
    //timers =  millis();
    digitalWrite(TriggerPin, HIGH); 
    urmTrigger(urmID[i]);
    delay(40);
    urmReader(urmID[i]);
    digitalWrite(TriggerPin, LOW);
    delay(10);
    decodeURM4(i);
    
    //timere =  millis() - timers; 
    //SerialPort.print(urmData[i]);
    //SerialPort.print(" ");
    //SerialPort.println(timere);
  }

  sensorLF = urmData[0];
  if(sensorLF>300)sensorLF = 9999;
  sensorLR = urmData[1];
  if(sensorLR>300)sensorLR = 9999;
  /*
  sensorF = urmData[2];
  if(sensorF>300)sensorF = 9999;
  sensorRF = urmData[3];
  if(sensorRF>300)sensorRF = 9999;
  sensorRR = urmData[4];
  if(sensorRR>300)sensorRR = 9999;
  */
    //SerialPort.print(sensorLF);
    //SerialPort.print(" ");
    //SerialPort.println(sensorLR);

  //sensorRF =  rand() % 255+1;
  //sensorRR =  rand() % 255+1;
}

void loop() {
  // put your main code here, to run repeatedly:
if (mySerial.available()) {  
    str = mySerial.readString();
    
    //mySerial.print(str);
    if(str=="0")
    {
      startCheck();     
    }
    else
    {    
      updateDis();
      result = result + sensorLF + ";"+ sensorLR+";"+sensorRF+";"+sensorRR;//+";"+sensorF;    
      //SerialPort.println(result);
      mySerial.println(result);
      //SerialPort.println(result);
      result = "";     
      
      //setAddr();
      //mySerial.println("ok");
    }
   
    //delay(20);
    //updateDis();
  }
  //updateDis();
}

void startCheck()
{
  mySerial.println("sensor"); 
}
