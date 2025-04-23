using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioSequenceManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioItem
    {
        public string name; // 音频名称
        public AudioClip clip; // 音频文件
        public bool selected; // 是否被选中播放
        [Range(0f, 1f)]
        public float volume = 1f; // 音频音量
    }

    [Header("音频设置")]
    public List<AudioItem> audioItems = new List<AudioItem>(); // 所有可用音频
    public float delayBetweenClips = 0.5f; // 音频之间的延迟时间
    public bool playOnAwake = false; // 是否在启动时自动播放
    public bool loop = false; // 是否循环播放序列

    [Header("随机播放设置")]
    public bool randomSelection = false; // 是否随机选择音频
    public int randomCount = 3; // 随机选择的数量

    [Header("事件回调")]
    public UnityEvent onSequenceComplete; // 序列播放完成事件
    public UnityEvent<AudioClip> onClipStart; // 单个音频开始播放事件
    public UnityEvent<AudioClip> onClipEnd; // 单个音频结束播放事件

    private AudioSource audioSource;
    private List<AudioItem> playList = new List<AudioItem>(); // 当前播放列表
    private int currentIndex = 0;
    private bool isPlaying = false;
    private Coroutine playRoutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 初始化设置
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // 如果设置为启动时播放，则准备播放列表
        if (playOnAwake)
        {
            PreparePlaylist();
            StartPlayback();
        }
    }

    // 准备播放列表
    public void PreparePlaylist()
    {
        playList.Clear();

        if (randomSelection)
        {
            // 随机选择指定数量的音频
            List<AudioItem> availableItems = new List<AudioItem>(audioItems);
            int count = Mathf.Min(randomCount, availableItems.Count);

            for (int i = 0; i < count; i++)
            {
                if (availableItems.Count == 0) break;

                int randomIndex = Random.Range(0, availableItems.Count);
                playList.Add(availableItems[randomIndex]);
                availableItems.RemoveAt(randomIndex);
            }
        }
        else
        {
            // 使用已选中的音频
            foreach (AudioItem item in audioItems)
            {
                if (item.selected && item.clip != null)
                {
                    playList.Add(item);
                }
            }
        }

        currentIndex = 0;
    }

    // 开始播放
    public void StartPlayback()
    {
        if (playList.Count == 0)
            PreparePlaylist();
        

        if (playList.Count == 0)
            return;
        

        if (isPlaying)
            StopPlayback();
        

        isPlaying = true;
        playRoutine = StartCoroutine(PlaySequence());
    }

    // 停止播放
    public void StopPlayback()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        audioSource.Stop();
        isPlaying = false;
        currentIndex = 0;
    }

    // 暂停播放
    public void PausePlayback()
    {
        if (isPlaying)
        {
            audioSource.Pause();
        }
    }

    // 继续播放
    public void ResumePlayback()
    {
        if (isPlaying)
        {
            audioSource.UnPause();
        }
    }

    // 播放下一个音频
    public void PlayNext()
    {
        if (isPlaying)
        {
            audioSource.Stop();
            currentIndex++;

            if (currentIndex >= playList.Count)
            {
                if (loop)
                {
                    currentIndex = 0;
                }
                else
                {
                    StopPlayback();
                    onSequenceComplete?.Invoke();
                    return;
                }
            }

            PlayCurrentAudio();
        }
    }

    // 播放上一个音频
    public void PlayPrevious()
    {
        if (isPlaying)
        {
            audioSource.Stop();
            currentIndex--;

            if (currentIndex < 0)
            {
                currentIndex = playList.Count - 1;
            }

            PlayCurrentAudio();
        }
    }

    // 播放当前音频
    private void PlayCurrentAudio()
    {
        if (currentIndex >= 0 && currentIndex < playList.Count)
        {
            AudioItem item = playList[currentIndex];
            audioSource.clip = item.clip;
            audioSource.volume = item.volume;
            audioSource.Play();

            onClipStart?.Invoke(item.clip);
        }
    }

    // 顺序播放所有选中的音频
    private IEnumerator PlaySequence()
    {
        currentIndex = 0;

        while (currentIndex < playList.Count)
        {
            AudioItem item = playList[currentIndex];
            audioSource.clip = item.clip;
            audioSource.volume = item.volume;
            audioSource.Play();

            onClipStart?.Invoke(item.clip);

            // 等待当前音频播放完成
            float clipDuration = item.clip.length;
            yield return new WaitForSeconds(clipDuration);

            onClipEnd?.Invoke(item.clip);

            // 添加音频之间的延迟
            if (delayBetweenClips > 0 && currentIndex < playList.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenClips);
            }

            currentIndex++;

            // 如果是最后一个音频且需要循环播放
            if (currentIndex >= playList.Count && loop)
            {
                currentIndex = 0;
            }
        }

        // 播放完成
        isPlaying = false;
        onSequenceComplete?.Invoke();
    }

    // 为Inspector提供的辅助方法
    public void SelectAll()
    {
        foreach (AudioItem item in audioItems)
        {
            item.selected = true;
        }
    }

    public void DeselectAll()
    {
        foreach (AudioItem item in audioItems)
        {
            item.selected = false;
        }
    }

    // 获取当前播放状态
    public bool IsPlaying()
    {
        return isPlaying;
    }

    // 获取当前播放的音频名称
    public string GetCurrentAudioName()
    {
        if (isPlaying && currentIndex >= 0 && currentIndex < playList.Count)
        {
            return playList[currentIndex].name;
        }
        return string.Empty;
    }
}