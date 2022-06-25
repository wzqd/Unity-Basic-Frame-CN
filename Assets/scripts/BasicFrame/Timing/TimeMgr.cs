using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region 倒计时管理器
//倒计时管理器可以更方便地对一个方法进行计时

//其中方法为开始对一个方法的计时 并在计时完成时进行操作
//另一个为直接计时 计时完成时进行操作
//************不要在update函数里开协程************
#endregion
public class TimeMgr : Singleton<TimeMgr>
{
    /// <summary>
    /// 对一个函数进行倒计时
    /// </summary>
    /// <param name="TimeToWait">倒计时时间</param>
    /// <param name="TimeFunc">要计时的方法</param>
    /// <param name="AfterTime">计时结束进行的逻辑</param>
    public Coroutine StartFuncTimer(float TimeToWait, UnityAction TimeFunc, UnityAction AfterTime)
    {
        Coroutine timerCoroutine = MonoMgr.Instance.StartCoroutine(FuncTimerCoroutine(TimeToWait, TimeFunc, AfterTime));
        return timerCoroutine; //返回协程 可用于终止协程
    }

    /// <summary>
    /// 直接进行倒计时
    /// </summary>
    /// <param name="TimeToWait">倒计时时间</param>
    /// <param name="AfterTime">计时结束进行的逻辑</param>
    public Coroutine StartTimer(float TimeToWait, UnityAction AfterTime)
    {
        Coroutine timerCoroutine = MonoMgr.Instance.StartCoroutine(TimerCoroutine(TimeToWait, AfterTime));
        return timerCoroutine; //返回协程 可用于终止协程
    }

    /// <summary>
    /// 停止倒计时
    /// 目的时在一个脚本里停止协程 防止报错
    /// </summary>
    /// <param name="coroutineToStop">要停止的协程</param>
    public void StopTimer(Coroutine coroutineToStop)
    {
        MonoMgr.Instance.StopCoroutine(coroutineToStop);
    }
    
    
    private IEnumerator TimerCoroutine(float TimeToWait, UnityAction AfterTime) //无函数协程计时器
    {
        yield return new WaitForSeconds(TimeToWait);
        AfterTime();
    }
    
    private IEnumerator FuncTimerCoroutine(float TimeToWait, UnityAction TimeFunc, UnityAction AfterTime) //函数协程计时器
    {
        TimeFunc();
        yield return new WaitForSeconds(TimeToWait);
        AfterTime();
    }
}
