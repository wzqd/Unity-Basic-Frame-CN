using System;
using System.Collections;
using System.Collections.Generic;
using BasicFrame.FSM;
using UnityEngine;

public class FSMSystem: MonoBehaviour
{
    private List<FSMState> stateList;
    private FSMState currentState;

    public void Update()
    {
        currentState.Act();
    }

    protected virtual void PerformTransition(e_Transition trans){}

}
