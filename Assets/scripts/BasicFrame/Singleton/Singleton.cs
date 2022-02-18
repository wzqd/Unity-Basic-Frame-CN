using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 单例模式基类
//用于减少单例模式代码的书写
//继承了该类，即可变为单例类 使用 类名.Instance.方法 调用其中方法
//----继承了该类之后不能使用MonoBehavior中方法----
//----该单例模式为懒汉单例，不考虑多线程情况----
#endregion

public class Singleton<T> where T:new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
        if (instance == null)
                instance = new T();
        return instance;
        }
    }
}

