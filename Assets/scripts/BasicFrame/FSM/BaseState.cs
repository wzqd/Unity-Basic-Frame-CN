using System;


#region 状态基类
//有限状态机中的状态基类

//其中主要的方法为三个虚方法，分别在进入，退出和持续状态时调用
//只需继承该类并覆写需要的方法即可
//********注意继承的类必须和对应状态机的枚举选项名相同，因为是用反射创建的********
#endregion
public abstract class BaseState
{
    public E_States StateName;
    private FSM _fsm;
    public virtual void Init(FSM fsm, E_States stateType)
    {
        this._fsm = fsm; //归属的状态机
        this.StateName = stateType; //初始的状态
    }

    /// <summary>
    /// 进入状态时干什么
    /// </summary>
    public virtual void Enter(){} 
    /// <summary>
    /// 在状态中每一帧干什么
    /// </summary>
    public virtual void Act(){} 
    /// <summary>
    /// 退出状态时干什么
    /// </summary>
    public virtual void Exit(){}
}
