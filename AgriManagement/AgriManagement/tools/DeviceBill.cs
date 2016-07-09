using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgriManagement.tools
{
    class DeviceBill
    {
        DeviceAdapter _device = new DeviceAdapter();
        public DeviceBill(DeviceAdapter device)
        {
            _device = device;
        }

        public bool Check()
        {
            try
            {
                byte[] cmds = Cmds.cmd_Bit();
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                if (recv[0] == 0x00 && recv[1] == 0x00)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool setBS(int fre, int sf, int bw, int cr, int che, int enc)
        {
            try
            {
                byte[] cmds = Cmds.cmd_SetBS(fre, sf, bw, cr, che, enc);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                if (recv[2] == 0x01)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public List<int> getBS()
        {
            try
            {
                byte[] cmds = Cmds.cmd_ReadBS();
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return null;

                List<int> data = new List<int>();
                int freq = recv[0] * 1000000 + recv[1] * 10000 + recv[2] * 100 + recv[3];
                int bw = recv[4];
                int sf = recv[5];
                int rc = recv[6];
                int check = recv[7];
                int enc = recv[8];
                data.Add(freq);
                data.Add(sf);
                data.Add(bw);
                data.Add(rc);
                data.Add(check);
                data.Add(enc);

                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool setBSLength(int length)
        {
            try
            {
                byte[] cmds = Cmds.cmd_SetNodesCount(length);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public int getBSLength()
        {
            try
            {
                byte[] cmds = Cmds.cmd_ReadNodesCount();
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return 0;

                int h = recv[2];
                int l = recv[3];
                return h * 100 + l;
            }
            catch
            {
                return 0;
            }
        }
        public List<int> getBSList()
        {
            try
            {
                List<int> data = new List<int>();
                byte[] cmds = Cmds.cmd_ReadNodesCount();
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return null;

                int h = recv[2];
                int l = recv[3];
                int length = h * 100 + l;

                for (int i = 0; i < length; i++)
                {
                    h = recv[4+i];
                    l = recv[5+i];
                    int id = h * 100 + l;
                    data.Add(id);
                }

                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool AddBSList(List<int> list)
        {
            try
            {
                List<int> data = new List<int>();
                byte[] cmds = Cmds.cmd_AddNodes(list);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool DeleteBSList(List<int> list)
        {
            try
            {
                List<int> data = new List<int>();
                byte[] cmds = Cmds.cmd_DeleteNodes(list);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public double[] getNodeData(int id)
        {
            try
            {
                byte[] cmds = Cmds.cmd_ReadNodeData(id);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return null;

                int index = 2;

                double _id = recv[index] * 100 + recv[index + 1];
                double status = recv[index+2] * 100 + recv[index + 3];
                double temp = recv[index + 4] * 100 + recv[index + 5];
                double moist = recv[index + 6] * 100 + recv[index + 7];
                double CH3 = recv[index + 8] * 100 + recv[index + 9];
                double data1 = recv[index + 10] * 100 + recv[index + 11];
                double data2 = recv[index + 12] * 100 + recv[index + 13];
                double data3 = recv[index + 14] * 100 + recv[index + 15];

                double[] data = { _id, status, temp, moist, CH3, data1, data2, data3 };

                return data;
            }
            catch
            {
                return null;
            }
        }
        public List<double[]> getAllNodeData()
        {
            try
            {
                List<double[]> datas = new List<double[]>();
                byte[] cmds = Cmds.cmd_ReadAllNodeData();
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return null;
                int count = (recv.Length - 2) / 16;

                for (int i = 0; i < count; i++)
                {
                    int index = i*16 + 2;

                    double _id = recv[index] * 100 + recv[index + 1];
                    double status = recv[index + 2] * 100 + recv[index + 3];
                    double temp = recv[index + 4] * 100 + recv[index + 5];
                    double moist = recv[index + 6] * 100 + recv[index + 7];
                    double CH3 = recv[index + 8] * 100 + recv[index + 9];
                    double data1 = recv[index + 10] * 100 + recv[index + 11];
                    double data2 = recv[index + 12] * 100 + recv[index + 13];
                    double data3 = recv[index + 14] * 100 + recv[index + 15];

                    double[] data = { _id, status, temp, moist, CH3, data1, data2, data3 };

                    datas.Add(data);
                }

                return datas;
            }
            catch
            {
                return null;
            }
        }
        public double[] getNodeDebug(int id)
        {
            try
            {
                byte[] cmds = Cmds.cmd_ReadNodeDebug(id);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return null;

                int index = 2;

                double _id = recv[index] * 100 + recv[index + 1];
                double res1 = recv[index + 2] * 100 + recv[index + 3];
                double res2 = recv[index + 4] * 100 + recv[index + 5];
                double data1 = recv[index + 6] * 100 + recv[index + 7];

                double[] data = { _id, res1, res2, data1 };

                return data;
            }
            catch
            {
                return null;
            }
        }
        public List<double[]> getAllNodeDebug()
        {
            try
            {
                List<double[]> datas = new List<double[]>();

                byte[] cmds = Cmds.cmd_ReadAllNodeDebug();
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return null;
                int count = (recv.Length - 2) / 8;

                for (int i = 0; i < count; i++)
                {
                    int index = i * 8 + 2;

                    double _id = recv[index] * 100 + recv[index + 1];
                    double res1 = recv[index + 2] * 100 + recv[index + 3];
                    double res2 = recv[index + 4] * 100 + recv[index + 5];
                    double data1 = recv[index + 6] * 100 + recv[index + 7];

                    double[] data = { _id, res1, res2, data1 };
                    datas.Add(data);
                }
                return datas;
            }
            catch
            {
                return null;
            }
        }
        public bool UpdateNodeId(int oldId,int newId)
        {
            try
            {
                List<int> data = new List<int>();
                byte[] cmds = Cmds.cmd_UpdateNodeID(oldId,newId);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                if (recv[2] == 0x01)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool setBSToNode(int id, int fre, int sf, int bw, int cr, int che, int enc)
        {
            try
            {
                byte[] cmds = Cmds.cmd_SetNodeBS(id, fre, sf, bw, cr, che, enc);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                if (recv[2] == 0x01)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool setBSToNode(int oldId, int newId, int fre, int sf, int bw, int cr, int che, int enc)
        {
            try
            {
                byte[] cmds = Cmds.cmd_SetNodeBS(oldId, newId, fre, sf, bw, cr, che, enc);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                if (recv[2] == 0x01)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public bool setNodeRom(int id, int addr, List<byte> datas)
        {
            try
            {
                byte[] cmds = Cmds.cmd_SetNodeRom(id, addr, datas);
                _device.sp_DataSender(cmds);
                Thread.Sleep(1000);
                byte[] recv = _device.sp_read();

                if (recv == null) return false;

                if (recv[2] == 0x01)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }












    }
}
