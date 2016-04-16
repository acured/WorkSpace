from picamera.array import PiRGBArray
from LineHelper import LineHelper
from picamera import PiCamera
import time
import numpy as np
import cv2
import socket
import fcntl
import struct
import io
from thread import *

class TCPCameraAdapter:
    def __init__(self,port):
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.IP = socket.inet_ntoa(fcntl.ioctl(
        s.fileno(),
        0x8915,  # SIOCGIFADDR
        struct.pack('256s', 'wlan0'[:15]))[20:24])
        self.PORT = port
        self.image = ''
        print self.IP," ",self.PORT
        print("Camera Adapter ready!")

    def picamerathread(self,width,height):
        camera = PiCamera()
        camera.resolution = (width, height)
        camera.framerate = 32
        rawCapture = PiRGBArray(camera, size=(width, height))
        time.sleep(0.1)
        for frame in camera.capture_continuous(rawCapture, format="bgr", use_video_port=True):
            self.image = frame.array
            rawCapture.truncate(0)

    def ConnStart(self):
        self.Adapter = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.Adapter.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.Adapter.bind((self.IP, self.PORT))
        self.Adapter.listen(1)
        start_new_thread(self.picamerathread,(640,480))

    def DoListen(self):
	#data = ""
        while True:
            try:
                conn,addr = self.Adapter.accept()		
		#data = np.array(self.image)
		#a=LineHelper(data)
		#d = a.FindCenter()		
               	encode_param=[int(cv2.IMWRITE_JPEG_QUALITY),90]
                result,temp=cv2.imencode('.jpg',self.image,encode_param)
                
		data1 = np.array(temp)
		
		#data = np.array(self.image)
                stringData=data1.tostring()
                conn.send(str(len(data1)).ljust(16)+'\n')
                #conn.send(stringData+'\n')
                conn.send(data1)
                conn.close()
            except:
                print 'error'
                
    def GetFrameCMD(self,w):
        #count = 0
        while True:
            if(w.stutas == "passGap"):
                #get frame cmd L/R/G

                #encode_param=[int(cv2.IMWRITE_JPEG_QUALITY),90]
                #result,temp=cv2.imencode('.jpg',self.image,encode_param)
                data=np.array(self.image)
                a=LineHelper(data)
		#self.image = a.image
                dire = a.FindCenter()
                #print dire
                #cv2.imshow('result',a.image)
                #cv2.waitKey(0)
                w.Dir = dire
                print w.Dir
		#if(count>100):
                #    w.cmd = "stop"
                #    count = 0
                #count = count+1
		del a
                #print(count)
	    time.sleep(0.1)
        
