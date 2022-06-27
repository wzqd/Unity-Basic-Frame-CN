using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 状态机基类
//有限状态机的状态机基类

//其中主要的方法是修改状态，需要的话可以设置初始状态
//Update函数可以覆写，在其中加入何时修改状态的逻辑
//使用时需要准备一个枚举，********枚举选项名必须和状态名相同，因为要用反射创建状态对象********
//********一定要设置一个初始枚举值********

//优化思路可以用整型代替枚举，避免装拆箱子    可以加入对象池系统
#endregion
public abstract class BaseFSM: MonoBehaviour
{
    private List<BaseState> stateList = new List<BaseState>();//后续可以用对象池替换（小体量状态不是非常需要）
    public Enum CurrentStateType { get; set; } //枚举类型,一定要设置成设置一个初始状态
    private BaseState currentState;


    /// <summary>
    /// 虚方法 生命周期函数 使用时记得覆写以及使用base方法
    /// </summary>
    protected virtual void Update()
    {
        if (currentState != null)
        {
            currentState.Act(); //每一帧执行状态内的方法
        }
    }

    /// <summary>
    ///修改状态
    /// </summary>
    /// <param name="newState">要进入的新状态</param>
    /// <param name="allowReenter">是否允许重复进入同一状态</param>
    public void ChangeState(Enum newState, bool allowReenter = false)
    {
        if (CurrentStateType == null) return; //保证有初始状态

        if (!allowReenter && newState.Equals(CurrentStateType)) return; //如果重复进入同一个状态，或不允许重复进入，直接返回
        if(currentState != null) 
            currentState.Exit(); //执行退出当前状态的方法

        currentState = GetStateObj(newState); //得到新的状态
        CurrentStateType = currentState.StateType;
        currentState.Enter(); //执行新状态的进入方法
        

    }

    /// <summary>
    ///从列表中得状态
    /// </summary>
    /// <param name="stateType">枚举类型</param>
    /// <returns>返回状态，供切换状态使用</returns>
    private BaseState GetStateObj(Enum stateType) //此处有装拆箱问题，以后解决
    {
        foreach (var stateObj in stateList) //如果状态已经存在列表内，直接返回
        {
            if (stateObj.StateType.Equals(stateType)) return stateObj;
        }

        //*********枚举中的类型名要和状态类名一直，否则会创建空物体*********
        BaseState state = Activator.CreateInstance(Type.GetType(stateType.ToString())!) as BaseState; //若不存在，创建一个
        state.Init(this,stateType); //将其初始化为传入的枚举类型
        stateList.Add(state); //加入列表
        return state;
    }
}
