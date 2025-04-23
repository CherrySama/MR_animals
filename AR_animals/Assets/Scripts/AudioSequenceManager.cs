using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioSequenceManager : MonoBehaviour
{
    [System.Serializable]
    public class AudioItem
    {
        public string name; // ��Ƶ����
        public AudioClip clip; // ��Ƶ�ļ�
        public bool selected; // �Ƿ�ѡ�в���
        [Range(0f, 1f)]
        public float volume = 1f; // ��Ƶ����
    }

    [Header("��Ƶ����")]
    public List<AudioItem> audioItems = new List<AudioItem>(); // ���п�����Ƶ
    public float delayBetweenClips = 0.5f; // ��Ƶ֮����ӳ�ʱ��
    public bool playOnAwake = false; // �Ƿ�������ʱ�Զ�����
    public bool loop = false; // �Ƿ�ѭ����������

    [Header("�����������")]
    public bool randomSelection = false; // �Ƿ����ѡ����Ƶ
    public int randomCount = 3; // ���ѡ�������

    [Header("�¼��ص�")]
    public UnityEvent onSequenceComplete; // ���в�������¼�
    public UnityEvent<AudioClip> onClipStart; // ������Ƶ��ʼ�����¼�
    public UnityEvent<AudioClip> onClipEnd; // ������Ƶ���������¼�

    private AudioSource audioSource;
    private List<AudioItem> playList = new List<AudioItem>(); // ��ǰ�����б�
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

        // ��ʼ������
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // �������Ϊ����ʱ���ţ���׼�������б�
        if (playOnAwake)
        {
            PreparePlaylist();
            StartPlayback();
        }
    }

    // ׼�������б�
    public void PreparePlaylist()
    {
        playList.Clear();

        if (randomSelection)
        {
            // ���ѡ��ָ����������Ƶ
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
            // ʹ����ѡ�е���Ƶ
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

    // ��ʼ����
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

    // ֹͣ����
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

    // ��ͣ����
    public void PausePlayback()
    {
        if (isPlaying)
        {
            audioSource.Pause();
        }
    }

    // ��������
    public void ResumePlayback()
    {
        if (isPlaying)
        {
            audioSource.UnPause();
        }
    }

    // ������һ����Ƶ
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

    // ������һ����Ƶ
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

    // ���ŵ�ǰ��Ƶ
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

    // ˳�򲥷�����ѡ�е���Ƶ
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

            // �ȴ���ǰ��Ƶ�������
            float clipDuration = item.clip.length;
            yield return new WaitForSeconds(clipDuration);

            onClipEnd?.Invoke(item.clip);

            // �����Ƶ֮����ӳ�
            if (delayBetweenClips > 0 && currentIndex < playList.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenClips);
            }

            currentIndex++;

            // ��������һ����Ƶ����Ҫѭ������
            if (currentIndex >= playList.Count && loop)
            {
                currentIndex = 0;
            }
        }

        // �������
        isPlaying = false;
        onSequenceComplete?.Invoke();
    }

    // ΪInspector�ṩ�ĸ�������
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

    // ��ȡ��ǰ����״̬
    public bool IsPlaying()
    {
        return isPlaying;
    }

    // ��ȡ��ǰ���ŵ���Ƶ����
    public string GetCurrentAudioName()
    {
        if (isPlaying && currentIndex >= 0 && currentIndex < playList.Count)
        {
            return playList[currentIndex].name;
        }
        return string.Empty;
    }
}