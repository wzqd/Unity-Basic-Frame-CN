using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

#region 公共Mono模块
//让其他不继承MonoBehavior的类可以使用Mono中的方法，比如协程

// ----当使用生命周期函数时， 用addListener监听其方法----
#endregion

public class MonoMgr : SingletonMono<MonoMgr>
{
    private event UnityAction updateEvent;

    void Update()
    {
        if (updateEvent != null) //如果监听的委托不为空
            updateEvent(); //在自己的Update中调用别人的“update”
    }
    
    /// <summary>
    /// 添加update监听
    /// </summary>
    /// <param name="func">无mono的“update”函数</param>
    public void AddUpdateListener(UnityAction func) 
    {
        updateEvent += func;
    }
    
    /// <summary>
    /// 删除Update监听
    /// </summary>
    /// <param name="func">无mono的“update”函数</param>
    public void RemoveUpdateListener(UnityAction func)
    {
        updateEvent -= func;
    }
}    
