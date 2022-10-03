using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region 拖拽脚本实现的Mono单例基类
//继承该脚本的类可使用MonoBehaviour所有方法
//但是需要拖拽到场景物体上
//该脚本依附的对象过场景会消失，只是作为单例存在于一个场景中(不消失会导致物体重复存在)
//需要注意不要拖到两个物体上，否则只会依附于最后一个物体

#endregion
public class SingletonMonoDrag<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;
    public static T Instance => instance;

    protected void Awake()
    {
        instance = this as T;
    }
}

