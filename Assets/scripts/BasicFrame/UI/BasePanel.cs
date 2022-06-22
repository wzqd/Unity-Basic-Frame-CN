using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

#region UI面板基类
//作为所有UI面板的基类，让所有UGUI的面板继承此脚本来用于UI管理器的管理
//实现寻找子类所有控件的方法以及显示和隐藏的方法
#endregion
public class BasePanel : MonoBehaviour
{
    /// <summary>
    ///字典容器存储所有UI控件
    ///键：hierarchy中控件名
    ///值：其中的控件，用list存储是因为可能是组合控件
    /// </summary>
    private Dictionary<string, List<UIBehaviour>> UIComponentsDict = new Dictionary<string, List<UIBehaviour>>();
    
    /// <summary>
    /// 生命周期函数
    /// </summary>
    private void Awake()
    {
        //开始时向字典中加入
        FindChildrenComponent<Button>();
        FindChildrenComponent<Toggle>();
        FindChildrenComponent<Slider>();
        FindChildrenComponent<Image>();
        FindChildrenComponent<Text>();
        FindChildrenComponent<ScrollRect>();
    }

    /// <summary>
    /// 找到子对象的控件
    /// </summary>
    private void FindChildrenComponent<T>() where T: UIBehaviour
    {
        T[] UIComponents = GetComponentsInChildren<T>(); //得到子对象所有组件
        
        foreach (var componentInChildren in UIComponents)//遍历所有组件
        {
            string componentName = componentInChildren.gameObject.name; //子对象名
            if (UIComponentsDict.ContainsKey(componentName))//如果已经存在(是复合控件)
            {
                UIComponentsDict[componentName].Add(componentInChildren);//往它的list里加组件
            }
            else //如果第一次添加
            {
                UIComponentsDict.Add(componentName, new List<UIBehaviour>() {componentInChildren}); //新建一个list
            }
        }
    }
    
    /// <summary>
    /// 得到对应对象名字的上对应类型的组件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    protected T GetUIComponent<T>(string UIComponentName) where T: UIBehaviour
    {
        if (UIComponentsDict.ContainsKey(UIComponentName))
        {
            for (int i = 0; i < UIComponentsDict[UIComponentName].Count; i++)
            {
                if (UIComponentsDict[UIComponentName][i] is T)
                {
                    return UIComponentsDict[UIComponentName][i] as T;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 虚方法 显示面板
    /// </summary>
    public virtual void Show(){}
    /// <summary>
    /// 虚方法 隐藏面板
    /// </summary>
    public virtual void Hide(){}

}
