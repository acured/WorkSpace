// # Editor     : ZRH from DFRobot
// # Date       : 29.08.2014
 
// # Product name: URM V4.0 ultrasonic sensor
// # Product SKU : SEN0001
// # Version     : 1.0
 
// # Description:
// # The sketch for using the URM37 Serial  mode from DFRobot  
// #   and writes the values to the serialport
 
// # Connection:
// #       Vcc (Arduino)      -> Pin 1 VCC (URM V4.0)
// #       GND (Arduino)      -> Pin 2 GND (URM V4.0)
// #       Pin TX1 (Arduino)  -> Pin 8 RXD (URM V4.0)
// #       Pin RX0 (Arduino)  -> Pin 9 TXD (URM V4.0)
// # Working Mode: Serial  Mode.
 
uint8_t EnTempCmd[4]={0x11,0x00,0x00,0x11};    // temperature measure command
uint8_t TempData[4];
unsigned int TempValue=0;
void setup()
{
  Serial.begin(9600);
  delay(100);
  Serial.println("Init the sensor");
}
void loop()
{
  SerialCmd();
  delay(200);
}
void SerialCmd()
{
   int i;
   for(i = 0;i < 4;i++){
      Serial.write(EnTempCmd[i]);
   }
    while (Serial.available() > 0)  //if serial receive any data
    {
       for(i = 0;i < 4;i++){
         TempData[i] = Serial.read();
       }
       TempValue = TempData[1]<<8;
       TempValue =TempValue+TempData[2];
       Serial.print("temperature : "); 
       Serial.print(TempValue,DEC);  
       Serial.println(" oC");
    }
}
