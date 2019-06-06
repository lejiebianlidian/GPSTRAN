using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Common
{
   public class StatusDetect
    {
        //ping IP
        public bool IpDetect(IPEndPoint endPoint)
        {
            try
            {
                bool networkFlag = false;
                Ping p = new Ping();
                PingOptions pOption = new PingOptions();
                pOption.DontFragment = true;
                string data = "Test Data!";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 100;
                PingReply reply = p.Send(endPoint.Address, timeout, buffer, pOption);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    networkFlag = true;
                }
                return networkFlag;

            }
            catch
            {
                Logger.Error("error occur when ping: "+endPoint.Address.ToString());
                return false;

            }


        }





    }
}
