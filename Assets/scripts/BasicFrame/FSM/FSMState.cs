using System;


public abstract class FSMState
{
    public Enum StateType;
    private FSMSystem FSM;
    public virtual void Init(FSMSystem fsm, Enum stateType)
    {
        this.FSM = fsm;
        this.StateType = stateType;
    }


    public virtual void Enter(){}
    public virtual void Act(){}
    public virtual void Exit(){}
}
