using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriManagement.tools
{
    class DeviceAdapter
    {
        SerialPort p;
        string portName = "COM9";
        int baudRate = 115200;
        Random ran = new Random();
        public bool canRead = false;

        public void setPortName(string name)
        {
            this.portName = name;
        }
        public void connect()
        {
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


                p.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            }
            catch (System.Exception ex)
            {
            }
        }

        public void sp_DataSender(byte[] cmd)
        {
            try
            {
                if (p == null)
                {
                    connect();
                }
                if (p.IsOpen)
                {
                    int length = cmd.Length;
                    p.Write(cmd, 0, length);
                    canRead = false;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                connect();
            }
        }

        public byte[] sp_read()
        {
            try
            {
                if (canRead)
                {
                    int bytes = p.BytesToRead;
                    byte[] buffer = new byte[bytes];
                    p.Read(buffer, 0, bytes);

                    return buffer;
                }
                else return null;
            }
            catch (Exception ex)
            {
                return null;
            }
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

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            canRead = true;
        }
    }
}
