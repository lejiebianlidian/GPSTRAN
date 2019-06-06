using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Common;

namespace GPSTran
{
    public partial class SplashForm : Form
    {
        private Thread InitThread;

        public SplashForm()
        {
            InitializeComponent();

            //only one process instance
            Process[] processes;
            Process curProcess = Process.GetCurrentProcess();
            processes = Process.GetProcessesByName(curProcess.ProcessName);
            foreach (var p in processes)
            {
                try
                {
                    if (p.Id != curProcess.Id)
                    {
                        p.Kill();
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
            //initialize log;
            

            Logger.Info("Program start!");

           
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            try
            {
                new XMLIni().InitGeneral();
            }
            catch (Exception ex) 
            {
                MessageBox.Show("error occur when read language config,program can not start");
                Logger.Fatal("error occur when read language config,program closed,error message:"+ex.ToString());
                Process.GetCurrentProcess().Kill();

            }

            this.Text = Resource.lHelper.Key("m5");
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackgroundImage = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory+"image\\close.jpg");
            
            //start init Thread
            try
            {
                InitThread = new Thread(new ThreadStart(Init));
                InitThread.Start();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(Resource.lHelper.Key("m6"));
                Logger.Info(string.Format("Program closed:{0}",ex.ToString()));
                Process.GetCurrentProcess().Kill();
            
            }
        }

        private void SplashForm_MouseHover(object sender, EventArgs e)
        {

        }

        delegate void ShowS(string s);
        private void ShowStatus(string s) 
        {

            this.Status.Text = Resource.lHelper.Key("n20")+":" + s;
        
        }
        delegate void CloseForm();
        private void CloseFormFunction() 
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
         
        }
        delegate void Failed();
        private void InitFailed() 
        {
            MessageBox.Show(Resource.lHelper.Key("m6"));
            Logger.Info("Program closed");
            Process.GetCurrentProcess().Kill();
        }

        private void Init() 
        {
            Thread.Sleep(1000);
            ShowS show = ShowStatus;
            Failed fail = InitFailed;
            CloseForm close = CloseFormFunction;
            XMLIni ini = new XMLIni();

            this.Invoke(show,Resource.lHelper.Key("m7"));
            if (ini.Init() <=0) 
            {
                this.Invoke(fail, null);
                return;
            
            }
            Thread.Sleep(1000);

            this.Invoke(show, Resource.lHelper.Key("m8"));
            Thread.Sleep(1000);
            this.Invoke(close);

        }

        private void SplashForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

    }
}
