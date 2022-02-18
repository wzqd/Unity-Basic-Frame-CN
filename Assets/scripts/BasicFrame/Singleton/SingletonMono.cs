using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 继承Mono的单例模式基类
//继承该基类，既可以变为单例类，又可以使用MonoBehavior中的方法
//----该单例模式为懒汉单例，不考虑多线程情况----
#endregion

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if( instance == null )
            {
                GameObject obj = new GameObject {name = typeof(T) + "_Singleton"};
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            } 
            return instance;
        }
    }
}
