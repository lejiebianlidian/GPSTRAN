using System.Collections.Generic;

namespace Common
{
   public class DllModel
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private PluginType pType;

        public PluginType PType
        {
            get { return pType; }
            set { pType = value; }
        }
        private string mainDllPath;

        public string MainDllPath
        {
            get { return mainDllPath; }
            set { mainDllPath = value; }
        }
        private List<string> dependDllPath;

        public List<string> DependDllPath
        {
            get { return dependDllPath; }
            set { dependDllPath = value; }
        }


    }
}
