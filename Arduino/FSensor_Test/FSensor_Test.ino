# include<DFSensor.h>

DFSensor sensorLF(8,13,A1,0);//0 is useing analog mode! Sensor(int URTRIG,int URECHO,uint8_t sensorPin,int Mode);
DFSensor sensorLR(8,0,A3,0);
DFSensor sensorRF(8,11,A0,0);
DFSensor sensorRR(8,12,A2,0);

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);          
  sensorLF.start();
  sensorLR.start();          
  sensorRF.start();
  sensorRR.start();
  delay(500);
  Serial.println("Init the sensor"); 
 }

void loop() {

  sensorLR.trig();
  
  // put your main code here, to run repeatedly:
  int dis_L_front = sensorLF.echo();
  int dis_L_rear = sensorLR.echo();
  int dis_R_front = sensorRF.echo();
  int dis_R_rear = sensorRR.echo();
  Serial.print("LF: ");  
  Serial.print(dis_L_front);
  Serial.print("    LR: ");
  Serial.print(dis_L_rear);
  Serial.print("    RF: ");
  Serial.print(dis_R_front);
  Serial.print("    RR: ");       
  Serial.println(dis_R_rear);  
  delay(200);
}
