import cv2
import numpy as np

class LineHelper:
    
    def __init__(self,img):
        self.image=img[200:480,0:640]
        self.kernel = np.ones((5,5),np.uint8)
        self.height,self.width=img.shape[:2]
        self.msg=''
    def __FindHoughAvg(self):
        self.image=cv2.dilate(self.image,self.kernel,iterations=1)
        self.image=cv2.erode(self.image,self.kernel,iterations=1)
        canny=cv2.Canny(self.image,50,150)
        lines=cv2.HoughLines(canny,1,np.pi/180,60)
        posk=posb=poscount=0
        negk=negb=negcount=0
        if lines==None:
            self.msg='None'
            return
        for line in lines:
            for rho,theta in line:
                err=int(360*(theta/(2*np.pi)))
                if (err>20 and err<80)or(err>90 and err<130):
                    a = np.cos(theta)
                    b =np.sin(theta)
                    x0 = a*rho
                    y0 = b*rho
                    x2 = int(x0 - 1000*(-b))
                    y2 = int(y0 - 1000*(a))
                    lk=-1/np.tan(theta)
                    lb=y2-lk*x2

                if (err>20 and err<80):
                    posk+=lk
                    posb+=lb
                    poscount+=1
                    cv2.line(self.image,(0,int(lb)),(self.width,int(self.width*lk+lb)),(0,0,255),1)
                elif (err>90 and err<130):
                    negk+=lk
                    negb+=lb
                    negcount+=1
                    cv2.line(self.image,(0,int(lb)),(self.width,int(self.width*lk+lb)),(0,0,255),1)
        if(poscount==0):
            self.msg='left'
            return
        if(negcount==0):
            self.msg='right'
            return
        self.avgposk=posk/poscount
        self.avgposb=posb/poscount
        self.avgnegk=negk/negcount
        self.avgnegb=negb/negcount
        cv2.line(self.image,(0,int(self.avgposb)),(self.width,int(self.width*self.avgposk+self.avgposb)),(0,255,255),3)
        cv2.line(self.image,(0,int(self.avgnegb)),(self.width,int(self.width*self.avgnegk+self.avgnegb)),(0,255,255),3)
        self.ix,self.iy=self.__Intersection(self.avgposk,self.avgnegk,self.avgposb,self.avgnegb)
        cv2.circle(self.image,(self.ix,self.iy),10,(255,255,0),-1)
        tempy=self.height
        for tempx in range(0,self.width):
            a=self.GetDist(tempx,tempy,self.avgposk,self.avgposb)
            b=self.GetDist(tempx,tempy,self.avgnegk,self.avgnegb)
            if abs(a-b)<1:
                break
        cv2.line(self.image,(self.ix,self.iy),(tempx,tempy),(0,255,255),3)
        if tempx<self.width/3:
            self.msg='left'
        elif tempx>self.width*2/3:
            self.msg='right'
        else:
            self.msg='go'
    #private funcitons
    def __Intersection(self,k1,k2,b1,b2):
        if k1==k2 :
            return 0,0
        x=int((b2-b1)/(k1-k2))
        y=int(k1*x+b1)
        return x,y
    def GetDist(self,x,y,k,b):
        a=abs(k*x+b-y)
        b=np.sqrt(1+k*k)
        return a/b
    def FindCenter(self):
        self.__FindHoughAvg()
        return self.msg

        













