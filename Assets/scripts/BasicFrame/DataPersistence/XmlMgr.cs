using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

/// <summary>
/// XML数据存储管理器
/// 主要有存储和读取两个方法
/// </summary>
public class XmlMgr : Singleton<XmlMgr>
{
    /// <summary>
    ///数据存储
    /// </summary>
    /// <param name="data">要存储的数据</param>
    /// <param name="fileName">要存入的文件名</param>
    public void SaveData(object data, string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".xml"; //文件路径
        using (StreamWriter writer = new StreamWriter(path)) //开写入文件流
        {
            XmlSerializer s = new XmlSerializer(data.GetType()); //序列化
            s.Serialize(writer, data);
        }
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="type">要读取的对象type</param>
    /// <param name="fileName">要读取的文件名</param>
    /// <returns></returns>
    public object LoadData(Type type, string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".xml"; //默认路径
        if (!File.Exists(path)) //若默认存储路径不存在
        {
            path = Application.streamingAssetsPath + "/" + fileName + ".xml"; //读取默认数据路径（手动配置的初始数据）
            if (!File.Exists(path)) //若也没有初始数据
            {
                return Activator.CreateInstance(type); //根据type 返回空类型
            }
        }

        using(StreamReader reader = new StreamReader(path)) //开读取文件流
        {
            XmlSerializer s = new XmlSerializer(type); //反序列化
            return s.Deserialize(reader);
        }
    }
    
    
}
