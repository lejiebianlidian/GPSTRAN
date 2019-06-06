using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common;
using System.Threading;

namespace GPSTran
{
    public partial class ExitForm : Form
    {
        public ExitForm()
        {
            InitializeComponent();
        }

        private void ExitForm_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;
            this.Text = Resource.lHelper.Key("m10");

            Thread exitThread = new Thread(new ThreadStart(ProgramExit));
            exitThread.Start();

        }

        private void ExitForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dlgResult = MessageBox.Show(Resource.lHelper.Key("m9"), Resource.lHelper.Key("m1"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (dlgResult != DialogResult.Yes)
            {
                e.Cancel = true;
            }
            else
            {
                Logger.Info("Program closed");
                Process.GetCurrentProcess().Kill();
            }

        }

        private delegate void BarValue(int value);
        private void ProgramExit() 
        {
            BarValue bv = ValueChange;
            if (Form1.udpReceiveThread != null)
            {
                Form1.udpReceiveThread.Safe_Stop();

            }
            this.Invoke(bv, 10);
            Thread.Sleep(200);
            if (Form1.groupInfoThread != null)
            {
                Form1.groupInfoThread.Safe_Stop();
            }

            if (Form1.userInfoThread != null)
            {
                Form1.userInfoThread.Safe_Stop();
            }
            this.Invoke(bv, 20);
            Thread.Sleep(200);
            if (Form1.tableMaintenanceThread != null)
            {
                Form1.tableMaintenanceThread.Safe_Stop();
            }
            this.Invoke(bv, 30);
            Thread.Sleep(200);
            Thread.Sleep(200);
            if (Form1.entityAndIssiThread != null)
            {
                Form1.entityAndIssiThread.Safe_Stop();
            }
            this.Invoke(bv, 50);
            Thread.Sleep(200);
            if (Form1.ipTransferThread != null)
            {
                Form1.ipTransferThread.Safe_Stop();

            }
            this.Invoke(bv, 60);
            Thread.Sleep(200);
            if (Form1.commonTableThread != null)
            {
                Form1.commonTableThread.Safe_Stop();
            }
            this.Invoke(bv, 70);
            Thread.Sleep(400);
            if (Form1.PluginExecuteThread != null)
            {
                Form1.PluginExecuteThread.Safe_Stop();
            }
            this.Invoke(bv, 90);
            WaitThreadStop();

            this.Invoke(bv, 100);
        
        }
        private void ValueChange(int i) 
        {
            this.progressBar1.Value = i;
            this.Refresh();
            if (i == 100) 
            {
                
                Logger.Info("Program closed");
                Process.GetCurrentProcess().Kill();
            
            }
        
        }

        private void WaitThreadStop() 
        {
            foreach (PluginManager pManager in Resource.PluginInstanceList.Values) 
            {
                pManager.SetEnabled(false);
                while (!pManager.IsStop()) 
                {
                    Thread.Sleep(100);
                }

            }
            foreach (DBManager dbManager in Resource.DBInstanceList.Values) 
            {
                dbManager.SetEnabled(false);
                while (!dbManager.IsStop()) 
                {
                    Thread.Sleep(100);
                
                }
            
            }
            foreach (IPManager ipManager in Resource.IPInstanceList.Values)
            {
                ipManager.SetEnabled(false);
                while (!ipManager.IsStop())
                {
                    Thread.Sleep(100);

                }

            }


        }

    }
}
