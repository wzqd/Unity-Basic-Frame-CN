using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#region 音频管理器
//音频管理器用于管理背景音乐和音效
//原理是将AudioSource全都放在一个物体上，动态添加删除
//正因为如此，该管理器可能不适用于大型项目，比如音源会被距离影响的游戏

//主要方法有开始或停止背景音乐和音效
#endregion

public class AudioMgr : Singleton<AudioMgr>
{
    private AudioSource BGM = null;
    private float BGMVolume = 1f;

    private GameObject soundCarrier = null;
    private float audioVolume = 1f;
    private List<AudioSource> audioList = new List<AudioSource>();

    /// <summary>
    /// 构造函数 添加update监听
    /// </summary>
    public AudioMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    /// <summary>
    /// 每帧检测是否有放完的音频并且清除
    /// </summary>
    private void Update()
    {
        for (int i = audioList.Count-1; i >= 0; --i)
        {
            if (!audioList[i].isPlaying)
            {
                Object.Destroy(audioList[i]);
                audioList.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// 播放BGM
    /// </summary>
    /// <param name="name">BGM名</param>
    public void PlayBGM(string name)
    {
        if (BGM == null)
        {
            GameObject obj = new GameObject("BGMCarrier");
            GameObject.DontDestroyOnLoad(obj);
            BGM = obj.AddComponent<AudioSource>();
        }
        
        ResMgr.Instance.LoadAsync<AudioClip>("Music/BGM/" + name, (audioClip) =>
        {
            BGM.clip = audioClip;
            BGM.loop = true;
            BGM.volume = BGMVolume;
            BGM.Play();
        });
    }

    /// <summary>
    /// 改变BGM音量
    /// </summary>
    /// <param name="volume">音量</param>
    public void ChangeBGMVolume(float volume)
    {
        BGMVolume = volume;
        if (BGM == null) return;
        BGM.volume = BGMVolume;
    }
    
    /// <summary>
    /// 暂停BGM
    /// </summary>
    public void PauseBGM()
    {
        if (BGM == null) return;
        BGM.Pause();
        
    }
    
    /// <summary>
    /// 停止BGM
    /// </summary>
    public void StopBGM()
    {
        if (BGM == null) return;
        BGM.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="afterPlay">播放结束后回调</param>
    /// <return>返回AudioSource用于暂停</return>
    public AudioSource PlayAudio(string name, bool isLoop, UnityAction<AudioSource> afterPlay = null)
    {
        if (soundCarrier == null)
        {
            soundCarrier = new GameObject ( "SoundCarrier");
        }

        AudioSource source = null;
        ResMgr.Instance.LoadAsync<AudioClip>("Music/Audio/" + name, (audioClip) =>
        {
            source = soundCarrier.AddComponent<AudioSource>();
            source.clip = audioClip;
            source.loop = isLoop;
            source.volume = audioVolume;
            source.Play();
            audioList.Add(source);

            if(afterPlay != null)
                afterPlay(source);
            
        });
        return source;
    }

    /// <summary>
    /// 改变所有音效音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void ChangeAudioVolume(float volume)
    {
        audioVolume = volume;
        foreach (AudioSource source in audioList)
        {
            source.volume = volume;
        }
    }

    /// <summary>
    /// 停止播放指定音效
    /// </summary>
    /// <param name="source">音效</param>
    public void StopAudio(AudioSource source)
    {
        if (audioList.Contains(source))
        {
            audioList.Remove(source);
            source.Stop();
            Object.Destroy(source);
        }
        
    }

    /// <summary>
    /// 清空所有音效 过场景时使用
    /// </summary>
    public void ClearAudio()
    {
        audioList.Clear();
    }

}
