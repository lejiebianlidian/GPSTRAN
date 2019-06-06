namespace Common
{
   public interface IPlugin
    {
        void SetPluginModel(PluginModel pModel);//set PluginModel which contains all config items
        PluginModel GetPluginModel();//get PluginModel which contains all config items
        int GetStatus();//get status of working plugin
        long GetStatistics();//get total of GPS data which plugin has received
        PluginType GetPluginType();//get plugin type,three types:RemoteDB,RemoteIP,TranDB
        void SetEnabled(bool enabled);// control wether to deal with GPS data
        void SetDebugEnabled(bool enabled);//control wether to detect exception of method execute
        void Execute(byte[] data);//method to deal with GPS data
        bool IsStop();//wether this plugin instance is stopped
    }
}
