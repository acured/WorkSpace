import time
import thread
import sys
from devicedriver import devicedriver
from TCPAdapter import TCPAdapter
from TCPCameraAdapter import TCPCameraAdapter
from worker import worker

#main code

#TCP_IP = '10.172.42.160'
print "app start!!!!"
time.sleep(1)

d = devicedriver()
d.checkDevice()

w = worker()

adapter = TCPAdapter(5005)
adapter.ConnStart()

reporter = TCPAdapter(5000)
reporter.ConnStart()

Cadapter = TCPCameraAdapter(5555)
Cadapter.ConnStart()

#thread.start_new_thread( Cadapter.DoListen,())
thread.start_new_thread( Cadapter.GetFrameCMD,(w,))
print 'app started!'

Start=True
msgRecv=""
msgSend="stop"

print "end initialize, start listen..."

def Report():
    while 1:
        try:
            msg = reporter.Recv()
            #print "------------------start report-------------------"
            #print "received data:", msg,w.cmd
            w.cmd = msg
            retmsg = w.doWork("GetDIS",d) + ";" + w.stutas
            #do some logic
            #print retmsg
            reporter.Send(retmsg)
            #print "-------------------end report--------------------"
        except:
            print "Unexpected error:", sys.exc_info()[0]
            break

def UpdateDis():
    while 1:
        try:
            d.updateDistance()
            time.sleep(0.05)
        except:
            print "Unexpected error:", sys.exc_info()[0]
            break

thread.start_new_thread(Report,())
thread.start_new_thread(UpdateDis,())

while 1:
    try:
        print "**********************************************"
        msgRecv = adapter.Recv()
        print "received data:", msgRecv

        #stop
        if(msgRecv=="Stop"):
            Start=False
            msgSend="Stop"

        #exit
        if(msgRecv=="q"):
            print "Exit!!"
            break
            
        #do work   
        if(Start):
            thread.start_new_thread(w.doWork,(msgRecv,d))
            #msgSend = w.doWork(msgRecv,d)

        #start
        if(msgRecv=="Start"):
            Start=True
            msgSend="Start"
    except:
        print "Unexpected error:", sys.exc_info()[0]
        break
adapter.ConnClose()
