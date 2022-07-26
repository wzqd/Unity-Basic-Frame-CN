using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#region 场景管理器
//场景管理器封装了两个更好的场景加载方法

//分别是同步加载场景和异步加载场景，两者都可以在加载完成后调用一个函数
//异步加载场景方法中触发了一个ProgressBar的事件，传入了加载的进度值，有需要可以监听
#endregion
public class SceneMgr : Singleton<SceneMgr>
{
    
    /// <summary>
    /// 同步加载场景
    /// 可能会有卡顿，在加载完后调用afterLoad
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="afterLoad">加载后调用的函数</param>
    /// <param name="loadSceneMode">加载场景的模式，默认为普通加载</param>
    public void LoadScene(string sceneName, UnityAction afterLoad, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(sceneName,loadSceneMode);
        afterLoad();
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName">场景名</param>
    /// <param name="afterLoad">加载后调用的函数</param>
    /// <param name="loadSceneMode">加载场景的模式，默认为普通加载</param>
    public void LoadSceneAsync(string sceneName, UnityAction afterLoad, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        MonoMgr.Instance.StartCoroutine(LoadSceneAsyncCoroutine(sceneName, afterLoad,loadSceneMode));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName, UnityAction afterLoad, LoadSceneMode loadSceneMode) //协程函数
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName,loadSceneMode);

        while (!ao.isDone)
        {
            EventMgr.Instance.EventTrigger("ProgressBar", ao.progress); //每次加载触发事件，传入新进度值

            yield return ao.progress; //更新进度值
        }

        afterLoad(); //调用加载完之后的方法
    }
}
