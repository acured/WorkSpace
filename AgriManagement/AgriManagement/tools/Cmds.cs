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
        //读取BS端的中所有节点的工作数据
        public static byte[] ReadAllNodeData = { 0x02, 0x02 };//Node ID//1st Node Work Data Block	2nd Node Work Data Block	… …	Nth Node Work Data Block//Node ID	State	Temperature	Humidity	NH4	Reserved1	Reserv2ed2	Reserved3
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
        //通过BS配置节点寄存器
        public static byte[] SetNodeRegister = { 0x03, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };//Node ID	Reg Addr	Length of Byte	Byte1	Byte2	… …	Byte16





    }
}
