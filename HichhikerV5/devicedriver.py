import serial
import os
import time

class devicedriver:
    def __init__(self):        
        self.motorDevice=''
        self.sensorDevice=''
        self.Dis = [9999.0,9999.0,9999.0,9999.0]
        self.isReady = False
        
    def checkDevice(self):
        ser = []
        parked=[]
        checkdata=bytes(0x00)
        print("device initialize....")
        for vtr in range(0,12):
            if os.path.exists('/dev/ttyUSB'+str(vtr)):
                ser.append(serial.Serial('/dev/ttyUSB'+str(vtr), 9600, timeout=0.1, parity=serial.PARITY_ODD, stopbits=1))
                parked.append(vtr+1)

        for s in ser:
            while True:
                s.write(checkdata)
                time.sleep(1)
                response=s.readline()
                if(response=="motor\r\n"):
                    self.motorDevice = s
                    print("Motor driver ready!")
                if(response=="sensor\r\n"):
                    self.sensorDevice = s
                    print("Sensor driver ready!")
                break;

        if(self.motorDevice<>'' and self.sensorDevice<>''):
            print("all driver are connected!")
            isReady = True
        else:
            if(self.sensorDevice==''):
                print("Sensor connecte fault!")
            if(self.motorDevice==''):
                print("Motor connecte fault!")
            isReady = False
                    
    def updateDistance(self):
        if(self.sensorDevice==''):
            print("Sensor connecte fault!")
            return
        cmdGetDis=bytes([0x01])
        while True:
            self.sensorDevice.write(cmdGetDis)
	    #time.sleep(0.01)
            result = self.sensorDevice.readline()
	    #print(result)
            if(result==""):
                result = self.sensorDevice.readline()
		#print(result)
            response=result.split('\r',1)[0]
	    #print "hahaha"
            dis = response.split(';', 4 )
            #print(response)
	    self.Dis[0] = float(dis[0])
            self.Dis[1] = float(dis[1])
            self.Dis[2] = float(dis[2])
            self.Dis[3] = float(dis[3])
            break

    def setspeed(self,dirl,dirr,l,r):
        if(self.motorDevice==''):
            #print("Motor connecte fault!")
            return        
        message = ''
        if(dirl):
            message = str(1)
        else:
            message = str(0)

        if(dirr):
            message = message + str(1)
        else:
            message = message + str(0)

        if(l>99):
            l=99
        if(l<0):
            l=0

        if(r>95):
            r=95
        if(r<0):
            r=0
        #print(l,r)
        #if(r>55):
        #    r = r - 5
        strl = str(l)
        strr = str(r)
        if(l<10):
            strl = "0"+strl
        if(r<10):
            strr = "0"+strr
        #print(strl,strr)
        message = message+strl+strr
        #print(message)
        self.motorDevice.write(message.encode())
	response = self.motorDevice.readline()
	while(response==""):
	    response = self.motorDevice.readline()
	#print(response)        

    def startwallfollow(self):
        if(self.motorDevice==''):
            print("Motor connecte fault!")
            return        
        message = 'wallfollow'        
        self.motorDevice.write(message.encode())

    def startGo(self):
        if(self.motorDevice==''):
            print("Motor connecte fault!")
            return        
        message = 'go'        
        self.motorDevice.write(message.encode())

    def startturn(self):
        if(self.motorDevice==''):
            print("Motor connecte fault!")
            return        
        message = 'turn'        
        self.motorDevice.write(message.encode())

    def stopCMD(self):
        if(self.motorDevice==''):
            print("Motor connecte fault!")
            return        
        message = 'done'        
        self.motorDevice.write(message.encode())    
        
    def stop(self):
        if(self.motorDevice==''):
            print("Motor connecte fault!")
            return        
        message = '000000'        
        self.motorDevice.write(message.encode())
        #self.motorDevice.flush()
