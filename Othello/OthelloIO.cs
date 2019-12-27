using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

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

        public static string GetBase64String(object obj)
        {
            using (MemoryStream m = new MemoryStream())
            {
                new BinaryFormatter().Serialize(m, obj);
                return Convert.ToBase64String(m.ToArray());
            }
        }

        public static object GetObjectFromBase64String(string b64str)
        {
            byte[] bytes = Convert.FromBase64String(b64str);
            using (MemoryStream m = new MemoryStream(bytes, 0, bytes.Length))
            {
                m.Write(bytes, 0, bytes.Length);
                m.Position = 0;
                return new BinaryFormatter().Deserialize(m);
            }
        }

        public static string ConvertBase64StringToJSON(string b64str)
        {
            return JsonConvert.SerializeObject(b64str);
        }

        public static string ConvertJSONToBase64String(string json)
        {
            return JsonConvert.DeserializeObject<string>(json);
        }
    }
}
