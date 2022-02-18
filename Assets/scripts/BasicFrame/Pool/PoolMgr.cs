using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 抽屉数据  池子中的一列容器
/// </summary>
public class PoolData
{
    //Hierarchy中抽屉父节点 
    public GameObject fatherObj;
    //抽屉
    public List<GameObject> poolList;

    /// <summary>
    /// 构造函数 创建抽屉
    /// </summary>
    /// <param name="obj">抽屉的第一个对象</param>
    /// <param name="poolObj">最大的父节点</param>
    public PoolData(GameObject obj, GameObject poolObj)
    {
        //为新抽屉创建一个父对象 然后抽屉作为最大节点Pool的子物体
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform; 
        poolList = new List<GameObject> (){}; //创建抽屉
        PushToPool(obj); // 失活对象 存入抽屉
    }

    /// <summary>
    /// 放入抽屉
    /// </summary>
    /// <param name="obj">放入的对象</param>
    public void PushToPool(GameObject obj)
    {
        //失活 让其隐藏
        obj.SetActive(false);
        //存起来
        poolList.Add(obj);
        //设置父对象
        obj.transform.parent = fatherObj.transform;
    }

    /// <summary>
    /// 从抽屉取出
    /// </summary>
    /// <returns>取出的对象</returns>
    public GameObject GetFromPool()
    {
        GameObject obj = poolList[0]; 
        poolList.RemoveAt(0);//取出第一个
        
        obj.SetActive(true); //激活 让其显示
        obj.transform.parent = null; //断开了父子关系

        return obj;
    }
}


#region 缓存池模块
//缓存池模块用于加载多个可能会重复使用，重复销毁的对象
//原理是把使用过的对象失活，而不销毁
//把再次使用的对象激活，而不重新创建

//包含取对象和放对象两个方法
//有一个大的缓存池容器字典
//字典中包含PoolData类 包裹了一个列表和hierarchy中父节点（便于管理） 
#endregion
public class PoolMgr : Singleton<PoolMgr>
{                                                                                                                                                                                                     
    //缓存池容器 
    //string —— 一类对象的名字（一个抽屉）
    //PoolData —— 包裹了一个列表和hierarchy中父节点（便于管理） 
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj; //所有PoolData hierarchy中的父节点

    /// <summary>
    /// 取出对象 （异步）
    /// </summary>
    /// <param name="name">要取出的对象名</param>
    /// <param name="AfterGetCallback">异步加载出来后要执行的方法 用lambda</param>
    /// <returns></returns>
    public void GetObjAsync(string name, UnityAction<GameObject> AfterGetCallback)
    {
        //有抽屉 并且抽屉里有东西
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            //调用PoolData的取出方法 激活对象
            AfterGetCallback(poolDic[name].GetFromPool()); //把拿出来的资源作为回调函数的参数
        }
        else //无抽屉 或 抽屉里对象都被取出去
        {
            //通过异步加载资源 创建对象
            ResMgr.Instance.LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name; //让该对象名字改成和抽屉名一样 （因为创建会自带clone后缀 不方便管理）
                AfterGetCallback(o); //把加载出来的资源作为回调函数的参数
            });
        }
    }

    /// <summary>
    /// 取出对象 （同步）
    /// </summary>
    /// <param name="name">对象名</param>
    /// <returns>返回取出的对象</returns>
    public GameObject GetObj(string name)
    {
        GameObject obj;
        //有抽屉 并且抽屉里有东西
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0) 
        {
            // 调用PoolData 激活对象
            obj = poolDic[name].GetFromPool();
        }
        else //无抽屉 或 抽屉里对象都被取出去
        {
            // 同步加载 实例化对象
            obj = ResMgr.Instance.Load<GameObject>(name);
            obj.name = name;
        }
        return obj;
    }
        
        
        
    /// <summary>
    /// 放回对象
    /// 将暂时不用的对象放回去 外部可以写一个延迟函数放回去
    /// </summary>
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null) // 如果最大的父节点还未创建
            poolObj = new GameObject("Pool");

        //里面有抽屉
        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushToPool(obj); //调用PoolData 失活对象
        }
        else //里面没有抽屉
        {
            poolDic.Add(name, new PoolData(obj, poolObj)); // 创建PoolData 
        }
    }


    /// <summary>
    /// 清空缓存池的方法 
    /// 主要用在 场景切换时
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
