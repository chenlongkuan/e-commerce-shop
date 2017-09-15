using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Msg.Utils
{
    /// <summary>
    /// 序列化类
    /// </summary>
    public class SerializationHelper
    {
        private SerializationHelper()
        {
        }

        private static Dictionary<int, XmlSerializer> serializer_dict = new Dictionary<int, XmlSerializer>();
        /// <summary>
        /// 获取xml序列化实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static XmlSerializer GetSerializer(Type t)
        {
            int type_hash = t.GetHashCode();

            if (!serializer_dict.ContainsKey(type_hash))
                serializer_dict.Add(type_hash, new XmlSerializer(t));

            return serializer_dict[type_hash];
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        public static object Load(Type type, string filename)
        {
            FileStream fs = null;
            try
            {
                // open the stream...
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(fs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="filename">文件路径</param>
        public static bool Save(object obj, string filename)
        {
            bool success = false;

            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fs, obj);
                success = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return success;

        }

        /// <summary>
        /// xml序列化成字符串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>xml字符串</returns>
        public static string Serialize(object obj)
        {
            string returnStr = "";

            XmlSerializer serializer = GetSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xtw = null;
            StreamReader sr = null;
            try
            {
                xtw = new System.Xml.XmlTextWriter(ms, Encoding.UTF8);
                xtw.Formatting = System.Xml.Formatting.Indented;
                serializer.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(ms);
                returnStr = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
                if (sr != null)
                    sr.Close();
                ms.Close();
            }
            return returnStr;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static object DeSerialize(Type type, string s)
        {

            byte[] b = System.Text.Encoding.UTF8.GetBytes(s);
            try
            {
                XmlSerializer serializer = GetSerializer(type);
                return serializer.Deserialize(new MemoryStream(b));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public static byte[] SerializeToDb<T>(IList<T> t)
        {
            if (t == null || t.Count() == 0)
            {
                return null;
            }
            //定义一个流
            Stream stream = new MemoryStream();
            //定义一个格式化器
            BinaryFormatter bf = new BinaryFormatter();
            foreach (var item in t)
            {
                bf.Serialize(stream, item);  //序列化
            }
            byte[] array = null;
            array = new byte[stream.Length];
            //将二进制流写入数组
            stream.Position = 0;
            stream.Read(array, 0, (int)stream.Length);
            //关闭流
            stream.Close();
            return array;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<T> DeSerializeToDb<T>(byte[] array)
        {
            if (array == null || array.Count() == 0)
            {
                return null;
            }
            List<T> arrayList = new List<T>();
            //定义一个流
            MemoryStream stream = new MemoryStream(array);
            //定义一个格式化器
            BinaryFormatter bf = new BinaryFormatter();
            while (stream.Position != stream.Length)
            {
                var obj = bf.UnsafeDeserialize(stream, null);
                arrayList.Add((T)obj);  //反序列化
            }
            stream.Close();
            return arrayList;
        }

        /// <summary>          
        /// JSON序列化  
        /// </summary>  
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return jsonString;
        }
        /// <summary>
        /// JSON反序列化  
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }
    }
}
