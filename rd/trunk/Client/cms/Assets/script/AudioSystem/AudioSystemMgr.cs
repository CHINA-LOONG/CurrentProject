using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundEffectInfo
{
    public SoundEffectInfo(string soundName, float triggerTime)
    {
        this.soundName = soundName;
        this.triggerTime = triggerTime;
    }

    public string soundName;
    public float triggerTime;
}

public class AudioSystemMgr : MonoBehaviour {

    private static AudioSystemMgr mInst;
    private static byte dontCreate = 0;

    void OnApplicationQuit()
    {
        dontCreate = 1;
    }

    public static AudioSystemMgr Instance
    {
        get 
        {
            if (mInst==null && AudioSystemMgr.dontCreate == 0)
            {
                GameObject go = new GameObject("AudioSystemMgr"); 
                go.AddComponent<AudioListener>();
                mInst = go.AddComponent<AudioSystemMgr>();
                //mInst.Init();
                //TODO: remove
                DontDestroyOnLoad(go);
            }
            return AudioSystemMgr.mInst; 
        }
    }

    //sound effect list used to pick effect has same name
    private List<SoundEffectInfo> mSoundEffectList = new List<SoundEffectInfo>();

    ////背景----副本使用音轨音源数量
    //private static readonly int audioMusicLength = 2;
    ////UI--事件--技能等音效音轨音源数量
    //private static readonly int audioSoundLength = 10;

    //private AudioObject[] audioMusics;
    //private AudioObject[] audioSounds;

    //private float musicVolume = 1.0f;
    //private float soundVolume = 1.0f;

    //public float MusicVolume
    //{
    //    get { return musicVolume; }
    //    set { musicVolume = Mathf.Clamp01(value); setAudioVolume(audioMusics, soundVolume); }
    //}
    //public float SoundVolume
    //{
    //    get { return soundVolume; }
    //    set { soundVolume = Mathf.Clamp01(value); setAudioVolume(audioSounds, soundVolume); }
    //}

    //private Transform mTrans;

    //public void Init()
    //{
    //    mTrans = transform;
    //    audioMusics = new AudioObject[audioMusicLength];
    //    audioSounds = new AudioObject[audioSoundLength];
    //    for (int i = 0; i < audioMusicLength; i++)
    //    {
    //        audioMusics[i] = addAudio("audio_music", true);
    //    }
    //    for (int i = 0; i < audioSoundLength; i++)
    //    {
    //        audioSounds[i] = addAudio("audio_sound", false);
    //    }
    //}

    //AudioObject addAudio(string objName,bool loop)
    //{
    //    GameObject obj = new GameObject(objName);
    //    obj.transform.parent = mTrans;
    //    AudioSource source = obj.AddComponent<AudioSource>();
    //    source.playOnAwake = false;
    //    source.loop = loop;

    //    AudioObject audioObj = new AudioObject()
    //    {
    //        source=source,
    //        isUser=false,
    //        useTimeScale=false
    //    };
    //    return audioObj;
    //}

    //AudioObject getUnUseAudio()
    //{
    //    for (int i = 0; i <audioSoundLength; i++)
    //    {
    //        if (!audioSounds[i].isUser)
    //        {
    //            return audioSounds[i];
    //        }
    //    }
    //    return audioSounds[0];
    //}

    //AudioObject getUnBGMusic()
    //{
    //    return audioMusics[0];
    //}

    //IEnumerator _clearUserFlag(AudioObject audioObj)
    //{
    //    //此处时间缩放貌似是在不好用
    //    yield return new WaitForSeconds(audioObj.source.clip.length);
    //    audioObj.source.clip = null;
    //    audioObj.isUser = false;
    //}

    //void setAudioVolume(AudioObject[] audios, float volume)
    //{
    //    foreach (var item in audios)
    //    {
    //        item.source.volume = volume;
    //    }
    //}

    ////播放效果音效
    //public void PlaySound(string clipId)
    //{
    //    AudioClip clip = ResourceMgr.Instance.LoadAssetType<AudioClip>(StaticDataMgr.Instance.GetAudioByID(clipId));
    //    AudioObject audioObj = getUnUseAudio();
    //    audioObj.isUser = true;
    //    audioObj.source.clip = clip;
    //    audioObj.source.Play();
    //    StartCoroutine(_clearUserFlag(audioObj));
    //}
    ////播放界面音效
    //public void PlaySound(GameObject go,SoundType type,TriggerType trigger=TriggerType.UI_Open)
    //{
    //    PlaySound sound = go.GetComponent<PlaySound>();
    //    string clipId = "";
    //    switch (type)
    //    {
    //        case SoundType.UI:
    //            if (trigger==TriggerType.UI_Open)
    //                clipId = sound.Sound1;
    //            else
    //                clipId = sound.Sound2;
    //            break;
    //        case SoundType.Tips:
    //        case SoundType.Click:
    //            clipId = sound.Sound1;
    //            break;
    //    }
    //    PlaySound(clipId);
    //}
    ////播放背景音乐
    //public void PlayMusic(string clipId)
    //{
    //    AudioClip clip = ResourceMgr.Instance.LoadAssetType<AudioClip>(StaticDataMgr.Instance.GetAudioByID(clipId));
    //    AudioObject audioObj = getUnUseAudio();
    //    audioObj.isUser = true;
    //    audioObj.source.clip = clip;
    //    audioObj.source.Play();
    //    StartCoroutine(_clearUserFlag(audioObj));
    //}
    private float musicVolume = 0.7f;
    private float soundVolume = 1.0f;
    private AudioSource audioMusic;

