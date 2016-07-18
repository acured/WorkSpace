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
        public void Dispose()
        {
            _device.Dispose();
        }

        public bool Check()
        {
            try
            {
                byte[] cmds = Cmds.cmd_Bit();
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if (recv[5] == 0x00 && recv[6] == 0x00)
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if (recv[7] == 0x01)
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return null;

                int index = 7;
                List<int> data = new List<int>();
                //int freq = BitConverter.ToInt32(new byte[]{recv[index],recv[index+1],recv[index+2],recv[index+3]}, 0);// recv[index] * 1000000 + recv[index + 1] * 10000 + recv[index + 2] * 100 + recv[index + 3];
                int freq = recv[index] * 256 * 256 * 256 + recv[index+1] * 256 * 256 + recv[index+2] * 256 + recv[index+3];
                int sf = recv[index + 4];
                int bw = recv[index + 5];
                int rc = recv[index + 6];
                int check = recv[index+7];
                int enc = recv[index+8];
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if(recv[8]==length)
                    return true;
                return false;
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return 0;

                int h = recv[7];
                int l = recv[8];
                return h * 256 + l;
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
                byte[] recv = _device.sp_DataSender(cmds);

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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;
                if (recv[7] == 0x01) return true;//count ,id1,id1,id2,id2....
                return false;
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;
                if (recv[7] == 0x01) return true;//count ,id1,id1,id2,id2....
                return false;
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return null;

                int index = 7;

                double _id = recv[index] * 100 + recv[index + 1];
                double status = recv[index+2] * 256 + recv[index + 3];
                double temp = recv[index + 4] * 256 + recv[index + 5];
                double moist = recv[index + 6] * 256 + recv[index + 7];
                double CH3 = recv[index + 8] * 256 + recv[index + 9];
                double data1 = recv[index + 10] * 256 + recv[index + 11];
                double data2 = recv[index + 12] * 256 + recv[index + 13];
                double data3 = recv[index + 14] * 256 + recv[index + 15];

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
                byte[] recv  = _device.sp_DataSender(cmds);

                if (recv == null) return null;
                int count = (recv.Length - 2) / 16;

                for (int i = 0; i < count; i++)
                {
                    int index = i*16 + 7;

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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return null;

                int index = 7;

                double _id = recv[index] * 100 + recv[index + 1];
                double res1 = recv[index + 2] * 256 + recv[index + 3];
                double res2 = recv[index + 4] * 256 + recv[index + 5];
                double data1 = recv[index + 6] * 256 + recv[index + 7];

                double[] data = { _id, res1, res2, data1 };

                return data;
            }
            catch
            {
                return null;
            }
        }
        public double[] getNodeList()
        {
            try
            {
                byte[] cmds = Cmds.cmd_ReadAllNodes();
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return null;

                int index = 7;

                int count = recv[index] * 256 + recv[index + 1];
                double[] data = new double[count];
                for (int i = 0; i < count; i++)
                {
                    data[i] = recv[index + 2 + i*2] * 100 + recv[index + 3 + i*2];
                }
                return data;
            }
            catch
            {
                return null;
            }
        }
        public bool DeleteNodeList()
        {
            try
            {
                byte[] cmds = Cmds.cmd_CleanAllNodes();
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if (recv[7] == 0x01) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        public List<double[]> getAllNodeDebug()
        {
            try
            {
                List<double[]> datas = new List<double[]>();

                byte[] cmds = Cmds.cmd_ReadAllNodeDebug();
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return null;
                int count = (recv.Length - 2) / 8;

                for (int i = 0; i < count; i++)
                {
                    int index = i * 8 + 7;

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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if (recv[7] == 0x01)
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if (recv[7] == 0x01)
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if (recv[7] == 0x01)
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
                byte[] recv = _device.sp_DataSender(cmds);

                if (recv == null) return false;

                if (recv[7] == 0x01)
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
