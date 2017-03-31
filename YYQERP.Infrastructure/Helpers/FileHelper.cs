using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YYQERP.Infrastructure.Helpers
{
    public class FileHelper
    {
        public static void WriteStringToFile(string strText, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);

                sw.Write(strText);
                sw.Flush();
               // sw.Close();

            }
        }


        public static bool CheckFileLength(float fileSize, out string errMsg)
        {
            errMsg = "";
            int allowFileSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["UploadFileSize"]);
            if (fileSize > allowFileSize)
            {
                errMsg = "上传的文件大小超过最大限制" + allowFileSize.ToString() + "M";
                return false;
            }

            return true;
        }


        public static void DeleteFile(string filepath)
        {
            File.Delete(filepath);
        }
    }
}
