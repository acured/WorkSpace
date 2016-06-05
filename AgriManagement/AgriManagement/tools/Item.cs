﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgriManagement.tools
{
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public string area { get; set; }
        public List<History> historys { get; set; }
        public Status status { get; set; }
        public int RetryCount { get; set; }
    }
    public class History
    {
        public int id { get; set; }
        public DateTime time { get; set; }
        public double temperature { get; set; }
        public double moisture { get; set; }
        public double NH { get; set; }
        public Status status { get; set; }
    }

    public class DataGridShow
    {
        public int id { get; set; }
        public string name { get; set; }
        public string area { get; set; }
        public double temperature { get; set; }
        public double moisture { get; set; }
        public double NH { get; set; }
        public DateTime time { get; set; }
        public Status status { get; set; }
        public bool isShow { get; set; }
    }
    public class DataGridShowModel : INotifyCollectionChanged
    {
        private DataGridShow data;

        public DataGridShow Data
        {
            get { return data; }
        }

        public DataGridShowModel(DataGridShow data)
        {
            this.data = data;
        }

        public int id
        {
            get { return Data.id; }
            set
            {
                Data.id = value;
                NotifyCollectionChanged("Title");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void NotifyCollectionChanged(String info)
        {
            if (CollectionChanged != null)
            {
                //CollectionChanged(this, new NotifyCollectionChangedEventArgs(info));
            }
        }
    }

    public class Users
    {
        public string id { get; set; }
        public string name { get; set; }
        public string psd { get; set; }
        public string nickname { get; set; }
    }

    public enum Status
    {
        OnLine,
        OffLine,
        Regular,
        Warning,
        Error,
    }
}