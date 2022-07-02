using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region 状态机基类
//有限状态机的的状态机

//其中主要的方法是修改状态，需要的话可以设置初始状态
//Update函数可以覆写，在其中加入何时修改状态的逻辑
//使用时需要准备一个枚举，********枚举选项名必须和状态名相同，因为要用反射创建状态对象********
#endregion
public abstract class FSM: MonoBehaviour
{
    private Dictionary<int,BaseState> stateDict = new Dictionary<int, BaseState>();
    public E_States CurrentStateType { get; set; } = E_States.Default;//枚举类型
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
    public void ChangeState(E_States newState, bool allowReenter = false)
    {

        if (!allowReenter && newState == CurrentStateType) return; //如果重复进入同一个状态，或不允许重复进入，直接返回
        if(currentState != null) 
            currentState.Exit(); //执行退出当前状态的方法

        currentState = GetStateObj(newState); //得到新的状态
        CurrentStateType = currentState.StateName;
        currentState.Enter(); //执行新状态的进入方法
        

    }

    /// <summary>
    ///从列表中得状态
    /// </summary>
    /// <param name="stateType">枚举类型</param>
    /// <returns>返回状态，供切换状态使用</returns>
    private BaseState GetStateObj(E_States stateType) //此处有装拆箱问题，以后解决
    {
        int key = (int) stateType;
        if (stateDict.ContainsKey(key)) 
            return stateDict[key]; //如果状态已经存在字典内，直接返回

        //*********枚举中的类型名要和状态类名一直，否则会创建空物体*********
        BaseState state = Activator.CreateInstance(Type.GetType(stateType.ToString())!) as BaseState; //若不存在，创建一个
        state.Init(this,stateType); //将其初始化为传入的枚举类型
        stateDict.Add((int)stateType, state); //加入列表
        return state;
    }

    /// <summary>
    /// 清空状态字典
    /// </summary>
    public void ClearAllStates()
    {
        stateDict.Clear();
    }
}
