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


public class UIMgr : Singleton<UIMgr>
{
    /// <summary>
    /// 面板字典
    /// 键：面板名
    /// 值：面板类型脚本
    /// </summary>
    private Dictionary<string, BasePanel> panelDict = new Dictionary<string, BasePanel>();

    private Transform canvas; //场景中唯一的canvas
    private Transform bot; //底层面板
    private Transform mid; //中层面板
    private Transform top; //上层面板
    
    public UIMgr()
    {
        GameObject obj = ResMgr.Instance.Load<GameObject>("UI/Canvas"); //动态载入面板预设体
        canvas = obj.transform;
        GameObject.DontDestroyOnLoad(obj); //使其过场景不移除

        //找到三个层级
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");

        obj = ResMgr.Instance.Load<GameObject>("UI/EventSystem"); //动态载入EventSystem
        GameObject.DontDestroyOnLoad(obj); //使其过场景不移除
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
            return; //返回退出函数
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
}
