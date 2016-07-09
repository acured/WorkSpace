using AgriManagement.tools;
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

        double _DivT = Config.DivT;
        double _DivM = Config.DivM;
        double _DivN = Config.DivN;

        string _Area = string.Empty;
        string _AreaChat = string.Empty;
        string _NodeChat = string.Empty;
        string _HistoryArea = string.Empty;

        //double _freq_data = Config.freq_data;
        //double _freq_chat = Config.freq_chat;
        double _freq_data = 0.5;
        double _freq_chat = 4;

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

        int sensorCount = 2;
        int _maxHistorys = 8640;//one days
        int _maxShow = 360;//one days

        Dictionary<string, Item> items = new Dictionary<string, Item>();
        Dictionary<string, List<History>> cloudData = new Dictionary<string, List<History>>();

        Dictionary<string, bool> _checkShow = new Dictionary<string, bool>();
        Dictionary<string, bool> _errorList = new Dictionary<string, bool>();

        List<DataGridShow> _mytestdata = new List<DataGridShow>();

        string _historyID = "0";

        Random ran = new Random();
        public MainWindow()
        {
            InitializeComponent();
            InitSensors();
            initUI();
            updateFacilityStatus();

            //errorData.ItemsSource = _errors;
            //errorData.Dispatcher.Invoke(() => { errorData.ItemsSource = _errors; });
            //errorData.ItemsSource = _errors;
            //app_cmd.Visibility = Visibility.Hidden;
        }

        private void testInitData()
        {
            DateTime start = DateTime.Now.AddDays(-30);
            DateTime end = DateTime.Now;
            InitSensors();

            while (start < end)
            {
                for (int i = 0; i < sensorCount; i++)
                {
                    History it = new History();
                    if (_device.updateSensorData(i.ToString(), ref it))//do device update
                    {
                        if (items.ContainsKey(i.ToString()))
                        {
                            it.time = start;
                            items[i.ToString()].status = Status.OnLine;
                            items[i.ToString()].historys.Add(it);
                        }
                    }
                    else
                    {
                        if (items.ContainsKey(i.ToString()))
                        {
                            it.time = start;
                            items[i.ToString()].status = Status.OffLine;
                            items[i.ToString()].historys.Add(it);
                            cloudData[i.ToString()].Add(it);                     
                        }
                    }
                }
                start = start.AddMinutes(5);
            }
        }
        public History CopyHistory(History h)
        {
            History hnew = new History();
            hnew.id = h.id;
            hnew.moisture = h.moisture;
            hnew.NH = h.NH;
            hnew.temperature = h.temperature;
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

            #region serial port
            //列出常用的波特率
            cob_port.Items.Add("9600");
            cob_port.Items.Add("19200");
            cob_port.Items.Add("56000");
            cob_port.Items.Add("115200");
            cob_port.SelectedIndex = StaticTools.getPortindex(port);


            for (int i = 0; i < 25; i++)
            {
                try
                {
                    SerialPort sp = new SerialPort("COM" + (i + 1).ToString());
                    sp.Open();
                    sp.Close();
                    cob_coms.Items.Add("COM" + (i + 1).ToString());
                    _device.setPortName("COM" + (i + 1).ToString());
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }
            if (cob_coms.Items.Count > 0)
                cob_coms.SelectedIndex = 0;
            #endregion

            #region init history Page
            List<string> areaList = StaticTools.GetAllArea();
            foreach (string s in areaList)
            {
                cmb_eqArea.Items.Add(s);
            }
            cmb_eqArea.SelectedIndex = 0;


            if (_play == 1) cb_play.IsChecked = true;
            else cb_play.IsChecked = false;
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
            _AreaChat = cmb_areachat.Items[0].ToString();

            cmb_areachat.SelectedIndex = 0;
            cmb_areatable.SelectedIndex = 0;


            cb_1.Content = areaList[0];
            cb_1.IsChecked = true;

            cb_2.Content = areaList[1];
            cb_2.IsChecked = false;

            cb_3.Content = areaList[2];
            cb_3.IsChecked = false;

            cb_4.Content = areaList[3];
            cb_4.IsChecked = false;

            update_cmd();
        }

        private void updateFacilityStatus()
        {
            tv_status1.Items.Clear();
            tv_status2.Items.Clear();
            tv_status3.Items.Clear();
            tv_status4.Items.Clear();

            ell_moniter.Fill = Brushes.Red;

            List<string> areaList = StaticTools.GetAllArea();

            TreeViewItem newChild = new TreeViewItem();
            newChild.Header = areaList[0] + "网络";

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
            blueBrush1.Color = Colors.Blue;
            blueRectangle1.StrokeThickness = 1;
            blueRectangle1.Stroke = blueBrush1;
            blueRectangle1.Fill = blueBrush1;

            sp1.Children.Add(newChildnode1);
            sp1.Children.Add(blueRectangle1);
            newChild.Items.Add(sp1);

            List<string> nodelist = new List<string>();
            nodelist = StaticTools.GetAllNodesByArea(areaList[0]);
            foreach (string s1 in nodelist)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Height = 40;

                TreeViewItem newChildnode = new TreeViewItem();
                newChildnode.Header = "编号：" + s1 + " 状态：";
                newChildnode.VerticalAlignment = VerticalAlignment.Center;
                Ellipse blueRectangle = new Ellipse();
                blueRectangle.Height = 15;
                blueRectangle.Width = 15;

                SolidColorBrush blueBrush = new SolidColorBrush();
                blueBrush.Color = Colors.Blue;
                blueRectangle.StrokeThickness = 1;
                blueRectangle.Stroke = blueBrush;
                blueRectangle.Fill = blueBrush;
                                
                sp.Children.Add(newChildnode);
                sp.Children.Add(blueRectangle);
                newChild.Items.Add(sp);
            }
            tv_status1.Items.Add(newChild);
            ExpandInternal(tv_status1);

            newChild = new TreeViewItem();
            newChild.Header = areaList[1] + "网络";
            
            sp1 = new StackPanel();
            sp1.Orientation = Orientation.Horizontal;
            sp1.Height = 40;

            newChildnode1 = new TreeViewItem();
            newChildnode1.Header = "工控端：";
            newChildnode1.VerticalAlignment = VerticalAlignment.Center;
            blueRectangle1 = new Ellipse();
            blueRectangle1.Height = 15;
            blueRectangle1.Width = 15;

            blueBrush1 = new SolidColorBrush();
            blueBrush1.Color = Colors.Blue;
            blueRectangle1.StrokeThickness = 1;
            blueRectangle1.Stroke = blueBrush1;
            blueRectangle1.Fill = blueBrush1;

            sp1.Children.Add(newChildnode1);
            sp1.Children.Add(blueRectangle1);
            newChild.Items.Add(sp1);

            nodelist = new List<string>();
            nodelist = StaticTools.GetAllNodesByArea(areaList[1]);
            foreach (string s1 in nodelist)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Height = 40;

                TreeViewItem newChildnode = new TreeViewItem();
                newChildnode.Header = "编号：" + s1 + " 状态：";
                newChildnode.VerticalAlignment = VerticalAlignment.Center;
                Ellipse blueRectangle = new Ellipse();
                blueRectangle.Height = 15;
                blueRectangle.Width = 15;
                SolidColorBrush blueBrush = new SolidColorBrush();
                blueBrush.Color = Colors.Blue;
                blueRectangle.StrokeThickness = 1;
                blueRectangle.Stroke = blueBrush;
                blueRectangle.Fill = blueBrush;

                sp.Children.Add(newChildnode);
                sp.Children.Add(blueRectangle);
                newChild.Items.Add(sp);
            }
            tv_status2.Items.Add(newChild);
            ExpandInternal(tv_status2);

            newChild = new TreeViewItem();
            newChild.Header = areaList[2] + "网络";

            sp1 = new StackPanel();
            sp1.Orientation = Orientation.Horizontal;
            sp1.Height = 40;

            newChildnode1 = new TreeViewItem();
            newChildnode1.Header = "工控端：";
            newChildnode1.VerticalAlignment = VerticalAlignment.Center;
            blueRectangle1 = new Ellipse();
            blueRectangle1.Height = 15;
            blueRectangle1.Width = 15;

            blueBrush1 = new SolidColorBrush();
            blueBrush1.Color = Colors.Blue;
            blueRectangle1.StrokeThickness = 1;
            blueRectangle1.Stroke = blueBrush1;
            blueRectangle1.Fill = blueBrush1;

            sp1.Children.Add(newChildnode1);
            sp1.Children.Add(blueRectangle1);
            newChild.Items.Add(sp1);

            nodelist = new List<string>();
            nodelist = StaticTools.GetAllNodesByArea(areaList[2]);
            foreach (string s1 in nodelist)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Height = 40;

                TreeViewItem newChildnode = new TreeViewItem();
                newChildnode.Header = "编号：" + s1 + " 状态：";
                newChildnode.VerticalAlignment = VerticalAlignment.Center;
                Ellipse blueRectangle = new Ellipse();
                blueRectangle.Height = 15;
                blueRectangle.Width = 15;
                SolidColorBrush blueBrush = new SolidColorBrush();
                blueBrush.Color = Colors.Blue;
                blueRectangle.StrokeThickness = 1;
                blueRectangle.Stroke = blueBrush;
                blueRectangle.Fill = blueBrush;

                sp.Children.Add(newChildnode);
                sp.Children.Add(blueRectangle);
                newChild.Items.Add(sp);
            }
            tv_status3.Items.Add(newChild);
            ExpandInternal(tv_status3);

            newChild = new TreeViewItem();
            newChild.Header = areaList[3] + "网络";

            sp1 = new StackPanel();
            sp1.Orientation = Orientation.Horizontal;
            sp1.Height = 40;

            newChildnode1 = new TreeViewItem();
            newChildnode1.Header = "工控端：";
            newChildnode1.VerticalAlignment = VerticalAlignment.Center;
            blueRectangle1 = new Ellipse();
            blueRectangle1.Height = 15;
            blueRectangle1.Width = 15;

            blueBrush1 = new SolidColorBrush();
            blueBrush1.Color = Colors.Blue;
            blueRectangle1.StrokeThickness = 1;
            blueRectangle1.Stroke = blueBrush1;
            blueRectangle1.Fill = blueBrush1;

            sp1.Children.Add(newChildnode1);
            sp1.Children.Add(blueRectangle1);
            newChild.Items.Add(sp1);

            nodelist = new List<string>();
            nodelist = StaticTools.GetAllNodesByArea(areaList[3]);
            foreach (string s1 in nodelist)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                sp.Height = 40;

                TreeViewItem newChildnode = new TreeViewItem();
                newChildnode.Header = "编号：" + s1 + " 状态：";
                newChildnode.VerticalAlignment = VerticalAlignment.Center;
                Ellipse blueRectangle = new Ellipse();
                blueRectangle.Height = 15;
                blueRectangle.Width = 15;
                SolidColorBrush blueBrush = new SolidColorBrush();
                blueBrush.Color = Colors.Blue;
                blueRectangle.StrokeThickness = 1;
                blueRectangle.Stroke = blueBrush;
                blueRectangle.Fill = blueBrush;

                sp.Children.Add(newChildnode);
                sp.Children.Add(blueRectangle);
                newChild.Items.Add(sp);
            }
            tv_status4.Items.Add(newChild);
            ExpandInternal(tv_status4);
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
            for (int i = 0; i < sensorCount; i++)
            {
                List<History> ls = new List<History>();
                List<History> ls1 = new List<History>();
                Item item = new Item();
                item.name = i.ToString();
                item.area = StaticTools.GetAreaByNode(i.ToString());
                item.status = Status.OnLine;
                item.id = i;
                item.RetryCount = 0;
                item.historys = new List<History>();
                items.Add(i.ToString(), item);
                cloudData.Add(i.ToString(), ls1);
            }
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
                            moisture = _mytestdata[i].moisture,
                            NH = _mytestdata[i].NH,
                            temperature = _mytestdata[i].temperature,
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
                    updateChat(_NodeChat);
                    updateHistory();
                    //updateTable(_historyID);
                    Thread.Sleep((int)this._freq_data * 1000);
                }
                catch (Exception ex)
                { }
            }
        }
        #endregion
        private void alert(string id)
        {
            if (_play == 1)
            {
                SoundPlayer sp = new SoundPlayer();
                string strFileName = AppDomain.CurrentDomain.BaseDirectory.ToString() + "2.wav";
                sp.SoundLocation = strFileName;
                sp.Play();
            }
            MessageBox.Show(id + "号设备故障，请查看！");
        }
        private void updateChat(string id)
        {
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
                    yAxie = top + heigh - StaticTools.GetYaxia(items[id].historys[items[id].historys.Count - _maxShow].temperature, 120, heigh);
                }
                else
                {
                    start = 0;
                    yAxie = top + heigh - StaticTools.GetYaxia(items[id].historys[0].temperature, 120, heigh);
                }
                int _uint = _maxShow / 20;
                for (int i = start + 1; i < items[id].historys.Count; i++)
                {
                    double y = top + heigh - StaticTools.GetYaxia(items[id].historys[i].temperature, 120, heigh);
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
                    yAxie = top + heigh + 240 - StaticTools.GetYaxia(items[id].historys[items[id].historys.Count - _maxShow].moisture, 120, heigh);
                }
                else
                {
                    start = 0;
                    yAxie = top + heigh + 240 - StaticTools.GetYaxia(items[id].historys[0].moisture, 120, heigh);
                }
                int _uint = _maxShow / 20;
                for (int i = start + 1; i < items[id].historys.Count; i++)
                {
                    double y = top + heigh + 240 - StaticTools.GetYaxia(items[id].historys[i].moisture, 120, heigh);
                    double x = xAxie + 2.2;
                    drawLine(cav_chat, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                    if ((i - start - 1) % _uint == 0)
                        drawText(cav_chat, items[id].historys[i].time.ToShortTimeString(), x - 15, heigh + 30 + 240, 12, 28);
                    xAxie = x;
                    yAxie = y;
                }
            }
            #endregion

            //#region mos NH
            //if (items[id].historys.Count > 0)
            //{
            //    double xAxie = 25;
            //    double yAxie = 0;

            //    int start = 0;
            //    if (items[id].historys.Count > _maxShow)
            //    {
            //        start = items[id].historys.Count - _maxShow;
            //        yAxie = top + heigh + 480 - StaticTools.GetYaxia(items[id].historys[items[id].historys.Count - _maxShow].moisture, 120, heigh);
            //    }
            //    else
            //    {
            //        start = 0;
            //        yAxie = top + heigh + 480 - StaticTools.GetYaxia(items[id].historys[0].moisture, 120, heigh);
            //    }
            //    int _uint = _maxShow / 20;
            //    for (int i = start + 1; i < items[id].historys.Count; i++)
            //    {
            //        double y = top + heigh + 480 - StaticTools.GetYaxia(items[id].historys[i].moisture, 120, heigh);
            //        double x = xAxie + 2.2;
            //        drawLine(cav_chat, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
            //        if ((i - start - 1) % _uint == 0)
            //            drawText(cav_chat, items[id].historys[i].time.ToShortTimeString(), x - 15, heigh + 30 + 480, 12, 40);
            //        xAxie = x;
            //        yAxie = y;
            //    }
            //}
            //#endregion
        }
        private List<History> GetAveHistory(string area)
        {
            List<History> lh = new List<History>();
            List<string> areaList = StaticTools.GetAllArea();
            var data = items.Where(a => a.Value.area == area).Select(p => p.Key);
            if (data.ToList().Count == 0) return lh;
            double datacount = items["0"].historys.Count;
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
                        maxTemp += items[key].historys[i].temperature;
                        maxNH += items[key].historys[i].NH;
                        maxMio += items[key].historys[i].moisture;
                        dt = items[key].historys[i].time;
                        count++;
                    }
                    h.moisture = maxMio / count;
                    h.NH = maxNH / count;
                    h.temperature = maxTemp / count;
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
                        maxTemp += items[key].historys[i].temperature;
                        maxNH += items[key].historys[i].NH;
                        maxMio += items[key].historys[i].moisture;
                        dt = items[key].historys[i].time;
                        count++;
                    }
                    h.moisture = maxMio / count;
                    h.NH = maxNH / count;
                    h.temperature = maxTemp / count;
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
                    yAxie = top + heigh - StaticTools.GetYaxia(hs[0].temperature, 120, heigh);

                    int _uint = 36;
                    for (int i = start + 1; i < hs.Count; i++)
                    {
                        double y = top + heigh - StaticTools.GetYaxia(hs[i].temperature, 120, heigh);
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
                    yAxie = top + 240 + heigh - StaticTools.GetYaxia(hs[0].moisture, 120, heigh);

                    int _uint = 36;
                    for (int i = start + 1; i < hs.Count; i++)
                    {
                        double y = top + 240 + heigh - StaticTools.GetYaxia(hs[i].moisture, 120, heigh);
                        double x = xAxie + 2.2;
                        drawLine(cav_history, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                        if (i % _uint == 0 || i == start + 1)
                            drawText(cav_history, hs[i].time.ToShortTimeString(), x - 15, top+ heigh + 5 + 240, 12, 28);
                        xAxie = x;
                        yAxie = y;
                    }
                }
                #endregion

                //#region mos NH
                //if (hs.Count > 0)
                //{
                //    double xAxie = 25;
                //    double yAxie = 0;

                //    int start = 0;
                //    yAxie = top + 480 + heigh - StaticTools.GetYaxia(hs[0].NH, 120, heigh);

                //    int _uint = 36;
                //    for (int i = start + 1; i < hs.Count; i++)
                //    {
                //        double y = top + 480 + heigh - StaticTools.GetYaxia(hs[i].NH, 120, heigh);
                //        double x = xAxie + 2.2;
                //        drawLine(cav_history, new Point(xAxie, yAxie), new Point(x, y), Brushes.Blue);
                //        if (i % _uint == 0 || i == start + 1)
                //            drawText(cav_history, hs[i].time.ToShortTimeString(), x - 15, top + heigh + 5 + 480, 12, 28);
                //        xAxie = x;
                //        yAxie = y;
                //    }
                //}
                //#endregion
                index++;
                return;
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
                //p = new Point(sp.X, sp.Y + 480);
                //drawRectang(cav, p, width, heigh, string.Format("{0}", "%"), string.Format("{0}", "氨气含量趋势图（%）"), brush);
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

        private void updateSenser()
        {
            getData();
            return;
            for (int i = 0; i < sensorCount; i++)
            {
                History it = new History();

                if (_device.updateSensorData(i.ToString(), ref it))//do device update
                {
                    if (items.ContainsKey(i.ToString()))
                    {
                        if (items[i.ToString()].historys.Count >= _maxHistorys)
                            items[i.ToString()].historys.RemoveAt(0);
                        History itforsave = CopyHistory(it);
                        History it1 = CopyHistory(it);

                        items[i.ToString()].status = Status.OnLine;
                        items[i.ToString()].historys.Add(it);

                        cloudData[i.ToString()].Add(it1);

                        if (checkData(it)==Status.Error)
                        {
                            DataGridShow error = new DataGridShow();
                            error.status = Status.Error;
                            error.area = items[i.ToString()].area;
                            error.id = items[i.ToString()].id;
                            error.moisture = it.moisture;
                            error.NH = it.NH;
                            error.status = Status.Error;
                            error.temperature = it.temperature;
                            error.time = it.time;

                            errorData.Dispatcher.Invoke(() => { errorData.Items.Add(error); });

                            if (!_errorList.ContainsKey(i.ToString()))
                            {
                                _errorList.Add(i.ToString(), true);
                            }
                        }

                        if (i == 1)
                        {
                            DataGridShow dgs = new DataGridShow()
                            {
                                status = checkData(it),
                                area = items[i.ToString()].area,
                                id = items[i.ToString()].id,
                                moisture = it.moisture,
                                NH = it.NH,
                                temperature = it.temperature,
                                time = it.time,
                                isShow = true
                            };
                            SrollBarDataGrid.Dispatcher.Invoke(() =>
                            {
                                SrollBarDataGrid.Items.Add(dgs);
                                //SrollBarDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                            });
                        }
                        else
                        {
                            DataGridShow dgs = new DataGridShow()
                            {
                                status = checkData(it),
                                area = items[i.ToString()].area,
                                id = items[i.ToString()].id,
                                moisture = it.moisture,
                                NH = it.NH,
                                temperature = it.temperature,
                                time = it.time,
                                isShow = false
                            };
                            if (_mytestdata.Count > 30 * 24 * 60)
                            {
                                _mytestdata.RemoveAt(0);
                            }
                            _mytestdata.Add(dgs);
                        }


                    }
                }
                else
                {
                    if (items.ContainsKey(i.ToString()))
                    {
                        if (items[i.ToString()].historys.Count >= _maxHistorys)
                            items[i.ToString()].historys.RemoveAt(0);
                        items[i.ToString()].historys.Add(it);
                        History it1 = new History();
                        it1.id = it.id;
                        it1.moisture = it.moisture;
                        it1.NH = it.NH;
                        it1.temperature = it.temperature;
                        it1.time = it.time;
                        items[i.ToString()].RetryCount++;
                        cloudData[i.ToString()].Add(it1);
                    }

                    DataGridShow error = new DataGridShow();
                    error.status = Status.Error;
                    error.area = items[i.ToString()].area;
                    error.id = items[i.ToString()].id;
                    error.moisture = it.moisture;
                    error.NH = it.NH;
                    error.status = Status.Error;
                    error.temperature = it.temperature;
                    error.time = it.time;

                    errorData.Dispatcher.Invoke(() => { errorData.Items.Add(error); });

                    if (!_errorList.ContainsKey(i.ToString()))
                    {
                        _errorList.Add(i.ToString(), true);
                    }
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
                int width = 300;
                int heigh = 100;

                int count = cav_show.Children.Count;
                for (int i = 0; i < count; i++)
                {
                    cav_show.Children.RemoveAt(0);
                }
                cav_show.Height = 600;
                foreach (string key in _checkShow.Keys)
                {
                    if (_checkShow[key])
                    {
                        foreach (string i in items.Keys)
                        {
                            if (StaticTools.checkNode(key, items[i].id.ToString()))
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

                                    tb.Text = string.Format("{0}号设备\n\r 温度：{1}·C    湿度：{2}",
                                        items[i].historys[index - 1].id.ToString(),
                                        items[i].historys[index - 1].temperature.ToString(),
                                        items[i].historys[index - 1].moisture.ToString(),
                                        items[i].historys[index - 1].NH.ToString());
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

            _device.sp_DataSender(cmd);
            Thread.Sleep(1500);
            byte[] recv = _device.sp_read();
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

                        Status status = checkData(new History() { id = i, moisture = moist, NH = NH, status = Status.Error, temperature = temp, time = DateTime.Now });
                        items[i.ToString()].historys.Add(new History() { id = i, moisture = moist, NH = NH, status = Status.Error, temperature = temp, time = DateTime.Now });

                        DataGridShow error = new DataGridShow();
                        error.area = items[i.ToString()].area;
                        error.id = items[i.ToString()].id;
                        error.moisture = moist;
                        error.NH = NH;
                        error.status = status;
                        error.temperature = temp;
                        error.time = DateTime.Now;
                        errorData.Dispatcher.Invoke(() => { errorData.Items.Add(error); });

                        DataGridShow dgs = new DataGridShow()
                        {
                            status = status,
                            area = items[i.ToString()].area,
                            id = items[i.ToString()].id,
                            moisture = moist,
                            NH = NH,
                            temperature = temp,
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
                    int index = 7;
                    int id = 1;
                    for (int i = 7; i < data.Length - 2; i = i + 16)
                    {
                        int _id = id - 1;
                        double temp = (data[i + 4] * 256 + data[i + 5]) / 10.0;
                        double moist = (data[i + 6] * 256 + data[i + 7]) / 10.0;
                        double NH = (data[i + 8] * 256 + data[i + 9]) / 10.0;
                        Console.WriteLine("发送中....温度：{0}， 湿度：{1}， 氨气浓度：{2}", temp, moist, NH);

                        Status status = checkData(new History() { id = id, moisture = moist, NH = NH, status = Status.Error, temperature = temp, time = DateTime.Now });
                        items[_id.ToString()].historys.Add(new History() { id = id, moisture = moist, NH = NH, status = Status.Error, temperature = temp, time = DateTime.Now });

                        if (status == Status.Warning||status == Status.Error)
                        {
                            errorCount++;
                            errorCount1++;
                            DataGridShow error = new DataGridShow();
                            error.area = items[_id.ToString()].area;
                            error.id = items[_id.ToString()].id;
                            error.moisture = moist;
                            error.NH = NH;
                            error.status = status;
                            error.temperature = temp;
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
                            moisture = moist,
                            NH = NH,
                            temperature = temp,
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
            catch (Exception ex)
            { }
        }



        private Status checkData(History item)
        {
            if (item.temperature > (_maxT + _DivT) || item.temperature < (_minT - _DivT)) return Status.Error;
            if (item.moisture > (_maxM + _DivM) || item.moisture < (_minM - _DivM)) return Status.Error;
            if (item.NH > (_maxN + _DivN) || item.NH < (_minN - _DivN)) return Status.Error;

            if (item.temperature > _maxT || item.temperature < _minT) return Status.Warning;
            if (item.moisture > _maxM || item.moisture < _minM) return Status.Warning;
            if (item.NH > _maxN || item.NH < _minN) return Status.Warning;
            
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
                double left = 25;
                double width = 1000;
                double heigh = 520;

                if (dir.Count > 0)
                {
                    int count = dir.Count > 24 ? 24 : dir.Count;
                    int index = dir.Count / count;
                    double xAxie = 25;
                    double yAxie = 0;

                    int start = 0;
                    if (type == "温度")
                        yAxie = top + heigh - StaticTools.GetYaxia(dir[0].temperature, 120, heigh);
                    else if (type == "湿度")
                        yAxie = top + heigh - StaticTools.GetYaxia(dir[0].moisture, 120, heigh);
                    else
                        yAxie = top + heigh - StaticTools.GetYaxia(dir[0].NH, 120, heigh);

                    for (int i = start + 1; i < count; i++)
                    {
                        double y = 0;
                        if (type == "温度")
                            y = top + heigh - StaticTools.GetYaxia(dir[i * index].temperature, 120, heigh);
                        else if (type == "湿度")
                            y = top + heigh - StaticTools.GetYaxia(dir[i * index].moisture, 120, heigh);
                        else
                            y = top + heigh - StaticTools.GetYaxia(dir[i * index].NH, 120, heigh);

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
            catch (Exception ex)
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
                StaticTools.SaveConfig("port", StaticTools.getPortvalue(cob_port.SelectedIndex).ToString());
                if (cb_play.IsChecked == true) _play = 1;
                else _play = 0;
                StaticTools.SaveConfig("play", _play.ToString());
                MessageBox.Show("修改成功！");
            }
            catch (Exception ex)
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
                updateChat(e.AddedItems[0].ToString());
                _NodeChat = e.AddedItems[0].ToString();
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
            string name = txt_eqName.Text;
            string area = cmb_eqArea.SelectedValue.ToString();
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
            StaticTools.AddNode(area, id, name);
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
            StaticTools.AddArea(txt_addarea.Text.ToString());
            int count = cmb_eqArea.Items.Count;
            for (int i = 0; i < count; i++)
            {
                cmb_eqArea.Items.RemoveAt(0);
            }
            List<string> areas = StaticTools.GetAllArea();
            foreach (string s in areas)
            {
                cmb_eqArea.Items.Add(s);
            }
            cmb_eqArea.SelectedIndex = 0;
            update_cmd();
            MessageBox.Show("添加成功！");
        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            //byte[] data = strToToHexByte(s);
            byte[] data = Cmds.testCmd;
            //_device.sp_DataSender(data);
            //Thread.Sleep(100);
            //byte[] recv = _device.sp_read();

            txt_recv.Text += hexByteToStr(data);
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
                    catch (Exception ex)
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
            List<int> data = new List<int>();
            List<double[]> data1 = new List<double[]>();
            List<int> data2 = new List<int>();
            int data3 = 0;
            double[] data4;
            data.Add(0101);
            data.Add(0102);
            int id = 0101;
            int id1 = 0102;
            bool result = false;

            int freq = 10;
            int sf = 10;
            int bw = 10;
            int cr = 10;
            int opt = 1;
            int rf = 10;

            int len = 10;
            int addr = 10;

            List<byte> ds = new List<byte>();
            ds.Add(0x00);
            ds.Add(0x01);


            result = _bill.AddBSList(data);
            result = _bill.Check();
            result = _bill.DeleteBSList(data);
            data1 = _bill.getAllNodeData();
            data1 = _bill.getAllNodeDebug();
            data2 = _bill.getBS();
            data3 = _bill.getBSLength();
            data2 = _bill.getBSList();
            data4 = _bill.getNodeData(id);
            data4 = _bill.getNodeDebug(id);
            result = _bill.UpdateNodeId(id, id1);
            result = _bill.setBSLength(len);
            result = _bill.setBS(freq, sf, bw, cr, opt, rf);
            result = _bill.setBSToNode(id, freq, sf, bw, cr, opt, rf);
            result = _bill.setBSToNode(id, id1, freq, sf, bw, cr, opt, rf);
            result = _bill.setNodeRom(id, addr, ds);
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
                    int freq = data[0];
                    int bw = data[1];
                    int sf = data[2];
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
            int freq = 10;
            int bw = 6;
            int sf = 1;
            int rc = 0;
            int check = 0;
            int enc = -1;
            try
            {
                freq = Convert.ToInt32(txt_fre.Text);
            }
            catch
            {
                MessageBox.Show("中心频点输入有误（10~10000）");
                return;
            }
            freq = freq > 10000 ? 10000 : freq;
            freq = freq < 10 ? 10 : freq;

            sf = Convert.ToInt32(txt_SF.SelectedValue.ToString());

            bw = getBW(txt_SF.SelectedValue.ToString());

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
                return;
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

                _device.sp_DataSender(cmd);
                Thread.Sleep(1500);
                byte[] recv = _device.sp_read();
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
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private void stopFan(int group, int member)
        {
            try
            {
                byte[] cmd = Cmds.cmd_CmdStopFanByID(group, member);

                _device.sp_DataSender(cmd);
                Thread.Sleep(1500);
                byte[] recv = _device.sp_read();
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
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            if (cmb_nodetable.SelectedValue == null) return;
            string id = cmb_nodetable.SelectedValue.ToString();
            if (id == "全部")
                _historyID = "";
            else
                _historyID = id;
            updateTable(_historyID);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            IsRun = false;
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            _device.setPortName(txt_port.Text);
            Console.WriteLine(_device.connect());
            _bill = new DeviceBill(_device);
            //start();
        }

        private byte[] Read(byte[] cmds)
        {
            _device.sp_DataSender(cmds);
            Thread.Sleep(1000);
            byte[] recv = _device.sp_read();

            return recv;
        }
    }
}
