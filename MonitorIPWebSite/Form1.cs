using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Edward;
using System.Net;
using System.IO;

namespace MonitorIPWebSite
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        #region 参数


        public static bool IsRun = false;
        public static string AppFolder = Application.StartupPath + @"\Monitor";
        public static string LogFolder = AppFolder + @"\Log";

        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Monitor IP & WebSite,Ver:" + Application.ProductVersion;
            CreateFolder();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {

            if (!IsRun)
            {
                btnGo.Text = "Stop";
                txtIP1.ReadOnly = true;
                txtWeb1.ReadOnly = true;
                txtIP2.ReadOnly = true;
                txtWeb2.ReadOnly = true;
            }
            else
            {
                btnGo.Text = "Start";
                txtIP1.ReadOnly = false;
                txtWeb1.ReadOnly = false;
                txtIP2.ReadOnly = false;
                txtWeb2.ReadOnly = false;
            }

            IsRun = !IsRun;

            while (IsRun)
            {
                string result = "";

                if (Other.pingIp(txtIP1.Text.Trim()))
                {
                    UpdateInfo(lstInfo, "Ping " + txtIP1.Text + " 成功.");
                    WriteLog("Ping " + txtIP1.Text + " 成功.");
                }
                else
                {
                    UpdateInfo(lstInfo, "Ping " + txtIP1.Text + " 失败.");
                    WriteLog("Ping " + txtIP1.Text + " 失败.");
                }

                if (Other.pingIp(txtIP2.Text.Trim()))
                {
                    UpdateInfo(lstInfo, "Ping " + txtIP2.Text + " 成功.");
                    WriteLog("Ping " + txtIP2.Text + " 成功.");
                }
                else
                {
                    UpdateInfo(lstInfo, "Ping " + txtIP2.Text + " 失败.");
                    WriteLog ("Ping " + txtIP2.Text + " 失败.");
                }


                if (CheckUrlVisit(txtWeb1.Text.Trim(), out result))
                {
                    UpdateInfo(lstInfo, txtWeb1.Text + "访问成功.");
                    WriteLog(txtWeb1.Text + "访问成功.");
                }
                else
                {
                    UpdateInfo(lstInfo, txtWeb1.Text + "访问失败." + result);
                    WriteLog(txtWeb1.Text + "访问失败." + result);
                }

                result = "";
                if (CheckUrlVisit(txtWeb2.Text.Trim(), out result))
                {
                    UpdateInfo(lstInfo, txtWeb2.Text + "访问成功.");
                    WriteLog(txtWeb2.Text + "访问成功.");
                }
                else
                {
                    UpdateInfo(lstInfo, txtWeb2.Text + "访问失败." + result);
                    WriteLog(txtWeb2.Text + "访问失败." + result);
                }


                Delay(5000);

            }
       
        }



        





        private void UpdateInfo(ListBox listbox, string message)
        {
            if (listbox.Items.Count > 1000)
                listbox.Items.RemoveAt(0);

            string item = string.Empty;
            //listbox.Items.Add("");
            item = DateTime.Now.ToString("HH:mm:ss") + " " + @message;
            if (listbox.InvokeRequired)
            {
                listbox.BeginInvoke(new Action<string>((msg) =>
                {
                    listbox.Items.Add(msg);
                }), item);

            }
            else
            {
                listbox.Items.Add(item);
            }
            if (listbox.Items.Count > 1)
            {
                listbox.TopIndex = listbox.Items.Count - 1;
                listbox.SetSelected(listbox.Items.Count - 1, true);
            }
        
        }

        /// <summary>
        /// 延時子程序
        /// </summary>
        /// <param name="interval">延時的時間，单位毫秒</param>
        private void Delay(double interval)
        {
            DateTime time = DateTime.Now;
            double span = interval * 10000;
            while (DateTime.Now.Ticks - time.Ticks < span)
            {
                Application.DoEvents();
            }

        }



        public bool CheckUrlVisit(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    resp.Close();
                    return true;
                }
            }
            catch (WebException webex)
            {
                return false;
            }

            return false;

        }


        public bool CheckUrlVisit(string url,out string result)
        {
            result = "OK";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    resp.Close();
                    return true;
                }
            }
            catch (WebException webex)
            {
                result = webex.Message;
                return false;
            }

            return false;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsRun)
                e.Cancel = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public static void WriteLog(string log)
        {
            string logfile = LogFolder + @"\" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                StreamWriter sw = new StreamWriter(logfile, true);
                log = DateTime.Now.ToString("HH:mm:ss") + "->" + log;
                sw.WriteLine(log);
                sw.Close();
            }
            catch (Exception)
            {

                //throw;
            }


        }

                /// <summary>
        /// 
        /// </summary>
        public static void CreateFolder()
        {
            if (!Directory.Exists(AppFolder))
                Directory.CreateDirectory(AppFolder);
            if (!Directory.Exists(LogFolder))
                Directory.CreateDirectory(LogFolder);
        }

    }
}
