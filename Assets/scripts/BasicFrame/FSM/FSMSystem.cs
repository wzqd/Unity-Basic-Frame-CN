using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSMSystem: MonoBehaviour
{
    private List<FSMState> stateList;
    public abstract Enum CurrentState { get; set; }
    private FSMState currentStateObj;

    private List<FSMState> stateObjList = new List<FSMState>(); //后续可以用对象池替换

    protected virtual void Update()
    {
        if (currentStateObj != null)
        {
            currentStateObj.Act();
        }
    }

    /// <summary>
    ///修改状态
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(Enum newState)
    {
        if (newState == CurrentState) return;
        if(currentStateObj != null) currentStateObj.Exit();

        currentStateObj = GetStateObj(newState);
        currentStateObj.Enter();
        

    }

    /// <summary>
    ///从列表中得状态 用于变状态
    /// </summary>
    /// <param name="stateType"></param>
    /// <returns></returns>
    private FSMState GetStateObj(Enum stateType)
    {
        foreach (var stateobj in stateList)
        {
            if (stateobj.StateType == stateType) return stateobj;
        }

        FSMState state = Activator.CreateInstance(Type.GetType(stateType.ToString())) as FSMState;
        state.Init(this,stateType);
        stateList.Add(state);
        return state;
    }
}
