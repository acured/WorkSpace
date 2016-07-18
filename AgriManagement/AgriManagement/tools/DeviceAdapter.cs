using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgriManagement.tools
{
    class DeviceAdapter
    {
        SerialPort p;
        string portName = "";
        int baudRate = 115200;
        Random ran = new Random();
        public bool canRead = false;

        

        public void setPortName(string name)
        {
            this.portName = name;
        }
        public void Dispose()
        {
            p.Close();
            p.Dispose();
        }
        public string connect()
        {
            for(int i=0;i<15;i++)
            {
                try
                {
                    p = new SerialPort();

                    p.PortName = "COM"+i;
                    p.BaudRate = baudRate;

                    p.StopBits = StopBits.One;
                    p.DataBits = 8;
                    p.Parity = Parity.None;
                    //p.BaudRate = baudRate;
                    p.ReceivedBytesThreshold = 1;

                    p.Open();

                    byte[] cmds = Cmds.cmd_Bit();
                    byte[] recv = this.sp_DataSender(cmds);

                    if (recv != null && recv[5] == 0x00 && recv[6] == 0x00)
                    {
                        portName = p.PortName;
                        return "连接成功！";
                    }
                }
                catch
                {
                    Console.WriteLine("failt to open!");
                }

                Thread.Sleep(10);
            }
            portName = "";
            return "连接失败！";
        }

        public string reconnect()
        {
            if (portName == "")
                return null;
            try
            {
                p = new SerialPort();

                p.PortName = portName;
                p.BaudRate = baudRate;

                p.StopBits = StopBits.One;
                p.DataBits = 8;
                p.Parity = Parity.None;
                //p.BaudRate = baudRate;
                p.ReceivedBytesThreshold = 1;

                p.Open();

                byte[] cmds = Cmds.cmd_Bit();
                byte[] recv = this.sp_DataSender(cmds);

                if (recv != null && recv[5] == 0x00 && recv[6] == 0x00)
                    return "连接成功！";
            }
            catch
            {
                Console.WriteLine("failt to open!");
            }
            return "连接失败！";
        }

        public byte[] sp_DataSender(byte[] cmd)
        {
            try
            {
                if (p == null)
                {
                    if (null == reconnect())
                        return null; 
                }
                if (p.IsOpen)
                {
                    int length = cmd.Length;
                    p.Write(cmd, 0, length);
                }
                else
                {
                    Console.WriteLine("port is close");
                    p.Open();

                    int length = cmd.Length;
                    p.Write(cmd, 0, length);
                }
                int count = 0;
                while (true)
                {
                    Thread.Sleep(50);
                    count++;
                    if (p.BytesToRead > 0)
                    {
                        int bytes = p.BytesToRead;
                        byte[] buffer = new byte[bytes];
                        p.Read(buffer, 0, bytes);
                        return buffer;
                    }
                    if (count > 20) return null;
                }
            }
            catch (Exception ex)
            {
                if (null == reconnect())
                    return null;
            }
            return null;
        }        

        int tempcout = 0;
        public bool updateSensorData(string id,ref History history)
        {
            if (id == "0"&& tempcout<10)
            {
                tempcout++;
                history.id = Convert.ToInt32(id);
                history.time = DateTime.Now;
                history.status = Status.Error;
                return false;
            }
            history.id = Convert.ToInt32(id);
            history.time = DateTime.Now;
            history.moisture = ran.Next(100);
            history.temperature = ran.Next(100);
            history.NH = ran.Next(100);
            history.status = Status.Regular;
            return true;
        }
    }
}
