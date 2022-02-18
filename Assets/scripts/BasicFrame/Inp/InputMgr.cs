using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 输入管理模块
//输入模块基本上是使用了事件中心模块
//利用监听方便对任意游戏对象进行键位操作

//主要有改建位和开启关闭全局控制两个方法
//如果要加操作 在字典里新加键值对 （以后会通过方法 从数据库中读取 还没实现）
#endregion
public class InputMgr : Singleton<InputMgr>
{
    public Dictionary<string, KeyCode> KeySet = new Dictionary<string, KeyCode>() // 所有行动配置
    {
        {"up", KeyCode.W},
        {"down",KeyCode.S},
        {"left", KeyCode.A},
        {"right", KeyCode.D},
        {"jump",KeyCode.K},
        {"dash", KeyCode.L},
        {"attack", KeyCode.J}
    };

    private bool isSwitchOn = false; //全局输入是否开启
    public InputMgr() //构造函数 无mono开启update
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }

    private void InputUpdate() //被开启的update函数
    {
        if (isSwitchOn != true) return;
        CheckKey(KeySet["up"]);
        CheckKey(KeySet["down"]);
        CheckKey(KeySet["left"]);
        CheckKey(KeySet["right"]);
        CheckKey(KeySet["jump"]);
        CheckKey(KeySet["dash"]);
        CheckKey(KeySet["attack"]);
        
    }
    
    /// <summary>
    /// 改建位
    /// </summary>
    /// <param name="act">要改的某个行动</param>
    /// <param name="newKey">改成的新键位</param>
    public void ChangeKey(string act, KeyCode newKey)
    {
         KeySet[act] = newKey;
    }
    
    private void CheckKey(KeyCode key) //检测键位是否按下或者抬起
    {
        if (Input.GetKeyDown(key)) //按下
        {
            EventMgr.Instance.EventTrigger("KeyIsPressed", key);
        }
        if (Input.GetKeyUp(key)) //抬起
        {
            EventMgr.Instance.EventTrigger("KeyIsReleased", key);
        }

        if (Input.GetKey(key)) //长按
        {
            EventMgr.Instance.EventTrigger("KeyIsHeld", key);
        }
    }

    /// <summary>
    /// 开启或关闭全局键位检测
    /// </summary>
    /// <param name="state">开或者关</param>
    public void SwitchAllButtons(bool state)
    {
        isSwitchOn = state;
    }
}



    