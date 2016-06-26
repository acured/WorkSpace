using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AgriManagement.tools
{
    class CloudAdapter
    {
        private string host = Config.host;
        private string updateData = Config.i_updateDate;
        private string updateUser = Config.i_updateUser;
        private string insertUser = Config.i_insertUser;
        private string deleteUser = Config.i_deleteUser;
        private string login = Config.i_login;

        public void Login(string uid, string psd)
        {
            string msg = "{ \"username\" : \"" + uid.ToString() + "\",\"password\" : \"" + psd.ToString() + "\"}";
            string url = host + login;
            dowork(url, msg);
        }

        public void InsertUser(string sign, string userid,string username,string password,string nickname,string time)
        {
            string msg = "{ \"sign\" : \"" 
                + sign.ToString() + "\",\"userid\" : \"" 
                + userid.ToString() + "\",\"username\" : \""
                + username.ToString() + "\",\"password\" : \""
                + password.ToString() + "\",\"nickname\" : \""
                + nickname.ToString() + "\",\"time\" : \""
                + time.ToString() + "\"}";

            string url = host + updateUser;
            dowork(url, msg);
        }

        public void DeleteUser(string sign, string userid)
        {
            string msg = "{ \"sign\" : \""
                + sign.ToString() + "\",\"userid\" : \""
                + userid.ToString() + "\"}";

            string url = host + deleteUser;
            dowork(url, msg);
        }

        public void UpdateUser(string sign, string userid, string username, string password, string nickname, string time)
        {
            string msg = "{ \"sign\" : \""
                + sign.ToString() + "\",\"userid\" : \""
                + userid.ToString() + "\",\"username\" : \""
                + username.ToString() + "\",\"password\" : \""
                + password.ToString() + "\",\"nickname\" : \""
                + nickname.ToString() + "\",\"time\" : \""
                + time.ToString() + "\"}";

            string url = host + insertUser;
            dowork(url, msg);
        }

        public List<Users> GetAllUsers()
        {
            List<Users> users = new List<Users>();

            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "123", name = "test1", psd = "123", nickname = "tt1" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });
            users.Add(new Users() { id = "456", name = "test2", psd = "123", nickname = "tt2" });

            return users;
        }

        public void UpdateData(string sign, Dictionary<string, List<History>> items)
        {
            string msg = "{ \"sign\" : \"" + sign.ToString() + "\",\"data\":[";

            foreach (string key in items.Keys)
            {
                msg += "{\"name\" : \"" + key + "\",\"items\":[";
                foreach (History h in items[key])
                {
                    if (h.temperature == 0 && h.moisture == 0 && h.NH == 0) continue;
                    msg += "{\"temp\" : \"" 
                        + h.temperature + "\",\"humidity\" : \"" 
                        + h.moisture + "\",\"ch3\" : \""
                        + h.NH + "\",\"ts\" : \"" 
                        + h.time.ToShortTimeString() + "\"},";
                }
                msg.Remove(msg.Length - 2);
                msg += "]},";
            }
            msg.Remove(msg.Length - 2);
            msg += "]}";


            string url = host + insertUser;
            dowork(url, msg);
        }

        public void UpdateData1(string sign, Dictionary<string, List<History>> items)
        {
            string msg = "{ \"sign\" : \"" + sign.ToString() + "\",\"businessid\" : \"112\",\"data\":[";

            foreach (string key in items.Keys)
            {
                msg += "{\"region\" : \"" + key + "\",\"sensor\" : \"" + key + "\",\"items\":[";
                foreach (History h in items[key])
                {
                    if (h.temperature == 0 && h.moisture == 0 && h.NH == 0) continue;
                    msg += "{\"temp\" : \""
                        + h.temperature + "\",\"humidity\" : \""
                        + h.moisture + "\",\"ch3\" : \""
                        + h.NH + "\",\"ts\" : \""
                        + h.time.ToShortTimeString() + "\"},";
                }
                msg.Remove(msg.Length - 2);
                msg += "]},";
            }
            msg.Remove(msg.Length - 2);
            msg += "]}";


            string url = host + updateData;
            dowork(url, msg);
        }


        private void dowork(string url,string Content)
        {
            String chatUrl = "http://115.29.137.121:8080/api/datacube/upload";
            String chatContent = Content;

            var request = WebRequest.Create(chatUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "multipart/form-data";

            byte[] byteArray = Encoding.UTF8.GetBytes(chatContent);
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(byteArray, 0, byteArray.Length);
            }

            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException e)
            {
                response = e.Response as HttpWebResponse;
            }

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        public async void PostMethodMulti(string id, double temp, double mosit, double NH)//temp[0]返回网页temp[1]返回cookies
        {
            String chatUrl = "http://115.29.137.121/api/datacube/upload";

            TimeSpan ts1 = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string ts = Convert.ToInt64(ts1.TotalSeconds).ToString();


            string data = "[{\"region\":1,\"sensor\":" + id + ",\"items\":[{\"temp\":" + temp + ",\"humidity\":" + mosit + ",\"ch3\":" + NH + ",\"ts\":" + ts + "}]}]";
            try
            {
                using (var client = new HttpClient())
                {
                    using (var content =
                        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        byte[] data1 = Encoding.UTF8.GetBytes("kevin");
                        content.Add(new StreamContent(new MemoryStream(data1)), "sign");
                        byte[] data2 = Encoding.UTF8.GetBytes("0");
                        content.Add(new StreamContent(new MemoryStream(data2)), "businessid");
                        byte[] data3 = Encoding.UTF8.GetBytes(data);
                        content.Add(new StreamContent(new MemoryStream(data3)), "data");

                        using (
                           var message =
                               await client.PostAsync(chatUrl, content))
                        {
                            var input = await message.Content.ReadAsStringAsync();

                            var asd = !string.IsNullOrWhiteSpace(input) ? Regex.Match(input, @"http://\w*\.directupload\.net/images/\d*/\w*\.[a-z]{3}").Value : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void PostMethodUpdateParam(double Maxt, double Mint, double Maxm, double Minm,double Maxn,double Minn)//temp[0]返回网页temp[1]返回cookies
        {
            String chatUrl = "http://115.29.137.121/api/alarm/set";

            string data = "{\"temp\":{\"upper\":" + Maxt + ",\"lower\":" + Mint + "},\"humidity\":{\"upper\":"
                +Maxm+",\"lower\":"+Minm+"},\"ch3\":{\"upper\":"+Maxn+",\"lower\":"+Minn+"}}";
            //string json = JsonConvert.SerializeObject(data);
            try
            {
                using (var client = new HttpClient())
                {
                    using (var content =
                        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        byte[] data1 = Encoding.UTF8.GetBytes("kevin");
                        content.Add(new StreamContent(new MemoryStream(data1)), "sign");
                        byte[] data2 = Encoding.UTF8.GetBytes("0");
                        content.Add(new StreamContent(new MemoryStream(data2)), "businessid");
                        byte[] data3 = Encoding.UTF8.GetBytes(data);
                        content.Add(new StreamContent(new MemoryStream(data3)), "data");

                        using (
                           var message =
                               await client.PostAsync(chatUrl, content))
                        {
                            var input = await message.Content.ReadAsStringAsync();
                            object o = JsonConvert.DeserializeObject(input);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
