﻿using AgriManagement.tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace AgriManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool IsRun = true;
        string _sign = string.Empty;
        double _maxT = Config.maxT;
        double _minT = Config.minT;
        double _maxM = Config.maxM;
        double _minM = Config.minM;
        double _maxN = Config.maxN;
        double _minN = Config.minN;

        double _isShow_tempA = Config.showT;
        double _isShow_moistA = Config.showM;
        double _isShow_tempE = Config.showC;
        double _isShow_moistE = Config.showD1;
        double _isShow_tempE2 = Config.showD2;
        double _isShow_moistE2 = Config.showD3;

        double _DivT = Config.DivT;
        double _DivM = Config.DivM;
        double _DivN = Config.DivN;

        string _Area = string.Empty;
        string _AreaChat = string.Empty;
        string _NodeChat = string.Empty;
        string _HistoryArea = string.Empty;

        //double _freq_data = Config.freq_data;
        //double _freq_chat = Config.freq_chat;
        double _freq_data = 2;

        double _retry = Config.retry;
        double _play = Config.play;
        List<Users> _users = new List<Users>();

        int port = Config.port;
        string comname = Config.com;

        string _host = Config.host;
        string _i_deleteUser = Config.i_deleteUser;
        string _i_insertUser = Config.i_insertUser;
        string _i_login = Config.i_login;
        string _i_updateData = Config.i_updateDate;
        string _i_updateUser = Config.i_updateUser;

        DeviceAdapter _device = new DeviceAdapter();
        CloudAdapter _cloud = new CloudAdapter();
        DeviceBill _bill;

        int _maxHistorys = 8640;//one days
        int _maxShow = 360;//one days

        Dictionary<string, Item> items = new Dictionary<string, Item>();
        Dictionary<string, Item> cloudData = new Dictionary<string, Item>();

        Dictionary<string, bool> _checkShow = new Dictionary<string, bool>();
        Dictionary<string, bool> _errorList = new Dictionary<string, bool>();

        List<DataGridShow> _mytestdata = new List<DataGridShow>();

        string _historyID = "0";

        Random ran = new Random();
        public MainWindow()
        {
            InitializeComponent();

            string msg = _device.connect();
            Console.WriteLine(msg);
            _bill = new DeviceBill(_device);


            InitSensors();
            initUI();

            //_cloud.PostInsertUser("testID", "testPwd", "testNN");
            start();


        }
        
        public History CopyHistory(History h)
        {
            History hnew = new History();
            hnew.id = h.id;
            hnew.moistureA = h.moistureA;
            hnew.temperatureE = h.temperatureE;
            hnew.temperatureA = h.temperatureA;
            hnew.time = h.time;
            return hnew;
        }

        private void initUI()
        {
            #region init setting page
            txt_temp_max.Text = _maxT.ToString();
            txt_temp_min.Text = _minT.ToString();
            txt_mos_max.Text = _maxM.ToString();
            txt_mos_min.Text = _minM.ToString();
            txt_RH_max.Text = _maxN.ToString();
            txt_RH_min.Text = _minN.ToString();
            txt_temp_deviation.Text = _DivT.ToString();
            txt_mos_deviation.Text = _DivN.ToString();
            txt_RH_deviation.Text = _DivN.ToString();


            txt_addr.Text = _host;
            txt_data.Text = _i_updateData;
            txt_delete.Text = _i_deleteUser;
            txt_insert.Text = _i_insertUser;
            txt_login.Text = _i_login;
            txt_update.Text = _i_updateUser;
            #endregion



            #region init history Page
            List<string> areaListShow = StaticTools.GetAllAreaShow();
            List<string> areaList = StaticTools.GetAllArea();
            foreach (string s in areaListShow)
            {
                cmb_eqArea.Items.Add(s);
            }
            cmb_eqArea.SelectedIndex = 0;


            if (_play == 1) cb_play.IsChecked = true;
            else cb_play.IsChecked = false;
            if (_isShow_tempA == 1) cb_temp.IsChecked = true;
            else cb_temp.IsChecked = false;
            if (_isShow_moistA == 1) cb_moist.IsChecked = true;
            else cb_moist.IsChecked = false;
            if (_isShow_tempE == 1) cb_ch3.IsChecked = true;
            else cb_ch3.IsChecked = false;
            if (_isShow_moistE == 1) cb_data1.IsChecked = true;
            else cb_data1.IsChecked = false;
            if (_isShow_tempE2 == 1) cb_data2.IsChecked = true;
            else cb_data2.IsChecked = false;
            if (_isShow_moistE2 == 1) cb_data3.IsChecked = true;
            else cb_data3.IsChecked = false;
            #endregion

            txt_SF.Items.Add("6");
            txt_SF.Items.Add("7");
            txt_SF.Items.Add("8");
            txt_SF.Items.Add("9");
            txt_SF.Items.Add("10");
            txt_SF.Items.Add("11");
            txt_SF.Items.Add("12");
            txt_SF.SelectedIndex = 0;

            txt_BW.Items.Add("7.80");
            txt_BW.Items.Add("10.4");
            txt_BW.Items.Add("15.6");
            txt_BW.Items.Add("20.8");
            txt_BW.Items.Add("31.2");
            txt_BW.Items.Add("41.6");
            txt_BW.Items.Add("62.5");
            txt_BW.Items.Add("125");
            txt_BW.Items.Add("250");
            txt_BW.Items.Add("500");
            txt_BW.SelectedIndex = 0;

            txt_rate.Items.Add("1");
            txt_rate.Items.Add("2");
            txt_rate.Items.Add("3");
            txt_rate.Items.Add("4");
            txt_rate.SelectedIndex = 0;

            foreach (string s in areaList)
            {
                cmb_areachat.Items.Add(s);
                cmb_areatable.Items.Add(s);

                _checkShow.Add(s, false);
            }
            if (cmb_areachat.Items.Count>0)
                _AreaChat = cmb_areachat.Items[0].ToString();

            cmb_areachat.SelectedIndex = 0;
            cmb_areatable.SelectedIndex = 0;

            if (areaList.Count > 0)
            {
                try
                {
                    if (areaList.Count == 1)
                    {
                        cb_1.Content = areaList[0];
                        cb_1.IsChecked = true;

                        cb_2.Visibility = Visibility.Hidden;
                        cb_3.Visibility = Visibility.Hidden;
                        cb_4.Visibility = Visibility.Hidden;
                    }
                    else if (areaList.Count == 2)
                    {
                        cb_1.Content = areaList[0];
                        cb_1.IsChecked = true;

                        cb_2.Content = areaList[1];
                        cb_2.IsChecked = false;

                        cb_3.Visibility = Visibility.Hidden;
                        cb_4.Visibility = Visibility.Hidden;
                    }
                    else if (areaList.Count == 3)
                    {
                        cb_1.Content = areaList[0];
                        cb_1.IsChecked = true;

                        cb_2.Content = areaList[1];
                        cb_2.IsChecked = false;

                        cb_3.Content = areaList[2];
                        cb_3.IsChecked = false;

                        cb_4.Visibility = Visibility.Hidden;
                    }
                    else if (areaList.Count == 4)
                    {
                        cb_1.Content = areaList[0];
                        cb_1.IsChecked = true;

                        cb_2.Content = areaList[1];
                        cb_2.IsChecked = false;

                        cb_3.Content = areaList[2];
                        cb_3.IsChecked = false;

                        cb_4.Content = areaList[3];
                        cb_4.IsChecked = false;
                    }
                }
                catch { };
            }

            update_cmd();
        }

        private void updateFacilityStatus()
        {
            try
            {
                if (tv_status1.Items.Count > 0)
                    tv_status1.Dispatcher.Invoke(() => { tv_status1.Items.Clear(); });
                if (tv_status2.Items.Count > 0)
                    tv_status2.Dispatcher.Invoke(() => { tv_status2.Items.Clear(); });
                if (tv_status3.Items.Count > 0)
                    tv_status3.Dispatcher.Invoke(() => { tv_status3.Items.Clear(); });
                if (tv_status4.Items.Count > 0)
                    tv_status4.Dispatcher.Invoke(() => { tv_status4.Items.Clear(); });

                //ell_moniter.Fill = Brushes.Red;

                List<string> areaList = StaticTools.GetAllArea();
                int index = 1;
                if (areaList.Count > 0)
                {
                    foreach (string area in areaList)
                    {
                        status_show.Dispatcher.Invoke(() => {
                        TreeViewItem newChild = new TreeViewItem();
                        newChild.Header = area + "网络";

                        StackPanel sp1 = new StackPanel();
                        sp1.Orientation = Orientation.Horizontal;
                        sp1.Height = 40;

                        TreeViewItem newChildnode1 = new TreeViewItem();
                        newChildnode1.Header = "工控端：";
                        newChildnode1.VerticalAlignment = VerticalAlignment.Center;
                        Ellipse blueRectangle1 = new Ellipse();
                        blueRectangle1.Height = 15;
                        blueRectangle1.Width = 15;

                        SolidColorBrush blueBrush1 = new SolidColorBrush();
                            foreach (string key in items.Keys)
                            {
                                if (items[key].area == area)
                                {
                                    if (items[key].status == Status.OffLine)
                                        blueBrush1.Color = Colors.Gray;
                                    else
                                        blueBrush1.Color = Colors.Blue;
                                }
                            }
                        blueRectangle1.StrokeThickness = 1;
                        blueRectangle1.Stroke = blueBrush1;
                        blueRectangle1.Fill = blueBrush1;

                        sp1.Children.Add(newChildnode1);
                        sp1.Children.Add(blueRectangle1);
                        newChild.Items.Add(sp1);

                        foreach (string key in items.Keys)
                        {
                            if (items[key].area == area)
                            {
                                StackPanel sp = new StackPanel();
                                sp.Orientation = Orientation.Horizontal;
                                sp.Height = 40;

                                TreeViewItem newChildnode = new TreeViewItem();
                                newChildnode.Header = "编号：" + items[key].name + " 状态：";
                                newChildnode.VerticalAlignment = VerticalAlignment.Center;
                                Ellipse blueRectangle = new Ellipse();
                                blueRectangle.Height = 15;
                                blueRectangle.Width = 15;

                                SolidColorBrush blueBrush = new SolidColorBrush();

                                if (items[key].status==Status.Regular|| items[key].status == Status.OnLine|| items[key].status == Status.Warning)
                                    blueBrush.Color = Colors.Blue;
                                else if(items[key].status == Status.OffLine || items[key].status == Status.Error)
                                    blueBrush.Color = Colors.Red;
                                blueRectangle.StrokeThickness = 1;
                                blueRectangle.Stroke = blueBrush;
                                blueRectangle.Fill = blueBrush;

                                sp.Children.Add(newChildnode);
                                sp.Children.Add(blueRectangle);
                                newChild.Items.Add(sp);
                            }
                        }
                        if (index == 1)
                        {
                            tv_status1.Items.Add(newChild);
                            ExpandInternal(tv_status1);
                        }
                        else if (index == 2)
                        {
                            tv_status2.Items.Add(newChild);
                            ExpandInternal(tv_status2);
                        }
                        else if (index == 3)
                        {
                            tv_status3.Items.Add(newChild);
                            ExpandInternal(tv_status3);
                        }
                        else
                        {
                            tv_status4.Items.Add(newChild);
                            ExpandInternal(tv_status4);
                        }
                        index++;
                        });
                    }
                }
            }
            catch
            {

            }
        }

        private static void ExpandInternal(System.Windows.Controls.ItemsControl targetItemContainer)
        {
            if (targetItemContainer == null) return;
            if (targetItemContainer.Items == null) return;
            for (int i = 0; i < targetItemContainer.Items.Count; i++)
            {
                System.Windows.Controls.TreeViewItem treeItem = targetItemContainer.Items[i] as System.Windows.Controls.TreeViewItem;
                if (treeItem == null) continue;
                if (!treeItem.HasItems) continue;

                treeItem.IsExpanded = true;
                ExpandInternal(treeItem);
            }

        }

        private void InitSensors()
        {
            List<string> areas = StaticTools.GetAllArea();

            foreach (string area in areas)
            {
                string areaid = StaticTools.getAreaID(area);
                List<string> nodes = StaticTools.GetAllNodesByArea(area);
                foreach (string node in nodes)
                {
                    List<History> ls = new List<History>();
                    List<History> ls1 = new List<History>();
                    Item item = new Item();
                    item.name = (Convert.ToInt32(node)+Convert.ToInt32(areaid) *100).ToString();
                    item.area = area;
                    item.status = Status.OnLine;
                    item.id = Convert.ToInt32(node) + Convert.ToInt32(areaid) * 100;
                    item.RetryCount = 0;
                    item.historys = new List<History>();
                    item.ErrorCount = 0;
                    item.lastStatus = Status.Regular;
                    items.Add(item.id.ToString(), item);
                }
            }

            cloudData = new Dictionary<string, Item>(items);
        }

        private void start()
        {
            Thread updateShowpage = new Thread(thread_updateShowPage);
            updateShowpage.Start();
        }

        #region thread method
        private void thread_checkDevice()
        {
            Thread.Sleep(1000);
            while (true)
            {
                foreach (string key in items.Keys)
                {
                    if (items[key].RetryCount > _retry)
                    {
                        if (items[key].status == Status.OnLine)
                        {
                            items[key].status = Status.OffLine;
                            alert(items[key].id.ToString());
                            items[key].RetryCount = 0;
                        }
                    }
                }

                //updateTable("", "");

                Thread.Sleep((int)this._freq_data * 100);
            }
        }

        private void updateTable(string id)
        {
            _mytestdata.Sort((a, b) => { return a.id - b.id; });
            SrollBarDataGrid.Dispatcher.Invoke(() =>
            {
                SrollBarDataGrid.Items.Clear();
                for (int i = 0; i < _mytestdata.Count; i++)
                {
                    if (id == _mytestdata[i].id.ToString())
                    {
                        DataGridShow dgs = new DataGridShow()
                        {
                            status = _mytestdata[i].status,
                            area = _mytestdata[i].area,
                            id = _mytestdata[i].id,
                            moistureA = _mytestdata[i].moistureA,
                            temperatureE = _mytestdata[i].temperatureE,
                            temperatureA = _mytestdata[i].temperatureA,
                            time = _mytestdata[i].time,
                            isShow = true
                        };
                        SrollBarDataGrid.Items.Add(dgs);
                    }
                    //SrollBarDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;

                }
            });
        }

        private void thread_updateShowPage()
        {
            while (IsRun)
            {
                try {
                    updateSenser();
                    updateShow();
                    updateChat(_AreaChat,_NodeChat);
                    updateHistory();
                    updateFacilityStatus();
                    //updateTable(_historyID);
                    Thread.Sleep((int)this._freq_data * 1000);
                }
                catch
                { }
            }
        }
        #endregion
        private void alert(string id)
        {
            cb_play.Dispatcher.Invoke(() => {
                if (cb_play.IsChecked == true)
                {
                    SoundPlayer sp = new SoundPlayer();
                    string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "2.wav";
                    sp.SoundLocation = strFileName;
                    sp.Play();
                }
            });

            //MessageBox.Show(id + "号设备故障，请查看！");
        }
        private void updateChat(string areaid,string nodeid)
        {

            string id = (Convert.ToInt32(StaticTools.getAreaID(areaid)) * 100 + Convert.ToInt32(nodeid)).ToString();
            txt_error_facility.Dispatcher.Invoke(() => {
                txt_error_facility.Text = "异常设备编号("+_errorList.Count+")：\n";
                foreach (string key in _errorList.Keys)
                {
                    if (_errorList[key])
                        txt_error_facility.Text += key + "\n";
                }
            });
            cav_chat.Dispatcher.Invoke(() =>
            {
                int count = cav_chat.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    cav_chat.Children.RemoveAt(0);
                }
            });

            double top = 25;
            double left = 25;
            double width = 800;
            double heigh = 180;
            drawRang(cav_chat, new Point(left, top), (int)width, (int)heigh,Brushes.White);

            #region temp chat
            if (items[id].historys.Count > 0)
            {
                double xAxie = 25;
                double yAxie = 0;

                int start = 0;
                if (items[id].historys.Count > _maxShow)
                {
                    start = items[id].historys.Count - _maxShow;
                    yAxie = top + heigh - StaticTools.GetYaxia(items[id].historys[items[id].historys.Count - _maxShow].temperatureA, 120, heigh);
                }
                else
                {
                    start = 0;
                    yAxie = top + heigh - StaticTools.GetYaxia(items[id].historys[0].temperatureA, 120, heigh);
                }
                int _uint = _maxShow / 20;
                for (int i = start + 1; i < items[id].historys.Count; i++)
                {
                    double y = top + heigh - StaticTools.GetYaxia(items[id].historys[i].temperatureA, 120, heigh);
                    double x = xAxie + 2.2;
                    drawLine(cav_chat, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                    if ((i-start-1) % _uint == 0)
                        drawText(cav_chat, items[id].historys[i].time.ToShortTimeString(), x - 15, heigh + 30, 12, 28);
                    xAxie = x;
                    yAxie = y;
                }
            }
            #endregion

            #region mos chat
            if (items[id].historys.Count > 0)
            {
                double xAxie = 25;
                double yAxie = 0;

                int start = 0;
                if (items[id].historys.Count > _maxShow)
                {
                    start = items[id].historys.Count - _maxShow;
                    yAxie = top + heigh + 240 - StaticTools.GetYaxia(items[id].historys[items[id].historys.Count - _maxShow].moistureA, 120, heigh);
                }
                else
                {
                    start = 0;
                    yAxie = top + heigh + 240 - StaticTools.GetYaxia(items[id].historys[0].moistureA, 120, heigh);
                }
                int _uint = _maxShow / 20;
                for (int i = start + 1; i < items[id].historys.Count; i++)
                {
                    double y = top + heigh + 240 - StaticTools.GetYaxia(items[id].historys[i].moistureA, 120, heigh);
                    double x = xAxie + 2.2;
                    drawLine(cav_chat, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                    if ((i - start - 1) % _uint == 0)
                        drawText(cav_chat, items[id].historys[i].time.ToShortTimeString(), x - 15, heigh + 30 + 240, 12, 28);
                    xAxie = x;
                    yAxie = y;
                }
            }
            #endregion

            #region mos NH
            if (items[id].historys.Count > 0)
            {
                double xAxie = 25;
                double yAxie = 0;

                int start = 0;
                if (items[id].historys.Count > _maxShow)
                {
                    start = items[id].historys.Count - _maxShow;
                    yAxie = top + heigh + 480 - StaticTools.GetYaxia(items[id].historys[items[id].historys.Count - _maxShow].moistureA, 120, heigh);
                }
                else
                {
                    start = 0;
                    yAxie = top + heigh + 480 - StaticTools.GetYaxia(items[id].historys[0].moistureA, 120, heigh);
                }
                int _uint = _maxShow / 20;
                for (int i = start + 1; i < items[id].historys.Count; i++)
                {
                    double y = top + heigh + 480 - StaticTools.GetYaxia(items[id].historys[i].moistureA, 120, heigh);
                    double x = xAxie + 2.2;
                    drawLine(cav_chat, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                    if ((i - start - 1) % _uint == 0)
                        drawText(cav_chat, items[id].historys[i].time.ToShortTimeString(), x - 15, heigh + 30 + 480, 12, 40);
                    xAxie = x;
                    yAxie = y;
                }
            }
            #endregion
        }
        private List<History> GetAveHistory(string area)
        {
            List<History> lh = new List<History>();
            List<string> areaList = StaticTools.GetAllArea();
            var data = items.Where(a => a.Value.area == area).Select(p => p.Key);
            if (data.ToList().Count == 0) return lh;

            string tempnode = StaticTools.GetAllNodesByArea(areaList[0])[0];
            string temparea = StaticTools.getAreaID(areaList[0]);

            string tempkey = (Convert.ToInt32(temparea) * 100 + Convert.ToInt32(tempnode)).ToString();

            double datacount = items[tempkey].historys.Count;
            int unit = (int)datacount / 288;
            for (int i = 0; i < datacount; i++)
            {
                if (unit == 0)
                {
                    double maxTemp = 0;
                    double maxNH = 0;
                    double maxMio = 0;
                    History h = new History();
                    DateTime dt = DateTime.Now;
                    double count = 0;
                    foreach (string key in data)
                    {
                        maxTemp += items[key].historys[i].temperatureA;
                        maxNH += items[key].historys[i].temperatureE;
                        maxMio += items[key].historys[i].moistureA;
                        dt = items[key].historys[i].time;
                        count++;
                    }
                    h.moistureA = maxMio / count;
                    h.temperatureE = maxNH / count;
                    h.temperatureA = maxTemp / count;
                    h.time = dt;
                    lh.Add(h);
                }
                else if (i % unit == 0)
                {
                    double maxTemp = 0;
                    double maxNH = 0;
                    double maxMio = 0;
                    History h = new History();
                    DateTime dt = DateTime.Now;
                    double count = 0;
                    foreach (string key in data)
                    {
                        maxTemp += items[key].historys[i].temperatureA;
                        maxNH += items[key].historys[i].temperatureE;
                        maxMio += items[key].historys[i].moistureA;
                        dt = items[key].historys[i].time;
                        count++;
                    }
                    h.moistureA = maxMio / count;
                    h.temperatureE = maxNH / count;
                    h.temperatureA = maxTemp / count;
                    h.time = dt;
                    lh.Add(h);
                }
            }
            return lh;
        }
        private void updateHistory()
        {
            cav_history.Dispatcher.Invoke(() =>
            {
                int count = cav_history.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    cav_history.Children.RemoveAt(0);
                }
            });

            double index = 0;

            List<string> areas = StaticTools.GetAllArea();
            int flag = 0;
            Brush color = Brushes.White;
            foreach (string area in areas)
            {
                if (flag == 1)color= Brushes.Thistle;
                if (flag == 2) color = Brushes.YellowGreen;
                if (flag == 3) color = Brushes.SpringGreen;

                flag++;
                double top = 25 + index * 720;
                double left = 25;
                double width = 640;
                double heigh = 180;

                drawText(cav_history, area + "历史曲线图", left, top-25, 16, 200);

                drawRang(cav_history, new Point(left, top), (int)width, (int)heigh, color);

                List<History> hs = GetAveHistory(area);
                #region temp chat
                if (hs.Count > 0)
                {
                    double xAxie = 25;
                    double yAxie = 0;

                    int start = 0;
                    yAxie = top + heigh - StaticTools.GetYaxia(hs[0].temperatureA, 120, heigh);

                    int _uint = 36;
                    for (int i = start + 1; i < hs.Count; i++)
                    {
                        double y = top + heigh - StaticTools.GetYaxia(hs[i].temperatureA, 120, heigh);
                        double x = xAxie + 2.2;
                        drawLine(cav_history, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                        if (i % _uint == 0 || i == start + 1)
                            drawText(cav_history, hs[i].time.ToShortTimeString(), x - 15, top+ heigh + 5, 12, 28);
                        xAxie = x;
                        yAxie = y;
                    }
                }
                #endregion

                #region mos chat
                if (hs.Count > 0)
                {
                    double xAxie = 25;
                    double yAxie = 0;

                    int start = 0;
                    yAxie = top + 240 + heigh - StaticTools.GetYaxia(hs[0].moistureA, 120, heigh);

                    int _uint = 36;
                    for (int i = start + 1; i < hs.Count; i++)
                    {
                        double y = top + 240 + heigh - StaticTools.GetYaxia(hs[i].moistureA, 120, heigh);
                        double x = xAxie + 2.2;
                        drawLine(cav_history, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                        if (i % _uint == 0 || i == start + 1)
                            drawText(cav_history, hs[i].time.ToShortTimeString(), x - 15, top+ heigh + 5 + 240, 12, 28);
                        xAxie = x;
                        yAxie = y;
                    }
                }
                #endregion

                #region mos NH
                if (hs.Count > 0)
                {
                    double xAxie = 25;
                    double yAxie = 0;

                    int start = 0;
                    yAxie = top + 480 + heigh - StaticTools.GetYaxia(hs[0].temperatureE, 120, heigh);

                    int _uint = 36;
                    for (int i = start + 1; i < hs.Count; i++)
                    {
                        double y = top + 480 + heigh - StaticTools.GetYaxia(hs[i].temperatureE, 120, heigh);
                        double x = xAxie + 2.2;
                        drawLine(cav_history, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                        if (i % _uint == 0 || i == start + 1)
                            drawText(cav_history, hs[i].time.ToShortTimeString(), x - 15, top + heigh + 5 + 480, 12, 28);
                        xAxie = x;
                        yAxie = y;
                    }
                }
                #endregion
                index++;
                //return;
            }

        }
        private void drawLine(Canvas cav, Point sp, Point ep, Brush brush)
        {
            cav.Dispatcher.Invoke(() =>
            {
                Line l = new Line();
                l.X1 = sp.X;
                l.Y1 = sp.Y;
                l.X2 = ep.X;
                l.Y2 = ep.Y;
                l.Stroke = brush;

                cav.Children.Add(l);
            });
        }
        private void drawLimit(Canvas cav, double y, int width, Brush brush)
        {
            cav.Dispatcher.Invoke(() =>
            {
                Line l = new Line();
                l.X1 = 25;
                l.Y1 = y;
                l.X2 = width + 25;
                l.Y2 = y;
                l.Stroke = brush;
                l.StrokeThickness = 0.5;

                cav.Children.Add(l);
            });
        }
        private void drawRectang(Canvas cav, Point sp, int width, int heigh, string unit, string title,Brush brush)
        {
            cav.Dispatcher.Invoke(() =>
            {
                Rectangle rect = new Rectangle();
                rect.Fill = brush;
                rect.Stroke = Brushes.Gray;
                rect.StrokeThickness = 0.5;
                var x = sp.X;
                var y = sp.Y;
                rect.Width = width;
                rect.Height = heigh;
                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                cav.Children.Add(rect);

                drawText(cav, unit, sp.X - 15, sp.Y, 12, 20);
                drawText(cav, title, width / 2 - title.Length, sp.Y - 22, 16, 200);

                drawLimit(cav, sp.Y + heigh - StaticTools.GetYaxia(_maxT, 120, heigh), width, Brushes.Gray);
                drawLimit(cav, sp.Y + heigh - StaticTools.GetYaxia(_minT, 120, heigh), width, Brushes.Gray);

                double left = x;
                while (left <= width)
                {
                    double xl = left;
                    double yl = sp.Y + heigh;
                    Line l = new Line();
                    l.X1 = xl;
                    l.Y1 = yl;
                    l.X2 = xl;
                    l.Y2 = yl - 5;
                    l.Stroke = Brushes.Gray;
                    rect.StrokeThickness = 0.5;
                    cav.Children.Add(l);

                    left += 40;
                }
            });
        }
        private void drawRang(Canvas cav, Point sp, int width, int heigh,Brush brush)
        {
            cav.Dispatcher.Invoke(() =>
            {
                Point p = new Point(sp.X, sp.Y);
                drawRectang(cav, p, width, heigh, string.Format("{0}", "°C"), string.Format("{0}", "温度趋势图（度）"), brush);
                p = new Point(sp.X, sp.Y + 240);
                drawRectang(cav, p, width, heigh, string.Format("{0}", "度"), string.Format("{0}", "湿度趋势图（度）"), brush);
                p = new Point(sp.X, sp.Y + 480);
                drawRectang(cav, p, width, heigh, string.Format("{0}", "%"), string.Format("{0}", "氨气含量趋势图（%）"), brush);
            });
        }
        private void drawText(Canvas cav, string data, double x, double y, double FontSize, double width)
        {
            cav.Dispatcher.Invoke(() =>
            {
                TextBlock tb = new TextBlock();
                tb.Foreground = Brushes.Black;
                tb.TextAlignment = TextAlignment.Center;
                tb.Width = width;
                tb.Height = 20;
                tb.FontSize = FontSize;
                tb.Text = string.Format("{0}", data);
                Thickness th = new Thickness(x, y, 0, 0);
                tb.Margin = th;
                cav.Children.Add(tb);
            });
        }

        private int getRemoteID(int nodeid)
        {
            if (nodeid == 101) return 101;
            if (nodeid == 201) return 202;

            return nodeid;
        }

        private void updateSenser()
        {
            //getData();
            //return;
            List<string> areas = StaticTools.GetAllArea();

            foreach (string area in areas)
            {
                string areaId = StaticTools.getAreaID(area);
                List<string> nodes = StaticTools.GetAllNodesByArea(area);

                foreach (string node in nodes)
                {
                    History it = new History();

                    int id = Convert.ToInt32(areaId) * 100 + Convert.ToInt32(node);
                    if (items[id.ToString()].historys.Count >= _maxHistorys)
                        items[id.ToString()].historys.RemoveAt(0);

                    double[] data = _bill.getNodeData(id);


                    try
                    {
                        if (data == null)
                        {
                            double temp = 0;
                            double moist = 0;
                            double NH = 0;
                            double data1 = 0;
                            double data2 = 0;
                            double data3 = 0;


                            Status status = checkData(new History() { id = id, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, moistureE = data1, temperatureE2 = data2, moistureE2 = data3, time = DateTime.Now });
                            items[id.ToString()].historys.Add(new History() { id = id, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, moistureE = data1, temperatureE2 = data2, moistureE2 = data3, time = DateTime.Now });
                            items[id.ToString()].status = Status.OffLine;
                            DataGridShow error = new DataGridShow();
                            error.area = items[id.ToString()].area;
                            error.id = items[id.ToString()].id;
                            error.moistureA = moist;
                            error.temperatureE = NH;
                            error.moistureE = data1;
                            error.temperatureE2 = data2;
                            error.moistureE2 = data3;
                            error.status = status;
                            error.temperatureA = temp;
                            error.time = DateTime.Now;
                            errorData.Dispatcher.Invoke(() => { errorData.Items.Add(error); });

                            DataGridShow dgs = new DataGridShow()
                            {
                                status = status,
                                area = items[id.ToString()].area,
                                id = items[id.ToString()].id,
                                moistureA = moist,
                                temperatureE = NH,
                                temperatureA = temp,
                                moistureE = data1,
                                temperatureE2 = data2,
                                moistureE2 = data3,
                                time = DateTime.Now,
                                isShow = false
                            };
                            _mytestdata.Add(dgs);
                            if (status == Status.Error)
                                if (!_errorList.ContainsKey(id.ToString()))
                                {
                                    _errorList.Add(id.ToString(), true);
                                }

                            cb_update.Dispatcher.Invoke(() => {
                                if (cb_update.IsChecked == true)
                                    _cloud.PostMethodMulti(id.ToString(), temp, moist, NH);
                            });
                            alert(items[id.ToString()].id.ToString());
                        }
                        else {
                            double temp = data[2];
                            double moist = data[3];
                            double NH = data[4];
                            double data1 = data[5];
                            double data2 = data[6];
                            double data3 = data[7];

                            Status status = checkData(new History() { id = id, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, moistureE = data1, temperatureE2 = data2,moistureE2 = data3, time = DateTime.Now });
                            items[id.ToString()].historys.Add(new History() { id = id, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, moistureE = data1, temperatureE2 = data2, moistureE2 = data3, time = DateTime.Now });

                            if (status == Status.Warning || status == Status.Error)
                            {
                                items[id.ToString()].ErrorCount++;
                                DataGridShow error = new DataGridShow();
                                error.area = items[id.ToString()].area;
                                error.id = items[id.ToString()].id;
                                error.moistureA = moist;
                                error.temperatureE = NH;
                                error.status = status;
                                error.moistureE = data1;
                                error.temperatureE2 = data2;
                                error.moistureE2 = data3;
                                error.temperatureA = temp;
                                error.time = DateTime.Now;
                                errorData.Dispatcher.Invoke(() => { errorData.Items.Add(error); });
                                if (items[id.ToString()].ErrorCount > 2)
                                {
                                    if (items[id.ToString()].lastStatus == Status.Regular)
                                    {
                                        items[id.ToString()].lastStatus = Status.Warning;
                                        int remoteid = getRemoteID(id);
                                        startFan(remoteid/100, remoteid%100);
                                        startFan(remoteid / 100, remoteid % 100);

                                        alert(items[id.ToString()].id.ToString());
                                    }
                                }
                            }
                            else
                            {
                                if (items[id.ToString()].lastStatus == Status.Warning)
                                {
                                    items[id.ToString()].lastStatus = Status.Regular;
                                    int remoteid = getRemoteID(id);
                                    stopFan(remoteid / 100, remoteid % 100);
                                    stopFan(remoteid / 100, remoteid % 100);
                                    items[id.ToString()].ErrorCount = 0;
                                }
                            }

                            DataGridShow dgs = new DataGridShow()
                            {
                                status = status,
                                area = items[id.ToString()].area,
                                id = items[id.ToString()].id,
                                moistureA = moist,
                                temperatureE = NH,
                                temperatureA = temp,
                                moistureE = data1,
                                temperatureE2 = data2,
                                moistureE2 = data3,
                                time = DateTime.Now,
                                isShow = false
                            };
                            _mytestdata.Add(dgs);
                            if (status == Status.Error)
                                if (!_errorList.ContainsKey(id.ToString()))
                                {
                                    _errorList.Add(id.ToString(), true);
                                }

                            cb_update.Dispatcher.Invoke(() => {
                                if (cb_update.IsChecked == true)
                                    _cloud.PostMethodMulti(id.ToString(), temp, moist, NH);
                            });
                        }
                    }
                    catch
                    { }
                }
            }
            //do cloud update!
        }

        private void updateShow()
        {
            cav_show.Dispatcher.Invoke((Action)(() =>
            {
                int leftoff = 0;
                int topoff = 20;
                int width =380;
                int heigh = 120;

                int count = cav_show.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    cav_show.Children.RemoveAt(0);
                }
                cav_show.Height = 400;
                foreach (string key in _checkShow.Keys)
                {
                    if (_checkShow[key])
                    {
                        foreach (string i in items.Keys)
                        {
                            //if (StaticTools.checkNode(key, (items[i].id%100).ToString()))
                            if (key == items[i].area)
                            {
                                int index = items[i].historys.Count;
                                if (index > 0)
                                {
                                    TextBlock tb = new TextBlock();
                                    var status = checkData(items[i].historys[index - 1]);
                                    if (status == Status.Regular)
                                    {
                                        tb.Foreground = Brushes.White;
                                        tb.Background = Brushes.Blue;
                                    }
                                    else if(status==Status.Error)
                                    {
                                        tb.Foreground = Brushes.White;
                                        tb.Background = Brushes.Red;
                                    }
                                    else
                                    {
                                        tb.Foreground = Brushes.White;
                                        tb.Background = Brushes.Yellow;
                                    }

                                    if (items[i].status == Status.OffLine)
                                    {
                                        tb.Foreground = Brushes.White;
                                        tb.Background = Brushes.Gray;
                                    }

                                    int itemCount = 0;

                                    string str = items[i].historys[index - 1].id.ToString() + "号设备";
                                    if (_isShow_tempA == 1)
                                    {
                                        string show = items[i].historys[index - 1].temperatureA.ToString() == "6553.5" ? "N/A" : items[i].historys[index - 1].temperatureA.ToString();
                                        if (itemCount % 2 == 0)
                                            str += "\n";
                                        else str += "        ";
                                        str += "空气温度：" + show + "·C";
                                        itemCount++;
                                    }
                                    if (_isShow_moistA == 1)
                                    {
                                        string show = items[i].historys[index - 1].moistureA.ToString() == "6553.5" ? "N/A" : items[i].historys[index - 1].moistureA.ToString();
                                        if (itemCount % 2 == 0)
                                            str += "\n";
                                        else str += "        ";
                                        str += "空气湿度：" + show + "";
                                        itemCount++;
                                    }
                                    if (_isShow_tempE == 1)
                                    {
                                        string show = items[i].historys[index - 1].temperatureE.ToString() == "6553.5" ? "N/A" : items[i].historys[index - 1].temperatureE.ToString();
                                        if (itemCount % 2 == 0)
                                            str += "\n";
                                        else str += "        ";
                                        str += "土壤温度1：" + show + "·C";
                                        itemCount++;
                                    }
                                    if (_isShow_moistE2 == 1)
                                    {
                                        string show = items[i].historys[index - 1].moistureE.ToString() == "6553.5" ? "N/A" : items[i].historys[index - 1].moistureE.ToString();
                                        if (itemCount % 2 == 0)
                                            str += "\n";
                                        else str += "        ";
                                        str += "土壤湿度1：" + show + "";
                                        itemCount++;
                                    }
                                    if (_isShow_moistE == 1)
                                    {
                                        string show = items[i].historys[index - 1].temperatureE2.ToString() == "6553.5" ? "N/A" : items[i].historys[index - 1].moistureE2.ToString();
                                        if (itemCount % 2 == 0)
                                            str += "\n";
                                        else str += "        ";
                                        str += "土壤温度2：" + show + "·C";
                                        itemCount++;
                                    }
                                    if (_isShow_tempE2 == 1)
                                    {
                                        string show = items[i].historys[index - 1].moistureE2.ToString() == "6553.5" ? "N/A" : items[i].historys[index - 1].temperatureE2.ToString();
                                        if (itemCount % 2 == 0)
                                            str += "\n";
                                        else str += "        ";
                                        str += "土壤湿度2：" + show + "";
                                        itemCount++;
                                    }


                                    tb.Text = str;
                                    tb.Height = heigh;
                                    tb.Width = width;
                                    tb.FontSize = 20;

                                    Thickness th = new Thickness(20 + leftoff, 20 + topoff, 0, 0);
                                    tb.Margin = th;

                                    if (leftoff + width * 2 + 20 > cav_show.Width)
                                    {
                                        topoff += heigh + 20;
                                        leftoff = 0;
                                        if ((topoff+ heigh + 20) < cav_show.Height)
                                            cav_show.Height += heigh + 20;
                                    }
                                    else
                                        leftoff += width + 20;

                                    cav_show.Children.Add(tb);
                                }
                            }
                        }
                    }
                }
            }));
        }

        private void getData()
        {
            string s = "AAA555000202024521";
            byte[] cmd = strToToHexByte(s);

            //byte[] cmd = Cmds.GetCmd(Cmds.ReadAllNodeData);

            byte[] recv = _device.sp_DataSender(cmd);
            if (recv == null)
            {
                //txt_recv.Text = "error";
                Console.WriteLine("未读出设备信息，请检查设备是否正常！");
            }
            else
            {
                //txt_recv.Text = "OK";
            }
            AnalysisData(recv);
        }

        //temp last status
        Status lastStatus = Status.Regular;
        int errorCount = 0;
        Status lastStatus1 = Status.Regular;
        int errorCount1 = 0;
        private void AnalysisData(byte[] data)
        {
            try
            {
                if (data == null)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        double temp = 0;
                        double moist = 0;
                        double NH = 0;

                        Status status = checkData(new History() { id = i, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, time = DateTime.Now });
                        items[i.ToString()].historys.Add(new History() { id = i, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, time = DateTime.Now });

                        DataGridShow error = new DataGridShow();
                        error.area = items[i.ToString()].area;
                        error.id = items[i.ToString()].id;
                        error.moistureA = moist;
                        error.temperatureE = NH;
                        error.status = status;
                        error.temperatureA = temp;
                        error.time = DateTime.Now;
                        errorData.Dispatcher.Invoke(() => { errorData.Items.Add(error); });

                        DataGridShow dgs = new DataGridShow()
                        {
                            status = status,
                            area = items[i.ToString()].area,
                            id = items[i.ToString()].id,
                            moistureA = moist,
                            temperatureE = NH,
                            temperatureA = temp,
                            time = DateTime.Now,
                            isShow = false
                        };
                        _mytestdata.Add(dgs);
                        if(status == Status.Error)
                        if (!_errorList.ContainsKey(i.ToString()))
                        {
                            _errorList.Add(i.ToString(), true);
                        }
                        //_cloud.PostMethodMulti(i.ToString(), temp, moist, NH);
                    }
                }
                else {
                    if (data.Length != 41)
                    {
                        Console.WriteLine("data length not fit!!!!!!Length is "+data.Length);
                        return;
                    }
                    int id = 1;
                    for (int i = 7; i < data.Length - 2; i = i + 16)
                    {
                        int _id = id - 1;
                        double temp = (data[i + 4] * 256 + data[i + 5]) / 10.0;
                        double moist = (data[i + 6] * 256 + data[i + 7]) / 10.0;
                        double NH = (data[i + 8] * 256 + data[i + 9]) / 10.0;
                        Console.WriteLine("发送中....温度：{0}， 湿度：{1}， 氨气浓度：{2}", temp, moist, NH);

                        Status status = checkData(new History() { id = id, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, time = DateTime.Now });
                        items[_id.ToString()].historys.Add(new History() { id = id, moistureA = moist, temperatureE = NH, status = Status.Error, temperatureA = temp, time = DateTime.Now });

                        if (status == Status.Warning||status == Status.Error)
                        {
                            errorCount++;
                            errorCount1++;
                            DataGridShow error = new DataGridShow();
                            error.area = items[_id.ToString()].area;
                            error.id = items[_id.ToString()].id;
                            error.moistureA = moist;
                            error.temperatureE = NH;
                            error.status = status;
                            error.temperatureA = temp;
                            error.time = DateTime.Now;
                            errorData.Dispatcher.Invoke(() => { errorData.Items.Add(error); });
                            if (_id == 1&&errorCount>2)
                            {
                                if (lastStatus == Status.Regular)
                                {
                                    lastStatus = Status.Warning;
                                    startFan(2, (_id + 2));
                                    startFan(2, (_id + 2));
                                }
                            }
                            if (_id == 0 && errorCount1 > 2)
                            {
                                if (lastStatus1 == Status.Regular)
                                {
                                    lastStatus1 = Status.Warning;
                                    startFan(2, (_id + 2));
                                    startFan(2, (_id + 2));
                                }
                            }
                        }
                        else
                        {
                            if (_id == 1)
                            {
                                if (lastStatus == Status.Warning)
                                {
                                    lastStatus = Status.Regular;
                                    stopFan(2, (_id + 2));
                                    stopFan(2, (_id + 2));
                                    errorCount = 0;
                                }
                            }
                            if (_id == 0)
                            {
                                if (lastStatus1 == Status.Warning)
                                {
                                    lastStatus1 = Status.Regular;
                                    stopFan(2, (_id + 2));
                                    stopFan(2, (_id + 2));
                                    errorCount1 = 0;
                                }
                            }
                        }

                        DataGridShow dgs = new DataGridShow()
                        {
                            status = status,
                            area = items[_id.ToString()].area,
                            id = items[_id.ToString()].id,
                            moistureA = moist,
                            temperatureE = NH,
                            temperatureA = temp,
                            time = DateTime.Now,
                            isShow = false
                        };
                        _mytestdata.Add(dgs);
                        if (status == Status.Error)
                            if (!_errorList.ContainsKey(_id.ToString()))
                            {
                                _errorList.Add(_id.ToString(), true);
                            }
                        Console.WriteLine("准备发送到云端！");
                        _cloud.PostMethodMulti(id.ToString(), temp, moist, NH);
                        id++;
                    }
                }
            }
            catch
            { }
        }



        private Status checkData(History item)
        {
            if (item.temperatureA < 6553)
            {
                if (item.temperatureA > (_maxT + _DivT) || item.temperatureA < (_minT - _DivT)) return Status.Error;
                if (item.temperatureA > _maxT || item.temperatureA < _minT) return Status.Warning;
            }
            if (item.moistureA < 6553)
            {
                if (item.moistureA > (_maxM + _DivM) || item.moistureA < (_minM - _DivM)) return Status.Error;
                if (item.moistureA > _maxM || item.moistureA < _minM) return Status.Warning;
            }
            if(item.temperatureE<6553)
            {
                if (item.temperatureE > (_maxT + _DivT) || item.temperatureE < (_minT - _DivT)) return Status.Error;
                if (item.temperatureE > _maxT || item.temperatureE < _minT) return Status.Warning;
            }
            if (item.moistureE < 6553)
            {
                if (item.moistureE > (_maxM + _DivM) || item.moistureE < (_minM - _DivM)) return Status.Error;
                if (item.moistureE > _maxM || item.moistureE < _minM) return Status.Warning;
            }
            if (item.temperatureE2 < 6553)
            {
                if (item.temperatureE2 > (_maxT + _DivT) || item.temperatureE2 < (_minT - _DivT)) return Status.Error;
                if (item.temperatureE2 > _maxT || item.temperatureE2 < _minT) return Status.Warning;
            }
            if (item.moistureE2 < 6553)
            {
                if (item.moistureE2 > (_maxM + _DivM) || item.moistureE2 < (_minM - _DivM)) return Status.Error;
                if (item.moistureE2 > _maxM || item.moistureE2 < _minM) return Status.Warning;
            }

            return Status.Regular;
        }



        private List<History> GetHistoryChatData(string id, int time)
        {
            List<History> data = items[id].historys;
            List<History> ls = new List<History>();

            DateTime nowdate = DateTime.Now;
            DateTime startdate = nowdate.AddHours(-time);

            ls = data.Where((a) => a.time > startdate).ToList();

            return ls;
        }

        private void drawHistory(Point sp, int width, int heigh, string type)
        {
            cav_history.Dispatcher.Invoke(() =>
            {
                int count = cav_history.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    cav_history.Children.RemoveAt(0);
                }

                string title = string.Empty;
                if (type == "氨气")
                {
                    title = string.Format("{0}", type + "浓度趋势图（%）");
                }
                else if (type == "温度")
                {
                    title = string.Format("{0}", type + "趋势图（度）");
                }
                else
                {
                    title = string.Format("{0}", type + "趋势图（度）");
                }
                drawRectang(cav_history, sp, width, heigh, string.Format("{0}", "C"), title, Brushes.White);
            });
        }

        private void drawHistoryData(List<History> dir, string type)
        {
            cav_history.Dispatcher.Invoke(() =>
            {
                double top = 25;

                double heigh = 520;

                if (dir.Count > 0)
                {
                    int count = dir.Count > 24 ? 24 : dir.Count;
                    int index = dir.Count / count;
                    double xAxie = 25;
                    double yAxie = 0;

                    int start = 0;
                    if (type == "温度")
                        yAxie = top + heigh - StaticTools.GetYaxia(dir[0].temperatureA, 120, heigh);
                    else if (type == "湿度")
                        yAxie = top + heigh - StaticTools.GetYaxia(dir[0].moistureA, 120, heigh);
                    else
                        yAxie = top + heigh - StaticTools.GetYaxia(dir[0].temperatureE, 120, heigh);

                    for (int i = start + 1; i < count; i++)
                    {
                        double y = 0;
                        if (type == "温度")
                            y = top + heigh - StaticTools.GetYaxia(dir[i * index].temperatureA, 120, heigh);
                        else if (type == "湿度")
                            y = top + heigh - StaticTools.GetYaxia(dir[i * index].moistureA, 120, heigh);
                        else
                            y = top + heigh - StaticTools.GetYaxia(dir[i * index].temperatureE, 120, heigh);

                        double x = xAxie + 40;
                        drawLine(cav_history, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                        drawText(cav_history, dir[i * index].time.ToShortTimeString(), x - 15, heigh + 30, 12, 30);
                        xAxie = x;
                        yAxie = y;
                    }
                }
            });
        }


        #region UI event method
        private void btn_seach_Click(object sender, RoutedEventArgs e)
        {
            //string itemid = com_item.SelectedValue.ToString();
            //string passTime = com_time.SelectedValue.ToString().Substring(2, com_time.SelectedValue.ToString().Length - 2);
            //int x = passTime.Length;
            //passTime = passTime.Substring(0, x - 2);
            //int passT = Convert.ToInt32(passTime);
            //string Type = com_type.SelectedValue.ToString();


            //drawHistory(new Point(25, 45), (int)cav_history.Width - 60, (int)cav_history.Height - 100, Type);
            //List<History> ls = GetHistoryChatData(itemid, passT);
            //drawHistoryData(ls, Type);
            //drawHistoryData(items[itemid], Type);
        }

        private void btn_updateSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cb_temp.IsChecked == true) _isShow_tempA = 1;
                else _isShow_tempA = 0;
                if (cb_moist.IsChecked == true) _isShow_moistA = 1;
                else _isShow_moistA = 0;
                if (cb_ch3.IsChecked == true) _isShow_tempE = 1;
                else _isShow_tempE = 0;
                if (cb_data1.IsChecked == true) _isShow_moistE = 1;
                else _isShow_moistE = 0;
                if (cb_data2.IsChecked == true) _isShow_tempE2 = 1;
                else _isShow_tempE2 = 0;
                if (cb_data3.IsChecked == true) _isShow_moistE2 = 1;
                else _isShow_moistE2 = 0;

                StaticTools.SaveConfig("showT", _isShow_tempA.ToString());
                StaticTools.SaveConfig("showM", _isShow_moistA.ToString());
                StaticTools.SaveConfig("showC", _isShow_tempE.ToString());
                StaticTools.SaveConfig("showD1", _isShow_moistE.ToString());
                StaticTools.SaveConfig("showD2", _isShow_tempE2.ToString());
                StaticTools.SaveConfig("showD3", _isShow_moistE2.ToString());

                if (cb_play.IsChecked == true) _play = 1;
                else _play = 0;
                StaticTools.SaveConfig("play", _play.ToString());

                double maxT = Convert.ToDouble(txt_temp_max.Text);
                this._maxT = maxT;
                StaticTools.SaveConfig("maxT", maxT.ToString());

                double minT = Convert.ToDouble(txt_temp_min.Text);
                this._minT = minT;
                StaticTools.SaveConfig("minT", minT.ToString());

                double maxM = Convert.ToDouble(txt_mos_max.Text);
                this._maxM = maxM;
                StaticTools.SaveConfig("maxM", maxM.ToString());

                double minM = Convert.ToDouble(txt_mos_min.Text);
                this._minM = minM;
                StaticTools.SaveConfig("minM", minM.ToString());

                double maxN = Convert.ToDouble(txt_RH_max.Text);
                this._maxN = maxN;
                StaticTools.SaveConfig("maxN", maxN.ToString());

                double minN = Convert.ToDouble(txt_RH_min.Text);
                this._minN = minN;
                StaticTools.SaveConfig("minN", minN.ToString());

                double divT = Convert.ToDouble(txt_temp_deviation.Text);
                this._DivT = divT;
                StaticTools.SaveConfig("DivT", divT.ToString());

                double divM = Convert.ToDouble(txt_mos_deviation.Text);
                this._DivM = divM;
                StaticTools.SaveConfig("DivM", divM.ToString());

                double divN = Convert.ToDouble(txt_RH_deviation.Text);
                this._DivN = divN;
                StaticTools.SaveConfig("DivN", divN.ToString());

                //_cloud.PostMethodUpdateParam(_maxT, _minT, _maxM, _minM, _maxN, _minN);
                _cloud.PostMethodUpdateParam(_maxT + _DivT, _minT - _DivT, _maxM + _DivM, _minM - _DivM, _maxN + _DivN, _minN - _DivN);

                MessageBox.Show("修改成功！");
            }
            catch
            {
                MessageBox.Show("修改错误，请检查输入是否为数字！");
            }
        }

        private void btn_commit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StaticTools.SaveConfig("host", txt_addr.Text);
                StaticTools.SaveConfig("i_updateDate", txt_data.Text);
                StaticTools.SaveConfig("i_updateUser", txt_update.Text);
                StaticTools.SaveConfig("i_insertUser", txt_insert.Text);
                StaticTools.SaveConfig("i_deleteUser", txt_delete.Text);
                StaticTools.SaveConfig("i_login", txt_login.Text);
                //StaticTools.SaveConfig("port", StaticTools.getPortvalue(cob_port.SelectedIndex).ToString());
                if (cb_play.IsChecked == true) _play = 1;
                else _play = 0;

                StaticTools.SaveConfig("play", _play.ToString());

                MessageBox.Show("修改成功！");
            }
            catch
            {
            }
        }



        private void btn_adduser_Click(object sender, RoutedEventArgs e)
        {
            AddUser au = new AddUser(_sign, "add", "");
            au.pevent += Au_pevent;
            au.Show();
        }

        private void Au_pevent(object sender, string result)
        {
            MessageBox.Show("");
        }

        private void btn_updateuser_Click(object sender, RoutedEventArgs e)
        {
            AddUser au = new AddUser(_sign, "edit", "");
            au.pevent += Au_pevent;
            au.Show();
        }

        private void btn_deluser_Click(object sender, RoutedEventArgs e)
        {
            //if (txt_id.Text == "")
            //{
            //    MessageBox.Show("账号ID不能为空！");
            //    return;
            //}

            //try
            //{
            //    _cloud.DeleteUser(_sign, txt_id.Text);

            //    MessageBox.Show("操作成功！");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("错误！");
            //}
        }
        #endregion


        private void cmb_areachat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = cmb_nodechat.Items.Count;
            for (int i = 0; i < count; i++)
                cmb_nodechat.Items.RemoveAt(0);

            List<string> nodeList = StaticTools.GetAllNodesByArea(e.AddedItems[0].ToString());
            _AreaChat = e.AddedItems[0].ToString();
            foreach (string s in nodeList)
            {
                cmb_nodechat.Items.Add(s);
            }
            if (cmb_nodechat.Items.Count > 0)
            {
                _NodeChat = cmb_nodechat.Items[0].ToString();
                cmb_nodechat.SelectedIndex = 0;
            }
        }

        private void cmb_nodechat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _NodeChat = e.AddedItems[0].ToString();
                updateChat(_AreaChat,_NodeChat);

            }
        }

        private void cmb_areatable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = cmb_nodetable.Items.Count;
            for (int i = 0; i < count; i++)
                cmb_nodetable.Items.RemoveAt(0);

            List<string> nodeList = StaticTools.GetAllNodesByArea(e.AddedItems[0].ToString());
            foreach (string s in nodeList)
            {
                cmb_nodetable.Items.Add(s);
            }
            if (cmb_nodetable.Items.Count > 0)
            {
                cmb_nodetable.SelectedIndex = 0;
            }
        }

        private void cmb_nodetable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (SrollBarDataGrid.Items.Count > 0)
            //{
            //    string area = cmb_areatable.SelectedValue.ToString();
            //    if (e.AddedItems.Count > 0)
            //    {
            //        string node = e.AddedItems[0].ToString();

            //        int index = 288 * 30 * Convert.ToInt32(node) + Convert.ToInt32(node);
            //        if (SrollBarDataGrid.Items.Count > index)
            //            updateTableshow(index);
            //    }
            //}
        }


        private void updateTableshow(int index)
        {
            if (index == 0)
            {
                SrollBarDataGrid.SelectedIndex = 0;
                SrollBarDataGrid.UpdateLayout();
                SrollBarDataGrid.ScrollIntoView(SrollBarDataGrid.SelectedItem);
            }
            if (SrollBarDataGrid.Items.Count > index + 30)
            {
                SrollBarDataGrid.SelectedIndex = index + 30;
                SrollBarDataGrid.UpdateLayout();
                SrollBarDataGrid.ScrollIntoView(SrollBarDataGrid.SelectedItem);
                SrollBarDataGrid.SelectedIndex = index;
            }
            else
            {
                SrollBarDataGrid.SelectedIndex = index;
                SrollBarDataGrid.UpdateLayout();
                SrollBarDataGrid.ScrollIntoView(SrollBarDataGrid.SelectedItem);
            }
        }

        private void btn_showAllUsers_Click(object sender, RoutedEventArgs e)
        {
            _users = _cloud.GetAllUsers();
            userData.Items.Clear();

            foreach (Users u in _users)
            {
                userData.Items.Add(new Users { id = u.id, name = u.name });
            }
        }

        private void btn_addEq_Click(object sender, RoutedEventArgs e)
        {
            string id = txt_eqId.Text;

            try { int temp = Convert.ToInt32(id); }
            catch
            {
                MessageBox.Show("请用数字标注设备ID！");
                return;
            }

            string name = txt_eqName.Text;
            string area = cmb_eqArea.SelectedValue.ToString().Split('(')[0];
            string areaid = cmb_eqArea.SelectedValue.ToString().Split('(')[1].Split(')')[0];
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("id不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("设备名称不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(area))
            {
                MessageBox.Show("请先添加区域！");
                return;
            }

            try
            {
                List<int> list = new List<int>();
                int _list_id = Convert.ToInt32(areaid) * 100 + Convert.ToInt32(id);
                list.Add(_list_id);
                if (!_bill.AddBSList(list))
                {
                    MessageBox.Show("设备更新失败，请检查设备是否连接！");
                    return;
                }
            }
            catch { MessageBox.Show("故障！"); }


            StaticTools.AddNode(areaid,area, id, name);

            update_cmd();
            MessageBox.Show("添加成功！");
        }

        private void update_cmd()
        {
            tv_cmd.Items.Clear();       
            List<string> areaList = StaticTools.GetAllArea();

            foreach (string area in areaList)
            {
                TreeViewItem newChild = new TreeViewItem();
                newChild.Header = area + "网络";
                List<string> nodelist = new List<string>();
                nodelist = StaticTools.GetAllNodesByArea(area);
                foreach (string s1 in nodelist)
                {
                    TreeViewItem newChildnode = new TreeViewItem();
                    newChildnode.Header = "设备编号：" + s1;
                    newChildnode.VerticalAlignment = VerticalAlignment.Center;
                    newChild.Items.Add(newChildnode);
                }
                tv_cmd.Items.Add(newChild);
            }

            ExpandInternal(tv_cmd);
        }

        private void btn_addArea_Click(object sender, RoutedEventArgs e)
        {
            try { int temp = Convert.ToInt32(txt_addareaID.Text); }
            catch
            {
                MessageBox.Show("请用数字标注网络ID！");
                return;
            }

            int id = StaticTools.getAreaNewID(txt_addareaID.Text);

            if (id < 0)
            {
                MessageBox.Show("网络数量已达到最大！");
                return;
            }
            else if (id == 0)
            {
                MessageBox.Show("网络ID重复，请更换！");
                return;
            }

            StaticTools.AddArea(txt_addarea.Text.ToString(), txt_addareaID.Text.ToString());
            int count = cmb_eqArea.Items.Count;
            for (int i = 0; i < count; i++)
            {
                cmb_eqArea.Items.RemoveAt(0);
            }
            List<string> areas = StaticTools.GetAllAreaShow();
            foreach (string s in areas)
            {
                cmb_eqArea.Items.Add(s);
            }
            cmb_eqArea.SelectedIndex = 0;
            update_cmd();
            MessageBox.Show("添加成功！");
        }

        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private static string hexByteToStr(byte[] hexByte)
        {
            string a = "";
            for (int i = 0; i < hexByte.Length; i++)
            {
                a += hexByte[i].ToString("X2") + " ";
            }
            return a;
        }


        private void btn_edt_Click(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    Users u = row.DataContext as Users;
                    string id = u.id;
                    AddUser au = new AddUser(_sign, "edit", id);
                    au.pevent += Au_pevent;
                    au.Show();
                }
        }

        private void btn_del_Click(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    Users u = row.DataContext as Users;
                    string id = u.id;

                    try
                    {
                        //_cloud.DeleteUser(_sign, id);

                        MessageBox.Show("操作成功！");
                    }
                    catch
                    {
                        MessageBox.Show("错误！");
                    }
                }
        }

        private void cb_1_Checked(object sender, RoutedEventArgs e)
        {
            var control = sender as CheckBox;
            _checkShow[control.Content.ToString()] = true;
        }

        private void cb_1_Unchecked(object sender, RoutedEventArgs e)
        {
            var control = sender as CheckBox;
            _checkShow[control.Content.ToString()] = false;
        }

        private void btn_read_def_Click(object sender, RoutedEventArgs e)
        {
            //List<int> data = new List<int>();
            //List<double[]> data1 = new List<double[]>();
            //List<int> data2 = new List<int>();
            //int data3 = 0;
            //double[] data4;
            //data.Add(0202);
            //data.Add(0203);
            //int id = 203;
            //int id1 = 205;
            bool result = false;

            //int freq = 430000000;
            //int bw = 7;
            //int sf = 10;
            //int rc = 2;
            //int check = 0;
            //int enc = 20;

            //int len = 200;
            //int addr = 0x40;

            //List<byte> ds = new List<byte>();
            //ds.Add(0x80);
            //ds.Add(0x01);
            
            //result = _bill.DeleteNodeList();
            //data4 = _bill.getNodeList();
            //result = _bill.AddBSList(data);
            result = _bill.Check();
            //result = _bill.DeleteBSList(data);
            //data1 = _bill.getAllNodeData();
            //data1 = _bill.getAllNodeDebug();
            //data2 = _bill.getBS();
            //data3 = _bill.getBSLength();
            //data2 = _bill.getBSList();
            //data4 = _bill.getNodeData(id);//status eunm
            //data4 = _bill.getNodeDebug(id);
            //result = _bill.UpdateNodeId(id, id1);
            //result = _bill.setBSLength(len);
            //result = _bill.setBS(freq, sf, bw, rc, check, enc);
            //result = _bill.setBSToNode(id, freq, sf, bw, rc, check, enc);
            //result = _bill.setBSToNode(id, id1, freq, sf, bw, rc, check, enc);
            //result = _bill.setNodeRom(id, addr, ds);
        }

        private void btn_read_Click(object sender, RoutedEventArgs e)
        {
            List<int> data = _bill.getBS();

            if (data == null)
            {
                MessageBox.Show("未读出设备信息，请检查设备是否正常！");
            }
            else
            {
                try
                {
                    int freq = data[0]/1000;
                    string bw = getBWShow(data[2].ToString());
                    int sf = data[1];
                    int rc = data[3];
                    int check = data[4];
                    int enc = data[5];
                    txt_fre.Text = freq.ToString();
                    txt_SF.SelectedValue = sf.ToString();
                    txt_BW.SelectedValue = bw.ToString();
                    txt_rate.SelectedValue = rc.ToString();
                    txt_uhua.IsChecked = (check == 1);
                    txt_enc.Text = enc.ToString();
                }
                catch
                {
                    MessageBox.Show("数据初始化失败！");
                }
            }
        }

        private void btn_set_Click(object sender, RoutedEventArgs e)
        {
            int freq = 430000000;
            int bw = 7;
            int sf = 10;
            int rc = 2;
            int check = 0;
            int enc = 20;
            try
            {
                freq = Convert.ToInt32(txt_fre.Text)*1000;
            }
            catch
            {
                MessageBox.Show("中心频点输入有误（10~10000）");
                return;
            }
            //freq = freq > 10000 ? 10000 : freq;
            //freq = freq < 10 ? 10 : freq;

            sf = Convert.ToInt32(txt_SF.SelectedValue.ToString());

            bw = getBW(txt_BW.SelectedValue.ToString());

            rc = Convert.ToInt32(txt_rate.SelectedValue.ToString());

            check = txt_uhua.IsChecked==true ? 1 : 0;

            try
            {
                enc = Convert.ToInt32(txt_enc.Text);
            }
            catch
            {
                MessageBox.Show("射频功率输入有误（-1~20）");
                return;
            }
            enc = enc > 20 ? 20 : enc;
            enc = enc < -1 ? -1 : enc;

            if (_bill.setBS(freq, sf, bw, rc, check, enc))
            {
                MessageBox.Show("BS设置成功！");
                return;
            }
            else
                MessageBox.Show("未读出设备信息，请检查设备是否正常！");
        }

        private int getBW(string index)
        {
            switch (index)
            {
                case "7.8": return 0;
                case "10.4": return 1;
                case "15.6": return 2;
                case "20.8": return 3;
                case "31.2": return 4;
                case "41.6": return 5;
                case "62.5": return 6;
                case "125": return 7;
                case "250": return 8;
                case "500": return 9;
                default: return 0;
            }
        }
        private string getBWShow(string index)
        {
            switch (index)
            {
                case "0": return "7.8";
                case "1": return "10.4";
                case "2": return "15.6";
                case "3": return "20.8";
                case "4": return "31.2";
                case "5": return "41.6";
                case "6": return "62.5";
                case "7": return "125";
                case "8": return "250";
                case "9": return "500";
                default: return "7.8";
            }
        }

        private void startFan(int group, int member)
        {
            try
            {
                byte[] cmd = Cmds.cmd_CmdStartFanByID(group, member);
                byte[] recv = _device.sp_DataSender(cmd);
                if (recv == null)
                {
                    Console.WriteLine("未读出设备信息，请检查设备是否正常！");
                    //MessageBox.Show("未读出设备信息，请检查设备是否正常！");
                }
                else if (recv.Length > 7 && recv[7] == 0x80)
                {
                    Console.WriteLine("未读出设备信息，请检查设备是否正常！time out!");
//                    MessageBox.Show("未读出设备信息，请检查设备是否正常！time out!");
                }
                else
                {
                    string Ref = txt_fre.Text;
                    string Rate = txt_rate.Text;
                    //string RF = txt_RF.Text;
                    string SF = txt_SF.Text;
                    string BW = txt_BW.Text;
                    string Enc = txt_enc.Text;
                    MessageBox.Show("操作成功！");
                }
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private void stopFan(int group, int member)
        {
            try
            {
                byte[] cmd = Cmds.cmd_CmdStopFanByID(group, member);
                byte[] recv = _device.sp_DataSender(cmd);
                if (recv == null)
                {
                    //MessageBox.Show("未读出设备信息，请检查设备是否正常！");
                }
                else if (recv.Length > 7 && recv[7] == 0x80)
                {
                    //MessageBox.Show("未读出设备信息，请检查设备是否正常！time out!");
                }
                else
                {
                    string Ref = txt_fre.Text;
                    string Rate = txt_rate.Text;
                    //string RF = txt_RF.Text;
                    string SF = txt_SF.Text;
                    string BW = txt_BW.Text;
                    string Enc = txt_enc.Text;
                    MessageBox.Show("操作成功！");
                }
            }
            catch
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_nodetable.SelectedValue == null) return;
            string id = cmb_nodetable.SelectedValue.ToString();
            string areaid = StaticTools.getAreaID(cmb_areatable.SelectedValue.ToString());

            id = (Convert.ToInt32(areaid) * 100 + Convert.ToInt32(id)).ToString();

            if (id == "全部")
                _historyID = "";
            else
                _historyID = id;
            updateTable(_historyID);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            IsRun = false;
            _device.Dispose();
            _bill.Dispose();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            //_device.setPortName(txt_port.Text);
            //Console.WriteLine(_device.connect());
            //_bill = new DeviceBill(_device);
            //start();
        }

        private byte[] Read(byte[] cmds)
        {
            byte[] recv = _device.sp_DataSender(cmds);

            return recv;
        }

        private void btn_del_Click_1(object sender, RoutedEventArgs e)
        {
            var cont = tv_cmd.SelectedValue as TreeViewItem;

            if (cont == null)
            {
                MessageBox.Show("请选中一个节点(网络)!");
                return;
            }

            var cont1 = cont.Parent as TreeViewItem;

            if (cont1 == null)
            {
                string node = "";
                string area = cont.Header.ToString().Substring(0, 2);

                if (StaticTools.DelNode(area, node))
                    MessageBox.Show("删除成功！");
                else MessageBox.Show("删除失败！");
            }
            else if (cont1 != null & cont != null)
            {
                string node = cont.Header.ToString().Split('：')[1];
                string area = cont1.Header.ToString().Substring(0, 2);

                try
                {
                    List<int> list = new List<int>();
                    string areaid = StaticTools.getAreaID(area);
                    int _list_id = Convert.ToInt32(areaid) * 100 + Convert.ToInt32(node);
                    list.Add(_list_id);
                    if (!_bill.DeleteBSList(list))
                    {
                        MessageBox.Show("设备更新失败，请检查设备是否连接！");
                        return;
                    }
                }
                catch { MessageBox.Show("故障！"); }

                if (StaticTools.DelNode(area, node))
                    MessageBox.Show("删除成功！");
                else MessageBox.Show("删除失败！");
            }

            update_cmd();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _bill.getNodeDebug(0101);
        }
    }
}
