using System;
using System.IO;
using System.Windows.Forms;

namespace UpdateAndSetup
{
    class FIlesCopy
    {


        private string sPath;
        private string dPath;
        public FIlesCopy(string spath, string dpath)
        {
            this.sPath = spath;
            this.dPath = dpath;
        }
        public int beginCopyFiles()
        {
            return CopyFolder(sPath, dPath);
        }

        public int CopyFolder(string sPath, string dPath)
        {

            int flag = 0;
            try
            {
                // 创建目的文件夹 
                if (!Directory.Exists(dPath))
                {
                    Directory.CreateDirectory(dPath);
                }

                // 拷贝文件 
                DirectoryInfo sDir = new DirectoryInfo(sPath);
                FileInfo[] fileArray = sDir.GetFiles();
                foreach (FileInfo file in fileArray)
                {
                    file.CopyTo(dPath + "\\" + file.Name, true);
                }

                // 循环子文件夹
                DirectoryInfo dDir = new DirectoryInfo(dPath);
                DirectoryInfo[] subDirArray =  sDir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirArray)
                {
                    CopyFolder(subDir.FullName, dPath + "//" + subDir.Name);
                }
                flag = 1;
            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show(string.Format("FileCopy:{0}", ex.ToString()));
            }

            return flag;
        }




    }
}
