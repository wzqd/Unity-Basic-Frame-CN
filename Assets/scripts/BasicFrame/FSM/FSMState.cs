using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;


namespace BasicFrame.FSM
{
    public abstract class FSMState
    {
        public e_State State { get; set; }
        private Dictionary<e_Transition, e_State> transStateDict;
        private List<FSMTransition> transitionList;
        public FSMState()
        {
            transStateDict = new Dictionary<e_Transition, e_State>();
            transitionList = new List<FSMTransition>();
            Construct();
        }

        protected abstract void Construct();

        public void AddToTransState(e_State state, e_Transition transition)
        {
            transStateDict.Add(transition, state);
            
            Type newTransType = Type.GetType(transition + "Trans");
            FSMTransition newTrans = Activator.CreateInstance(newTransType) as FSMTransition;
            transitionList.Add(newTrans);
        }
        
        public virtual void Enter(){}
        public virtual void Act(){}
        public virtual void Exit(){}
    }
}