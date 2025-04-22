using UnityEngine;

public class DogAudioController : MonoBehaviour
{
    public DogController dogController; // 狗控制器引用
    public AudioSequenceManager backgroundBarkManager; // 背景叫声管理器

    private bool wasBackgroundPlaying = false; // 记录背景叫声状态
    private bool isMuted = false; // 是否手动静音
    private bool wasPlayingBeforeMute = false; // 静音前是否在播放

    void Start()
    {
        // 检查引用
        if (dogController == null)
        {
            Debug.LogError("未找到DogController组件！");
            enabled = false;
            return;
        }

        if (backgroundBarkManager == null)
        {
            Debug.LogError("未找到AudioSequenceManager组件！");
            enabled = false;
            return;
        }

        // 订阅事件
        dogController.OnCallingStarted += HandleCallingStarted;
        dogController.OnCallingEnded += HandleCallingEnded;

        // 设置事件回调以在序列完成时重启（确保循环）
        backgroundBarkManager.onSequenceComplete.AddListener(HandleSequenceComplete);
    }

    void OnDestroy()
    {
        // 取消订阅事件以防止内存泄漏
        if (dogController != null)
        {
            dogController.OnCallingStarted -= HandleCallingStarted;
            dogController.OnCallingEnded -= HandleCallingEnded;
        }
    }

    // 处理呼叫开始事件
    private void HandleCallingStarted()
    {
        // 只在未手动静音的情况下处理
        if (!isMuted)
        {
            // 保存背景音乐状态并停止
            if (backgroundBarkManager != null)
            {
                wasBackgroundPlaying = backgroundBarkManager.IsPlaying();
                if (wasBackgroundPlaying)
                {
                    backgroundBarkManager.StopPlayback();
                    Debug.Log("呼叫开始：背景叫声已停止");
                }
            }
        }
        else
        {
            // 已手动静音，只记录状态
            wasBackgroundPlaying = false;
        }
    }

    // 处理呼叫结束事件
    private void HandleCallingEnded()
    {
        // 只有在未手动静音的情况下才恢复背景声音
        if (!isMuted && backgroundBarkManager != null && wasBackgroundPlaying)
        {
            backgroundBarkManager.PreparePlaylist();
            backgroundBarkManager.StartPlayback();
            Debug.Log("呼叫结束：背景叫声已恢复");
            wasBackgroundPlaying = false;
        }
    }

    // 处理序列完成事件（确保循环播放）
    private void HandleSequenceComplete()
    {
        // 如果设置了循环但序列结束了，且未手动静音，重新开始播放
        if (!isMuted && backgroundBarkManager.loop && !backgroundBarkManager.IsPlaying())
        {
            backgroundBarkManager.PreparePlaylist();
            backgroundBarkManager.StartPlayback();
        }
    }

    public void ToggleBackgroundSound()
    {
        if (backgroundBarkManager == null) return;

        if (isMuted)
        {
            // 如果当前是静音状态，恢复声音
            if (wasPlayingBeforeMute)
            {
                backgroundBarkManager.PreparePlaylist();
                backgroundBarkManager.StartPlayback();
            }
            isMuted = false;
            Debug.Log("背景叫声已恢复");
        }
        else
        {
            // 如果当前有声音，静音
            wasPlayingBeforeMute = backgroundBarkManager.IsPlaying();
            if (wasPlayingBeforeMute)
            {
                backgroundBarkManager.StopPlayback();
            }
            isMuted = true;
            Debug.Log("背景叫声已静音");
        }
    }
}