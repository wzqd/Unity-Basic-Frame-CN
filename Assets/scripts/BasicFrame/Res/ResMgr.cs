using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#region 资源加载模块
//用于加载资源到内存后 直接使用
//包含更好的同步和异步加载方法

//加载的若是gameObject可以直接实例化
//异步加载要传入方法对资源进行操作
#endregion
public class ResMgr : Singleton<ResMgr>
{
    /// <summary>
    /// 同步加载资源
    /// ----确保资源在Resources文件夹下----
    /// </summary>
    /// <param name="name">资源名</param>
    /// <typeparam name="T">泛型 要加载的资源类型</typeparam>
    /// <returns>将资源返回 若是gameObject直接生成</returns>
    public T Load<T>(string name) where T:Object
    {
        T res = Resources.Load<T>(name);
        
        if (res is GameObject) //如果对象是一个GameObject类型的 先实例化再返回出去
            return GameObject.Instantiate(res);
        else //如果是别的无法实例化的类型 直接返回
            return res;
    }
    
    /// <summary>
    /// 异步加载资源
    /// ----传回调函数时使用lambda表达式 (obj)=>{方法体} 更方便----
    /// </summary>
    /// <param name="name">资源名</param>
    /// <param name="AfterLoadCallback">回调函数 异步加载完成时 要对资源进行的操作 传进协程中</param>
    /// <typeparam name="T">资源的类型</typeparam>
    public void LoadAsync<T>(string name, UnityAction<T> AfterLoadCallback) where T:Object
    {
        //开启异步加载的协程 利用公共mono管理器开启monoBehavior中协程
        MonoMgr.Instance.StartCoroutine(LoadAsyncCoroutine(name, AfterLoadCallback));
    }

    /// <summary>
    /// 真正的协同程序函数
    /// 在异步加载方法中被开启
    /// </summary>
    /// <param name="name">资源名</param>
    /// <param name="AfterLoadCallback">回调函数 异步加载完成时 要对资源进行的操作 参数就是资源</param>
    /// <typeparam name="T">资源的类型</typeparam>
    /// <returns>yield return不是真返回 利用AfterLoadCallback回调函数进行返回值操作</returns>
    private IEnumerator LoadAsyncCoroutine<T>(string name, UnityAction<T> AfterLoadCallback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            AfterLoadCallback(GameObject.Instantiate(r.asset) as T); //把资源作为回调函数的参数
        else
            AfterLoadCallback(r.asset as T); //把资源作为回调函数的参数
    }
}
