using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Common;

namespace GPSTran
{
    static class Program
    {
       private static System.Threading.Mutex mutex;
       public static Form Splash;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main()
        {
                Logger.Init();
                Logger.Info(@"GPSTran启动！");
                Logger.Warn(@"GPSTran启动！");
                Logger.Error(@"GPSTran启动！");
                Logger.Fatal(@"GPSTran启动！");
                Logger.Debug(@"GPSTran启动！");

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                mutex = new System.Threading.Mutex(true, "OnlyRun");


                if (mutex.WaitOne(0, false))
                {

                    Splash = new SplashForm();
                    Application.Run(Splash);
                }
                else
                {
                    MessageBox.Show("程序已经在运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            
        }


    }
}
