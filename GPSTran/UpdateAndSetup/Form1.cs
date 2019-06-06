using System;
using System.Windows.Forms;

namespace UpdateAndSetup
{
    public partial class Form1 : Form
    {
        private bool firstSetupTag = false;
        private bool updateSetupTag = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            firstSetupTag = true;
            updateSetupTag = false;
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            updateSetupTag = true;
            firstSetupTag = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {

            string runPath="";
            if (firstSetupTag)
            {
                runPath =  System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            }
            if (runPath!="")
            {

                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory + @"SetupFile\setup.exe");
              System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();

              
            }
            if(updateSetupTag)
            {
                Form form4 = new Form4();
                form4.Show();
            }

        }
        private void getPath(string path)
        { 

        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
              
        }
        
       
    }
}
