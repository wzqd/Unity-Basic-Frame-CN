using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 面板层级枚举
/// </summary>
public enum E_PanelLayer
{
    Bot,
    Mid,
    Top
}

#region UI管理器
//UI管理器用于管理继承了BasePanel的所有面板
//自动加载预设好的Canvas和EventSystem，其中画布Canvas分成了上中下三层

//方法主要有显示面板和隐藏面板，显示面板时要记得设置上中下的层级，还可以添加加载后的函数
//次要的方法有得到正在显示的面板，得到层级panel的对象，以及添加自定义监听
#endregion
public class UIMgr : Singleton<UIMgr>
{
    /// <summary>
    /// 面板字典
    /// 键：面板名
    /// 值：面板类型脚本
    /// </summary>
    private Dictionary<string, BasePanel> panelDict = new Dictionary<string, BasePanel>();

    /// <summary>
    ///公共 场景中唯一的canvas
    /// </summary>
    public RectTransform canvas;
    
    private Transform bot; //底层面板
    private Transform mid; //中层面板
    private Transform top; //上层面板
    
    /// <summary>
    /// 构造函数
    /// 载入Canvas和EventSystem
    /// </summary>
    public UIMgr()
    {
        GameObject obj = ResMgr.Instance.Load<GameObject>("UI/Canvas"); //动态载入面板预设体
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj); //使其过场景不移除

        //找到三个层级
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");

        obj = ResMgr.Instance.Load<GameObject>("UI/EventSystem"); //动态载入EventSystem
        GameObject.DontDestroyOnLoad(obj); //使其过场景不移除
    }

    /// <summary>
    /// 得到某一层级
    /// </summary>
    /// <param name="panelLayer">层级面板名</param>
    /// <returns></returns>
    public Transform GetPanelLayer(E_PanelLayer panelLayer)
    {
        switch (panelLayer)
        {
            case E_PanelLayer.Bot:
                return bot;
            case E_PanelLayer.Mid:
                return mid;
            case E_PanelLayer.Top:
                return top;
        }
        return null;
    }
    
    
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="panelName">面板名</param>
    /// <param name="panelLayer">面板层级 Bot Mid Top</param>
    /// <param name="afterShown">面板显示之后调用的函数</param>
    /// <typeparam name="T">Panel类</typeparam>
    public void ShowPanel<T>(string panelName, E_PanelLayer panelLayer, UnityAction<T> afterShown = null) where T: BasePanel
    {
        if (panelDict.ContainsKey(panelName)) //如果字典中已经存在脚本（防止被加载两次）
        {
            panelDict[panelName].Show(); //执行面板类中的显示方法
            
            if (afterShown != null)
                afterShown(panelDict[panelName] as T); //如果传入了回调函数 则执行显示面板后的逻辑
            return; //返回退出函数 避免重复逻辑
        }
        
        
        //异步加载面板
        ResMgr.Instance.LoadAsync<GameObject>("UI/" + panelName, (panel) => //参数为面板obj
        {
            //加载完成后函数
            
            Transform currPanel = bot; //默认为底层
            switch (panelLayer)
            {
                case E_PanelLayer.Mid: 
                    currPanel = mid; //设置为中层
                    break;
                case E_PanelLayer.Top:
                    currPanel = top; //设置为上层
                    break;
            }

            panel.transform.SetParent(currPanel); //将面板设置为某一层的子对象
            
            //归位
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;
            
            (panel.transform as RectTransform).offsetMax= Vector2.zero;
            (panel.transform as RectTransform).offsetMin= Vector2.zero;
            
            T panelComponent = panel.GetComponent<T>(); //得到面板上的脚本

            panelComponent.Show(); //执行面板类中的显示方法
            
            if (afterShown != null)
                afterShown(panelComponent); //如果传入了回调函数 则执行显示面板后的逻辑
            panelDict.Add(panelName, panelComponent); //向字典内加入面板脚本
        });
    }
    
    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName">面板名</param>
    public void HidePanel(string panelName)
    {
        if (panelDict.ContainsKey(panelName))
        {
            panelDict[panelName].Hide(); //执行面板类中的隐藏方法(存储数据等)
            GameObject.Destroy(panelDict[panelName].gameObject); //删除场景上的面板
            panelDict.Remove(panelName); //将其从字典中移除
        }
    }

    /// <summary>
    ///得到正在显示的面板
    /// </summary>
    /// <param name="panelName">面板名</param>
    /// <typeparam name="T">面板类型</typeparam>
    public T GetPanel<T>(string panelName) where T: BasePanel
    {
        if (panelDict.ContainsKey(panelName))
            return panelDict[panelName] as T;
        return null;
    }

    
    /// <summary>
    /// 给UI组件添加自定义事件监听
    /// </summary>
    /// <param name="component">组件，用GetControl得</param>
    /// <param name="triggerType">事件类型</param>
    /// <param name="eventFunc">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour component, EventTriggerType triggerType, UnityAction<BaseEventData> eventFunc)
    {
        EventTrigger trigger = component.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = component.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = triggerType;
        entry.callback.AddListener(eventFunc);
    }
    
}