    public float MusicVolume
    {
        get { return musicVolume; }
        set { musicVolume = Mathf.Clamp01(value); AudioMusic.volume = musicVolume; }
    }
    public float SoundVolume
    {
        get { return soundVolume; }
        set { soundVolume = Mathf.Clamp01(value); }
    }
    public AudioSource AudioMusic
    {
        get
        {
            if (audioMusic == null)
            {
                GameObject obj = new GameObject("bgMusic");
                obj.transform.parent = transform;
                audioMusic = obj.AddComponent<AudioSource>();
                audioMusic.playOnAwake = false;
                audioMusic.loop = true;
            }
            return audioMusic;
        }
    }

    //播放效果音效
    public void PlaySoundByID(string clipId)
    {
        string clipName = StaticDataMgr.Instance.GetAudioByID(clipId);
        if (string.IsNullOrEmpty(clipName))
        {
            Logger.LogError("not found soundclip by Id:" + clipId);
            return;
        } 
        PlaySoundByName(clipName);
    }

    void Update()
    {
        float curTime = Time.time;
        int count = mSoundEffectList.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            if (curTime - mSoundEffectList[i].triggerTime >= BattleConst.sameAudioClipInterval)
            {
                mSoundEffectList.RemoveAt(i);
            }
        }
    }

    public void PlaySoundByName(string clipName)
    {
        int effectCount = mSoundEffectList.Count;
        for (int i = 0; i < effectCount; ++i)
        {
            if (mSoundEffectList[i].soundName == clipName)
            {
                return;
            }
        }

        mSoundEffectList.Add(new SoundEffectInfo(clipName, Time.time));
        AudioClip clip = ResourceMgr.Instance.LoadAssetType<AudioClip>(clipName);
        if (clip==null)
        {
            Logger.LogError("not found soundclip by Name:" + clipName);
            return;
        }
        AudioSource.PlayClipAtPoint(clip, transform.position, SoundVolume);
    }
    //播放界面音效
    //public void PlaySound(GameObject go, SoundType type, TriggerType trigger = TriggerType.UI_Open)
    //{
    //    PlaySound sound = go.GetComponent<PlaySound>();
    //    string clipId = "";
    //    switch (type)
    //    {
    //        case SoundType.UI:
    //            if (trigger == TriggerType.UI_Open)
    //                clipId = sound.Sound1;
    //            else
    //                clipId = sound.Sound2;
    //            break;
    //        case SoundType.Tips:
    //        case SoundType.Click:
    //            clipId = sound.Sound1;
    //            break;
    //    }
    //    PlaySoundByID(clipId);
    //}

    public void PlayMusicByID(string clipId)
    {
        if (string.IsNullOrEmpty(clipId))
        {
            Logger.LogError("ClipId is Null");
            return;
        }
        string clipName = StaticDataMgr.Instance.GetAudioByID(clipId);
        PlayMusicByName(clipName);
    }
    public void PlayMusicByName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Logger.LogError("Can not found ClipId:" + clipName);
        }
        if (AudioMusic.clip != null && AudioMusic.clip.name == clipName)
        {
            if (AudioMusic.isPlaying)
            {
                return;
            }
            else
            {
                AudioMusic.Play();
                return;
            }
        }
        AudioClip clip = ResourceMgr.Instance.LoadAssetType<AudioClip>(clipName);
        if (clip == null)
        {
            Logger.LogError("Can not found clipName:" + clipName);
            return;
        }
        AudioMusic.clip = clip;
        AudioMusic.Play();
    }
    public void PlayMusic()
    {
        if (AudioMusic.clip!=null)
        {
            AudioMusic.Play();
        }
    }
    public void PauseMusic()
    {
        AudioMusic.Pause();
    }
    public void StopMusic()
    {
        AudioMusic.Stop();
    }
}

//[System.Serializable]
//public class AudioObject
//{
//    //public int id;//好像ID并没什么卵用，本来要用来控制使用的ID
//    public AudioSource source;
//    public bool isUser = false;
//    public bool useTimeScale = false;
//}
