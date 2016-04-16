#include <SoftwareSerial.h>
SoftwareSerial mySerial1(3, 5); 
SoftwareSerial mySerial2(6 ,9);
SoftwareSerial mySerial3(10,11);
void setup() {
  // put your setup code here, to run once:
Serial.begin(57600);
while(!Serial){}

 Serial.println("11111");

   // set the data rate for the SoftwareSerial port
   mySerial1.begin(4800);
    Serial.println("1");
   mySerial2.begin(9600);
    Serial.println("2");
   mySerial3.begin(115200);
    Serial.println("3");
  mySerial1.println("111111");
}

void loop() { // run over and over
   if (mySerial1.available()) {
     Serial.write(mySerial1.read());
      Serial.println("myserial1.read");
   }
    if (mySerial2.available()) {
     Serial.write(mySerial2.read());
      Serial.println("myserial2.read");
   }
    if (mySerial3.available()) {
     Serial.write(mySerial3.read());
      Serial.println("myserial3.read");
   }
   if (Serial.available()) {
     mySerial1.write(Serial.read());
     Serial.println("serial.read");
   }
}
