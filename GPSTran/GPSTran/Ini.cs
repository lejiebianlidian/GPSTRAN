using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace GPSTran
{
    class Ini
    {
         // 声明INI文件的写操作函数 WritePrivateProfileString()

        [System.Runtime.InteropServices.DllImport("kernel32")]

        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        // 声明INI文件的读操作函数 GetPrivateProfileString()

        [System.Runtime.InteropServices.DllImport("kernel32")]

        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int SectionExists(string section, string filePath);

        private string sPath = null;
        public Ini(string path)
        {

            if (!System.IO.File.Exists(path))
            {
                throw new Exception("can't find path " + path);
            }

            this.sPath = path;
        }

        public void Write(string section, string key, string value)
        {

            // section=配置节，key=键名，value=键值，path=路径

            WritePrivateProfileString(section, key, value, sPath);

        }
        public string ReadValue(string section, string key)
        {

            // 每次从ini中读取多少字节

            Byte[] buffer = new Byte[65535];
            string s;
            // section=配置节，key=键名，temp=上面，path=路径
            try
            {
                int bufLength = GetPrivateProfileString(section, key, "", buffer, 255, sPath);
                s = Encoding.GetEncoding(0).GetString(buffer);
                s = s.Substring(0, bufLength);

                return s;

            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("ini.cs error:{0}", ex.ToString()));
                return "";
            }

        }


        public void ReadSections(StringCollection SectionList)
        {


            //Note:必须得用Bytes来实现，StringBuilder只能取到第一个Section

            byte[] Buffer = new byte[65535];


            int bufLen = 0;


            bufLen = GetPrivateProfileString(null, null, null, Buffer, Buffer.GetUpperBound(0), sPath);

            GetStringsFromBuffer(Buffer, bufLen, SectionList);



        }


        private void GetStringsFromBuffer(Byte[] Buffer, int bufLen, StringCollection Strings)
        {

            Strings.Clear();


            if (bufLen != 0)
            {

                int start = 0;

                for (int i = 0; i < bufLen; i++)
                {

                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {

                        String s = Encoding.GetEncoding(0).GetString(Buffer, start, i - start);

                        Strings.Add(s);

                        start = i + 1;

                    }


                }


            }

        }
    }
}
