using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriManagement.tools
{
    public static class Cmds
    {
        public static byte[] testCmd = { 0x02, 0x02 };
        //帧头
        public static byte[] Head = { 0xAA, 0xA5, 0x55 };
        //检测
        public static byte[] Check = {0x00, 0x02, 0x00, 0x00 };        
        //设置BS时间
        public static byte[] SetTime = { 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 };//m d h m s
        //读取BS时间                                                                //读取BS时间
        public static byte[] ReadTime = { 0x00, 0x02, 0x01, 0x02 };
        //设置BS RF
        public static byte[] SetRF = { 0x01, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//Freq	SF	BW	CR	Optimize	RF Power
        //读取BS RF
        public static byte[] ReadRF = { 0x00, 0x02, 0x01, 0x04 };
        //设置BS端的节点列表最大数量
        public static byte[] SetMaxNodes = { 0x01, 0x05, 0x00, 0x00 };
        //增加BS端的节点列表中的新节点
        public static byte[] AddNodes = { 0x01, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//AddLength	1st Node ID	2nd Node ID	… …	10th Node ID
        //删除BS端的节点列表中的节点
        public static byte[] DelNodes = { 0x01, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//DelLength	1st Node ID	2nd Node ID	… …	10th Node ID
        //读取BS端的节点列表中的所有节点ID
        public static byte[] ReadNodes = { 0x01, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//ListLength	1st Node ID	2nd Node ID	… …	nth Node ID
        //读取BS端的节点列表中的节点总数
        public static byte[] ReadNodeSum = { 0x01, 0x09, 0x00, 0x00 };//ListLength
        //读取BS端的中某一节点的工作数据
        public static byte[] ReadNodeData = { 0x01, 0x09, 0x00, 0x00 };//Node ID
        
        //读取BS端的中某一节点的调试数据
        public static byte[] ReadNodeDataDebug = { 0x02, 0x03, 0x00, 0x00 };//Node ID //Node ID	Node End RSSI	BS End RSSI	Reserved1
        //读取BS端的中所有节点的调试数据
        public static byte[] ReadAllNodeDataDebug = { 0x02, 0x04 };//Node ID//1st Node Work Data Block	2nd Node Work Data Block	… …	Nth Node Work Data Block//Node ID	State	Temperature	Humidity	NH4	Reserved1	Reserv2ed2	Reserved3
        //通过BS设置节点的ID
        public static byte[] SetNodeID = { 0x03, 0x01, 0x00, 0x00, 0x00, 0x00 };//original id new ID
        //通过BS设置节点的RF设置
        public static byte[] SetNodeRF = { 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//Original ID	Freq	SF	BW	CR	Optimize	RF Power
        //通过BS配置节点
        public static byte[] SetConfigNode = { 0x03, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//Original ID	New ID	Freq	SF	BW	CR	Optimize	RF Power
        


        public static byte[] ReadModelSetting = { 0x00, 0x02 };
        //读取BS端的中所有节点的工作数据
        public static byte[] ReadAllNodeData = { 0x00, 0x02, 0x02, 0x02, 0x45, 0x21 };//Node ID//1st Node Work Data Block	2nd Node Work Data Block	… …	Nth Node Work Data Block//Node ID	State	Temperature	Humidity	NH4	Reserved1	Reserv2ed2	Reserved3
        //通过BS配置节点寄存器
        public static byte[] SetNodeRegister = { 0x03, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//Node ID	Reg Addr	Length of Byte	Byte1	Byte2	… …	Byte16
        //satrt up fan
        public static byte[] SetFanStart = { 0x00, 0x07, 0x03, 0x04, 0x02, 0x02, 0x40, 0x01, 0x80 };//,0x0E,0x00 };//id : 0202
        //stop fan
        public static byte[] SetFanStop = { 0x00, 0x07, 0x03, 0x04, 0x02, 0x02, 0x40, 0x01, 0x00 };//,0x0E,0x00 };//id : 0202
        public static byte[] cmd_CmdStartFanByID(int group, int member)
        {
            byte g = Convert.ToByte(group);
            byte m = Convert.ToByte(member);

            byte[] temp = SetFanStart;
            temp[4] = g;
            temp[5] = m;

            return (GetCmd(temp));
        }
        public static byte[] cmd_CmdStopFanByID(int group, int member)
        {
            byte g = Convert.ToByte(group);
            byte m = Convert.ToByte(member);

            byte[] temp = SetFanStop;
            temp[4] = g;
            temp[5] = m;

            return (GetCmd(temp));
        }
        public static byte[] GetCmd(byte[] cmd)
        {
            byte[] temp = new byte[cmd.Length + 5];
            byte[] check = StaticTools.CheckQueueValide(cmd, cmd.Length);

            Buffer.BlockCopy(Head, 0, temp, 0, Head.Length);
            Buffer.BlockCopy(cmd, 0, temp, Head.Length, cmd.Length);
            Buffer.BlockCopy(check, 0, temp, Head.Length + cmd.Length, check.Length);


            return temp;
        }

        public static byte[] cmd_SetBS(int fre, int sf, int bw, int cr, int che, int enc)
        {
            byte[] bs = BitConverter.GetBytes(fre);

            byte bsf = Convert.ToByte(sf);
            byte bbw = Convert.ToByte(bw);
            byte bcr = Convert.ToByte(cr);
            byte bche = Convert.ToByte(che);
            byte benc = Convert.ToByte(enc);

            byte[] temp = { 0x00, 0x0B, 0x01, 0x03, bs[3], bs[2], bs[1], bs[0], bsf, bbw, bcr, bche, benc };

            return (GetCmd(temp));
        }
        public static byte[] cmd_ReadBS()
        {
            byte[] temp = { 0x00,0x02,0x01,0x04 };

            return (GetCmd(temp));
        }

        public static byte[] cmd_ReadNodesCount()
        {
            byte[] temp = {0x00,0x02, 0x01, 0x09 };

            return (GetCmd(temp));
        }
        public static byte[] cmd_SetNodesCount(int count)
        {
            byte h = Convert.ToByte(count / 256);
            byte l = Convert.ToByte(count % 256);
            byte[] temp = {0x00,0x04, 0x01, 0x05, h, l };

            return (GetCmd(temp));
        }
        public static byte[] cmd_AddNodes(List<int> counts)
        {
            int l = counts.Count * 2 + 3;

            

            byte[] temp = {Convert.ToByte(l/256),Convert.ToByte(l%256), 0x01, 0x06 };

            byte[] count = new byte[counts.Count * 2+5];
            count[0] = temp[0];
            count[1] = temp[1];
            count[2] = temp[2];
            count[3] = temp[3];
            count[4] = Convert.ToByte(counts.Count);
            for (int i=0;i<counts.Count;i++)
            {
                count[(i+2)*2+1] = Convert.ToByte(counts[i] / 100);
                count[(i+2)*2+2] = Convert.ToByte(counts[i] % 100);
            }

            return (GetCmd(count));
        }
        public static byte[] cmd_DeleteNodes(List<int> counts)
        {
            int l = counts.Count * 2 + 3;

            byte[] temp = { Convert.ToByte(l / 256), Convert.ToByte(l % 256), 0x01, 0x07 };

            byte[] count = new byte[counts.Count * 2 + 5];
            count[0] = temp[0];
            count[1] = temp[1];
            count[2] = temp[2];
            count[3] = temp[3];
            count[4] = Convert.ToByte(counts.Count);
            for (int i = 0; i < counts.Count; i++)
            {
                count[(i + 2) * 2 + 1] = Convert.ToByte(counts[i] / 100);
                count[(i + 2) * 2 + 2] = Convert.ToByte(counts[i] % 100);
            }

            return (GetCmd(count));
        }
        public static byte[] cmd_ReadAllNodes()
        {
            byte[] temp = {0x00,0x02, 0x01, 0x08 };

            return (GetCmd(temp));
        }

        public static byte[] cmd_CleanAllNodes()
        {
            byte[] temp = { 0x00, 0x02, 0x01, 0x16 };

            return (GetCmd(temp));
        }

        public static byte[] cmd_ReadNodeData(int node)
        {
            byte[] temp = {0x00,0x04, 0x02, 0x01,0x00,0x00 };

            temp[4] = Convert.ToByte(node / 100);
            temp[5] = Convert.ToByte(node % 100);

            return (GetCmd(temp));
        }
        public static byte[] cmd_ReadAllNodeData()
        {
            byte[] temp = {0x00,0x02, 0x02, 0x02 };

            return (GetCmd(temp));
        }

        public static byte[] cmd_ReadNodeDebug(int node)
        {
            byte[] temp = {0x00,0x04, 0x02, 0x03, 0x00, 0x00 };

            temp[4] = Convert.ToByte(node / 100);
            temp[5] = Convert.ToByte(node % 100);

            return (GetCmd(temp));
        }
        public static byte[] cmd_ReadAllNodeDebug()
        {
            byte[] temp = {0x00,0x02, 0x02, 0x04 };

            return (GetCmd(temp));
        }
        public static byte[] cmd_UpdateNodeID(int oldid,int newid)
        {
            byte[] temp = {0x00,0x06, 0x03, 0x01, 0x00, 0x00,0x00,0x00 };

            temp[4] = Convert.ToByte(oldid / 100);
            temp[5] = Convert.ToByte(oldid % 100);
            temp[6] = Convert.ToByte(newid / 100);
            temp[7] = Convert.ToByte(newid % 100);

            return (GetCmd(temp));
        }
        public static byte[] cmd_SetNodeBS(int node, int fre, int sf, int bw, int cr, int che, int enc)
        {
            byte hid = Convert.ToByte(node / 100);
            byte lid = Convert.ToByte(node % 100);
            byte[] bs = BitConverter.GetBytes(fre);
            byte bsf = Convert.ToByte(sf);
            byte bbw = Convert.ToByte(bw);
            byte bcr = Convert.ToByte(cr);
            byte bche = Convert.ToByte(che);
            byte benc = Convert.ToByte(enc);

            byte[] temp = { 0x00, 0x0D, 0x03, 0x02, hid, lid, bs[3], bs[2], bs[1], bs[0], bsf, bbw, bcr, bche, benc };

            return (GetCmd(temp));
        }
        public static byte[] cmd_SetNodeBS(int oldid,int newid, int fre, int sf, int bw, int cr, int che, int enc)
        {
            byte ohid = Convert.ToByte(oldid / 100);
            byte olid = Convert.ToByte(oldid % 100);
            byte nhid = Convert.ToByte(newid / 100);
            byte nlid = Convert.ToByte(newid % 100);

            byte[] bs = BitConverter.GetBytes(fre);
            byte bsf = Convert.ToByte(sf);
            byte bbw = Convert.ToByte(bw);
            byte bcr = Convert.ToByte(cr);
            byte bche = Convert.ToByte(che);
            byte benc = Convert.ToByte(enc);

            byte[] temp = { 0x00, 0x0F, 0x03, 0x03, ohid, olid, nhid, nlid,bs[3], bs[2], bs[1], bs[0], bsf, bbw, bcr, bche, benc };
            
            return (GetCmd(temp));
        }

        public static byte[] cmd_SetNodeRom(int id,int addr,List<byte> datas)
        {
            byte[] temp = { 0x03, 0x04 };
            byte ch = Convert.ToByte((datas.Count+6) / 256);
            byte cl = Convert.ToByte((datas.Count+6) % 256);
            byte hid = Convert.ToByte(id / 100);
            byte lid = Convert.ToByte(id % 100);

            byte[] count = new byte[datas.Count + 8];


            count[0] = ch;
            count[1] = cl;
            count[2] = temp[0];
            count[3] = temp[1];
            count[4] = hid;
            count[5] = lid;
            count[6] = Convert.ToByte(addr);
            count[7] = Convert.ToByte(datas.Count);

            for (int i = 0; i < datas.Count; i++)
            {
                count[i+8] = datas[i];
            }

            return (GetCmd(count));
        }
        public static byte[] cmd_Bit()
        {
            byte[] temp = { 0x00, 0x02,0x00,0x00 };

            return (GetCmd(temp));
        }
    }
}
