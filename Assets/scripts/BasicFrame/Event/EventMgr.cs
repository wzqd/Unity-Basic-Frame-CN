using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// 空接口 用于包裹事件类
/// 多次一举的原因是为了泛型事件的创建
/// </summary>
public interface IEventInfo
{

}

/// <summary>
/// 事件类 （泛型）
/// </summary>
/// <typeparam name="T">泛型 让事件可以传递一个参数</typeparam>
public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo( UnityAction<T> action) //构造 添加一个要记录的泛型参数方法
    {
        actions += action;
    }
}

/// <summary>
/// 事件类 （无参）
/// </summary>
public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action) //构造 添加一个要记录的方法
    {
        actions += action;
    }
}


#region 事件中心模块
//事件中心是基于观察者模式的管理类
// 包含添加事件监听，移除事件监听，触发事件，过场景清空事件

//其中派发事件可以传一个任意类型参数info备用 若有多个要传可传数组
//也有无参重载，不用传info参数
//监听事件也有监听有参和监听无参两种重载
//********事件名规范，帕斯卡命名法，首字母全大写********
#endregion
public class EventMgr : Singleton<EventMgr>
{
    //key —— 事件的名字（比如：怪物死亡，玩家死亡，通关 等等）
    //IEventInfo —— 事件（的父类接口）也就是要记录和监听方法的集合
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听 （泛型参数 ）
    /// </summary>
    /// <param name="name">对应事件名</param>
    /// <param name="action">事件中要添加的方法 有一个泛型参无返回值方法</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        //有没有对应的事件监听
        //若有
        if( eventDic.ContainsKey(name) )
        {
            (eventDic[name] as EventInfo<T>).actions += action; //向已有事件中记录方法
        }
        //若无
        else
        {
            eventDic.Add(name, new EventInfo<T>( action )); //向字典添加新泛型事件
        }
    }

    /// <summary>
    /// 添加事件监听 （无参重载）
    /// </summary>
    /// <param name="name">对应事件名</param>
    /// <param name="action">事件中要添加的方法 无参无返回值方法</param>
    public void AddEventListener(string name, UnityAction action)
    {
        //有没有对应的事件监听
        //若有
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action; //向已有事件中记录方法
        }
        //若无
        else
        {
            eventDic.Add(name, new EventInfo(action)); //向字典添加新事件
        }
    }
    
    /// <summary>
    /// 移除事件监听 （泛型参数）
    /// </summary>
    /// <param name="name">对应事件名</param>
    /// <param name="action">事件中要移除的方法 有一个参无返回值</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo<T>).actions -= action;
    }

    /// <summary>
    /// 移除事件监听 （无参重载）
    /// </summary>
    /// <param name="name">对应事件名</param>
    /// <param name="action">事件中要移除的方法 无参无返回值</param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo).actions -= action;
    }

    /// <summary>
    /// 事件派发和触发（泛型参数）
    /// </summary>
    /// <param name="name">自定义事件名</param>
    /// <param name="info">想要传过去的一个泛型参数 有多个可传数组</param>
    public void EventTrigger<T>(string name, T info)
    {
        //有没有对应的事件监听 （若无，说明无人监听该事件，也就不用触发）
        //若有
        if (eventDic.ContainsKey(name))
        {
            if((eventDic[name] as EventInfo<T>).actions != null)
                (eventDic[name] as EventInfo<T>).actions(info); //调用事件委托
        }
    }

    /// <summary>
    /// 事件派发和触发（无参重载）
    /// </summary>
    /// <param name="name">自定义事件名</param>
    public void EventTrigger(string name)
    {
        //有没有对应的事件监听（若无，说明无人监听该事件，也就不用触发）
        //若有
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo).actions != null)
                (eventDic[name] as EventInfo).actions(); //调用事件委托
        }
    }

    /// <summary>
    /// 清空事件中心 主要用在 场景切换时
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
