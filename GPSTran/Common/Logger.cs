using System;
using log4net;
using System.Reflection;

namespace Common
{
    public static class Logger
    {
        private static ILog logger;

        public static void Init() 
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            }
            catch
            {
                ///修改异常提示
                throw new Exception(string.Format("Logger初始化异常！"));
                //throw ex;
            }
        }
        public static void Error(string str)
        {
            logger.Error(str);
        }
        public static void Info(string str)
        {
            logger.Info(str);
        }
        public static void Warn(string str)
        {
            logger.Warn(str);
        }
        public static void Fatal(string str)
        {
            logger.Fatal(str);
        }
        public static void Debug(string str)
        {
            logger.Debug(str);
        }
    }
}
