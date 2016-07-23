using AgriManagement.tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AgriManagement
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public delegate void PassResultHandler(object sender, string result);
        public event PassResultHandler pevent;
        CloudAdapter _cloud = new CloudAdapter();
        string _sign = "";
        string _typy = "";
        string _id = "";
        public AddUser(string sign,string type,string id)
        {
            InitializeComponent();
            _sign = sign;
            _typy = type;
            _id = id;
        }

        private void btn_adduser_Click(object sender, RoutedEventArgs e)
        {
            if (txt_psd.Password != txt_compsd.Password || txt_psd.Password == "")
            {
                MessageBox.Show("确认密码无效！");
                return;
            }
            if (txt_id.Text == "")
            {
                MessageBox.Show("账号ID不能为空！");
                return;
            }
            if (txt_nickname.Text == "")
            {
                MessageBox.Show("昵称不能为空！");
                return;
            }

            try
            {
                if (_typy == "add")
                {
                    _cloud.PostInsertUser(txt_id.Text, txt_psd.Password, txt_nickname.Text);
                }
                else
                {
                    //_cloud.UpdateUser(_sign, txt_id.Text, txt_username.Text, txt_psd.Password, txt_nickname.Text, DateTime.Now.ToShortTimeString());
                }

                MessageBox.Show("操作成功！");
                this.Close();
                pevent(this, "");
            }
            catch
            {
                MessageBox.Show("错误！");
            }
        }
    }
}
