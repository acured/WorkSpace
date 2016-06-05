using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriManagement.tools
{
    public static class Config
    {
        static String _maxT = ConfigurationManager.AppSettings["maxT"];
        public static double maxT {
            get { return Convert.ToDouble(_maxT); }
            set { ConfigurationManager.AppSettings["maxT"] = value.ToString(); }
        }

        static String _minT = ConfigurationManager.AppSettings["minT"];
        public static double minT
        {
            get { return Convert.ToDouble(_minT); }
            set
            {
                ConfigurationManager.AppSettings["minT"] = value.ToString();
            }
        }
        static String _DivT = ConfigurationManager.AppSettings["DivT"];
        public static double DivT
        {
            get { return Convert.ToDouble(_DivT); }
            set
            {
                ConfigurationManager.AppSettings["DivT"] = value.ToString();
            }
        }
        static String _maxM = ConfigurationManager.AppSettings["maxM"];
        public static double maxM
        {
            get { return Convert.ToDouble(_maxM); }
            set
            {
                ConfigurationManager.AppSettings["maxM"] = value.ToString();
            }
        }
        static String _minM = ConfigurationManager.AppSettings["minM"];
        public static double minM
        {
            get { return Convert.ToDouble(_minM); }
            set
            {
                ConfigurationManager.AppSettings["minM"] = value.ToString();
            }
        }
        static String _DivM = ConfigurationManager.AppSettings["DivM"];
        public static double DivM
        {
            get { return Convert.ToDouble(_DivM); }
            set
            {
                ConfigurationManager.AppSettings["DivM"] = value.ToString();
            }
        }
        static String _maxN = ConfigurationManager.AppSettings["maxN"];
        public static double maxN
        {
            get { return Convert.ToDouble(_maxN); }
            set
            {
                ConfigurationManager.AppSettings["maxN"] = value.ToString();
            }
        }
        static String _minN = ConfigurationManager.AppSettings["minN"];
        public static double minN
        {
            get { return Convert.ToDouble(_minN); }
            set
            {
                ConfigurationManager.AppSettings["minN"] = value.ToString();
            }
        }
        static String _DivN = ConfigurationManager.AppSettings["DivN"];
        public static double DivN
        {
            get { return Convert.ToDouble(_DivN); }
            set
            {
                ConfigurationManager.AppSettings["DivN"] = value.ToString();
            }
        }
        static String _freq_chat = ConfigurationManager.AppSettings["freq_chat"];
        public static double freq_chat
        {
            get { return Convert.ToDouble(_freq_chat); }
            set
            {
                ConfigurationManager.AppSettings["freq_chat"] = value.ToString();
            }
        }
        static String _freq_data = ConfigurationManager.AppSettings["freq_data"];
        public static double freq_data
        {
            get { return Convert.ToDouble(_freq_data); }
            set
            {
                ConfigurationManager.AppSettings["freq_data"] = value.ToString();
            }
        }
        static String _host = ConfigurationManager.AppSettings["host"];
        public static string host
        {
            get { return _host; }
            set
            {
                ConfigurationManager.AppSettings["host"] = value.ToString();
            }
        }
        static String _i_updateDate = ConfigurationManager.AppSettings["i_updateDate"];
        public static string i_updateDate
        {
            get { return _i_updateDate; }
            set
            {
                ConfigurationManager.AppSettings["i_updateDate"] = value.ToString();
            }
        }
        static String _i_updateUser = ConfigurationManager.AppSettings["i_updateUser"];
        public static string i_updateUser
        {
            get { return _i_updateUser; }
            set
            {
                ConfigurationManager.AppSettings["i_updateUser"] = value.ToString();
            }
        }
        static String _i_insertUser = ConfigurationManager.AppSettings["i_insertUser"];
        public static string i_insertUser
        {
            get { return _i_insertUser; }
            set
            {
                ConfigurationManager.AppSettings["i_insertUser"] = value.ToString();
            }
        }
        static String _i_deleteUser = ConfigurationManager.AppSettings["i_deleteUser"];
        public static string i_deleteUser
        {
            get { return _i_deleteUser; }
            set
            {
                ConfigurationManager.AppSettings["i_deleteUser"] = value.ToString();
            }
        }
        static String _i_login = ConfigurationManager.AppSettings["i_login"];
        public static string i_login
        {
            get { return _i_login; }
            set
            {
                ConfigurationManager.AppSettings["i_login"] = value.ToString();
            }
        }
        static String _com = ConfigurationManager.AppSettings["com"];
        public static string com
        {
            get { return _com; }
            set
            {
                ConfigurationManager.AppSettings["com"] = value.ToString();
            }
        }
        static String _port = ConfigurationManager.AppSettings["port"];
        public static int port
        {
            get { return Convert.ToInt32(_port); }
            set
            {
                ConfigurationManager.AppSettings["port"] = value.ToString();
            }
        }
        static String _retry = ConfigurationManager.AppSettings["retry"];
        public static int retry
        {
            get { return Convert.ToInt32(_retry); }
            set
            {
                ConfigurationManager.AppSettings["retry"] = value.ToString();
            }
        }
        static String _play = ConfigurationManager.AppSettings["play"];
        public static int play
        {
            get { return Convert.ToInt32(_play); }
            set
            {
                ConfigurationManager.AppSettings["play"] = value.ToString();
            }
        }
    }
}
