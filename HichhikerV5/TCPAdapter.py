import socket
import fcntl
import struct

class TCPAdapter:
    def __init__(self,port):
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.IP = socket.inet_ntoa(fcntl.ioctl(
        s.fileno(),
        0x8915,  # SIOCGIFADDR
        struct.pack('256s', 'wlan0'[:15]))[20:24])
        self.PORT = port
        print self.IP," ",self.PORT
        print("Adapter ready!")

    def ConnStart(self):
        self.Adapter = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.Adapter.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.Adapter.bind((self.IP, self.PORT))
        self.Adapter.listen(1)

    def ConnClose(self):
        self.conn.close()

    def Recv(self):
        self.conn,self.addr = self.Adapter.accept()
        BUFFER_SIZE = 1024
        return self.conn.recv(BUFFER_SIZE)

    def Send(self,msg):
        self.conn.send(msg)
