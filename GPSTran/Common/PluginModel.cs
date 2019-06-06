using System.Collections.Generic;
using System.Net;

namespace Common
{
  public  class PluginModel
    {
        private string name;//instance name

    
        private DllModel dllModel;//plugin dlls

        public DllModel DllModel
        {
            get { return dllModel; }
            set { dllModel = value; }
        }
      // instance enabled
      private bool enabled;

      //GPS protocol
      private string protocol;

      //destination ip
      private string ip;

      //destination prot
      private int port;


      //UDP or TCP
      private string netProtocol;

      //longitude offset 
      private double lonOffset;


      //latitude offset
      private double latOffset;


      //if issi belongs to entityID  then,GPS data will transfer to the IP
      private List<int> entityID;

      //can access to the remote IP ?
      private bool ipAvailable;

      private int citycode;

      public int Citycode
      {
          get { return citycode; }
          set { citycode = value; }
      }

      public bool IpAvailable
      {
          get { return ipAvailable; }
          set { ipAvailable = value; }
      }

      public string Name
      {
          get { return name; }
          set { name = value; }
      }
      public bool Enabled
      {
          get { return enabled; }
          set { enabled = value; }
      }

      public string Protocol
      {
          get { return protocol; }
          set { protocol = value; }
      }
      public string Ip
      {
          get { return ip; }
          set { ip = value; }
      }
      public int Port
      {
          get { return port; }
          set { port = value; }
      }

      public string NetProtocol
      {
          get { return netProtocol; }
          set { netProtocol = value; }
      }
      public double LonOffset
      {
          get { return lonOffset; }
          set { lonOffset = value; }
      }

      public double LatOffset
      {
          get { return latOffset; }
          set { latOffset = value; }
      }

      public List<int> EntityID
      {
          get { return entityID; }
          set { entityID = value; }
      }

      public IPEndPoint EndPoint
      {
          get { return new IPEndPoint(IPAddress.Parse(ip), port); }
      }


      //table name
      private string tableName;

      public string TableName
      {
          get { return tableName; }
          set { tableName = value; }
      }
      private int maxCount;

      public int MaxCount
      {
          get { return maxCount; }
          set { maxCount = value; }
      }

      private string desIP;

      public string DesIP
      {
          get { return desIP; }
          set { desIP = value; }
      }
      private string desCatalog;

      public string DesCatalog
      {
          get { return desCatalog; }
          set { desCatalog = value; }
      }
      private int desPort;

      public int DesPort
      {
          get { return desPort; }
          set { desPort = value; }
      }
      private string desUser;

      public string DesUser
      {
          get { return desUser; }
          set { desUser = value; }
      }
      private string desPwd;

      public string DesPwd
      {
          get { return desPwd; }
          set { desPwd = value; }
      }
      private string desInstance;

      public string DesInstance
      {
          get { return desInstance; }
          set { desInstance = value; }
      }


    }
}
