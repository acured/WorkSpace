using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AgriManagement.tools
{
    public static class StaticTools
    {
        public static string FilePath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "file.txt";
        private static readonly byte[] auchCRCHi ={
0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81,
0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01,
0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01,
0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81,
0x40};

        private static readonly byte[] auchCRCLo ={
0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06, 0x07, 0xC7, 0x05, 0xC5, 0xC4,
0x04, 0xCC, 0x0C, 0x0D, 0xCD, 0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A, 0x1E, 0xDE, 0xDF, 0x1F, 0xDD,
0x1D, 0x1C, 0xDC, 0x14, 0xD4, 0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3, 0xF2, 0x32, 0x36, 0xF6, 0xF7,
0x37, 0xF5, 0x35, 0x34, 0xF4, 0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29, 0xEB, 0x2B, 0x2A, 0xEA, 0xEE,
0x2E, 0x2F, 0xEF, 0x2D, 0xED, 0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60, 0x61, 0xA1, 0x63, 0xA3, 0xA2,
0x62, 0x66, 0xA6, 0xA7, 0x67, 0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68, 0x78, 0xB8, 0xB9, 0x79, 0xBB,
0x7B, 0x7A, 0xBA, 0xBE, 0x7E, 0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71, 0x70, 0xB0, 0x50, 0x90, 0x91,
0x51, 0x93, 0x53, 0x52, 0x92, 0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B, 0x99, 0x59, 0x58, 0x98, 0x88,
0x48, 0x49, 0x89, 0x4B, 0x8B, 0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42, 0x43, 0x83, 0x41, 0x81, 0x80,
0x40};
        public static int getindex(double num)
        {
            if (num == 5) return 0;
            else if (num == 10) return 1;
            else if (num == 30) return 3;
            else if (num == 60) return 4;
            else if (num == 120) return 5;
            else if (num == 300) return 6;
            else if (num == 600) return 7;
            else if (num == 1800) return 8;
            else return 9;
        }
        public static int getPortindex(double num)
        {
            if (num == 9600) return 0;
            else if (num == 19200) return 1;
            else if (num == 56000) return 2;
            else return 3;
        }
        public static int getPortvalue(int index)
        {
            if (index == 0) return 9600;
            else if (index == 1) return 19200;
            else if (index == 2) return 56000;
            else return 115200;
        }
        public static double getvalue(int index)
        {
            if (index == 0) return 5;
            else if (index == 1) return 10;
            else if (index == 2) return 30;
            else if (index == 3) return 60;
            else if (index == 4) return 120;
            else if (index == 5) return 300;
            else if (index == 6) return 600;
            else if (index == 7) return 1800;
            else return 3600;
        }
        public static string MD5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
            return System.Text.Encoding.Default.GetString(result);
        }
        public static void saveData(string msg)
        {
            using (System.IO.StreamWriter file =
    new System.IO.StreamWriter(FilePath, true))
            {
                file.WriteLine(msg);
            }
        }

        public static string[] loadData()
        {
            string[] lines = System.IO.File.ReadAllLines(FilePath);
            return lines;
        }

        public static void SaveConfig(string key, string ConnenctionString)
        {
            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "AgriManagement.exe.config";
            doc.Load(strFileName);
            //找出名称为“add”的所有元素
            XmlNodeList nodes = doc.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                XmlAttribute att = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                if (att != null && att.Value == key)
                {
                    //对目标元素中的第二个属性赋值
                    att = nodes[i].Attributes["value"];
                    att.Value = ConnenctionString;
                    break;
                }
            }
            //保存上面的修改
            doc.Save(strFileName);
        }

        public static List<string> GetAllArea()
        {
            List<string> strs = new List<string>();

            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            doc.Load(strFileName);
            //找出名称为“add”的所有元素
            XmlNodeList nodes = doc.GetElementsByTagName("Area");

            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                string name = nodes[i].Attributes[0].Value;
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                strs.Add(name);
            }
            //保存上面的修改
            doc.Save(strFileName);
            return strs;
        }

        public static List<string> GetAllNodesByArea(string area)
        {
            List<string> strs = new List<string>();

            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            doc.Load(strFileName);
            //找出名称为“add”的所有元素
            XmlNodeList nodes = doc.GetElementsByTagName("Area");

            for (int i = 0; i < nodes.Count; i++)
            {
                string name = nodes[i].Attributes[0].Value;
                if (name == area)
                {
                    XmlNodeList nodes1 = nodes[i].ChildNodes;
                    for (int j = 0; j < nodes1.Count; j++)
                    {
                        strs.Add(nodes1[j].Attributes[0].Value);
                    }
                }
            }
            return strs;
        }

        public static void AddArea(string areaname)
        {
            List<string> strs = new List<string>();

            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            doc.Load(strFileName);
            //找出名称为“add”的所有元素

            XmlNode root = doc.SelectSingleNode("Areas");//查找 

            XmlElement xe1 = doc.CreateElement("Area");//创建一个节点 
            xe1.SetAttribute("name",areaname);//设置该节点genre属性 
            root.AppendChild(xe1);
            //保存上面的修改
            doc.Save(strFileName);
        }

        public static void AddNode(string areaname,string nodeid,string nodename)
        {
            if (!checkArea(areaname))
                AddArea(areaname);
            List<string> strs = new List<string>();

            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            doc.Load(strFileName);
            XmlNodeList nodes = doc.GetElementsByTagName("Area");

            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                string name = nodes[i].Attributes[0].Value;
                if (name == areaname)
                {
                    XmlElement xe1 = doc.CreateElement("node");//创建一个节点 
                    xe1.SetAttribute("id", nodeid);//设置该节点genre属性 
                    xe1.InnerText = nodename;
                    nodes[i].AppendChild(xe1);
                }
            }
            //保存上面的修改
            doc.Save(strFileName);
        }

        static bool checkArea(string area)
        {
            XmlDocument doc = new XmlDocument();
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            doc.Load(strFileName);

            XmlNodeList nodes = doc.GetElementsByTagName("Area");

            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                string name = nodes[i].Attributes[0].Value;
                if (name == area)
                {
                    return true;
                }
            }
            return false;
        }


        public static bool checkNode(string areaname, string nodeid)
        {
            List<string> strs = new List<string>();

            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            doc.Load(strFileName);
            XmlNodeList nodes = doc.GetElementsByTagName("Area");

            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                string name = nodes[i].Attributes[0].Value;
                if (name == areaname)
                {
                    XmlNodeList nodes1 = nodes[i].ChildNodes;
                    for (int j = 0; j < nodes1.Count; j++)
                    {
                        if (nodes1[j].Attributes[0].Value == nodeid) return true;
                    }
                }
            }
            return false;
        }

        public static string GetAreaByNode(string nodeid)
        {
            List<string> strs = new List<string>();

            XmlDocument doc = new XmlDocument();
            //获得配置文件的全路径
            string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "config.xml";
            doc.Load(strFileName);
            XmlNodeList nodes = doc.GetElementsByTagName("Area");

            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                string name = nodes[i].Attributes[0].Value;
                XmlNodeList nodes1 = nodes[i].ChildNodes;
                for (int j = 0; j < nodes1.Count; j++)
                {
                    if (nodes1[j].Attributes[0].Value == nodeid) return name;
                }
            }
            return "";
        }

        public static double GetYaxia(double value, double maxvalue, double contantHeight)
        {
            return (contantHeight / maxvalue) * value;
        }

        public static string GetCpuInfo()
        {
            string cpuInfo = " ";
            ManagementClass cimobject = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = cimobject.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
            }
            return cpuInfo.ToString();
        }
        public static byte[] CheckQueueValide(byte[] data, int lenth)
        {
            byte[] result = new byte[] { 0xFF, 0xFF };

            for (int i = 0; i < lenth; i++)// (lenth-->0)
            {
                int uindex = result[1] ^ data[i];
                result[1] = BitConverter.GetBytes(result[0] ^ auchCRCHi[uindex])[0];
                result[0] = auchCRCLo[uindex];
            }
            return result;
        }
    }
}
