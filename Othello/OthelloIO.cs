using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Othello
{
    public class OthelloIO
    {
        public static string CreateDefaultDirectory(string pathString)
        {
            if (!Directory.Exists(pathString))
            {
                try
                {
                    Directory.CreateDirectory(pathString);
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("CreateDefaultDirectory [Exception] : Exception Message = {0}", e.Message));
                }
            }

            return pathString;
        }
        public static string GetFileSavePath(string strSaveDirPath, string fileName)
        {
            return Path.Combine(strSaveDirPath, fileName);
        }
        public static object LoadFromBinaryFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryFormatter f = new BinaryFormatter();

            //Deserialize and read
            object obj = f.Deserialize(fs);
            fs.Close();

            return obj;
        }
        public static void SaveToBinaryFile(object obj, string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();

            // Serializer and write
            bf.Serialize(fs, obj);
            fs.Close();
        }
    }
}
