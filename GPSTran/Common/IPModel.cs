using System.Collections.Generic;
using System.Net;

namespace Common
{
   public class IPModel
    {
        // instance name
        private string name;

        // instance enabled
        private bool enabled;

        //GPS protocol
        //private string protocol;

        //destination ip
        private string ip;

        //destination prot
        private int port;


        //UDP or TCP
        //private string netProtocol;

        //longitude offset 
        private double lonOffset;

       
        //latitude offset
        private double latOffset;

       
        //if issi belongs to entityID  then,GPS data will transfer to the IP
        private List<int> entityID;

        //can access to the remote IP ?
        private bool ipAvailable;

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

        //public string Protocol
        //{
        //    get { return protocol; }
        //    set { protocol = value; }
        //}
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

        //public string NetProtocol
        //{
        //    get { return netProtocol; }
        //    set { netProtocol = value; }
        //}
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
            get { return new IPEndPoint(IPAddress.Parse(ip),port); }
        }



    }
}
