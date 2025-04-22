using UnityEngine;

public class DogAudioController : MonoBehaviour
{
    public DogController dogController; // ������������
    public AudioSequenceManager backgroundBarkManager; // ��������������

    private bool wasBackgroundPlaying = false; // ��¼��������״̬
    private bool isMuted = false; // �Ƿ��ֶ�����
    private bool wasPlayingBeforeMute = false; // ����ǰ�Ƿ��ڲ���

    void Start()
    {
        // �������
        if (dogController == null)
        {
            Debug.LogError("δ�ҵ�DogController�����");
            enabled = false;
            return;
        }

        if (backgroundBarkManager == null)
        {
            Debug.LogError("δ�ҵ�AudioSequenceManager�����");
            enabled = false;
            return;
        }

        // �����¼�
        dogController.OnCallingStarted += HandleCallingStarted;
        dogController.OnCallingEnded += HandleCallingEnded;

        // �����¼��ص������������ʱ������ȷ��ѭ����
        backgroundBarkManager.onSequenceComplete.AddListener(HandleSequenceComplete);
    }

    void OnDestroy()
    {
        // ȡ�������¼��Է�ֹ�ڴ�й©
        if (dogController != null)
        {
            dogController.OnCallingStarted -= HandleCallingStarted;
            dogController.OnCallingEnded -= HandleCallingEnded;
        }
    }

    // ������п�ʼ�¼�
    private void HandleCallingStarted()
    {
        // ֻ��δ�ֶ�����������´���
        if (!isMuted)
        {
            // ���汳������״̬��ֹͣ
            if (backgroundBarkManager != null)
            {
                wasBackgroundPlaying = backgroundBarkManager.IsPlaying();
                if (wasBackgroundPlaying)
                {
                    backgroundBarkManager.StopPlayback();
                    Debug.Log("���п�ʼ������������ֹͣ");
                }
            }
        }
        else
        {
            // ���ֶ�������ֻ��¼״̬
            wasBackgroundPlaying = false;
        }
    }

    // ������н����¼�
    private void HandleCallingEnded()
    {
        // ֻ����δ�ֶ�����������²Żָ���������
        if (!isMuted && backgroundBarkManager != null && wasBackgroundPlaying)
        {
            backgroundBarkManager.PreparePlaylist();
            backgroundBarkManager.StartPlayback();
            Debug.Log("���н��������������ѻָ�");
            wasBackgroundPlaying = false;
        }
    }

    // ������������¼���ȷ��ѭ�����ţ�
    private void HandleSequenceComplete()
    {
        // ���������ѭ�������н����ˣ���δ�ֶ����������¿�ʼ����
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
            // �����ǰ�Ǿ���״̬���ָ�����
            if (wasPlayingBeforeMute)
            {
                backgroundBarkManager.PreparePlaylist();
                backgroundBarkManager.StartPlayback();
            }
            isMuted = false;
            Debug.Log("���������ѻָ�");
        }
        else
        {
            // �����ǰ������������
            wasPlayingBeforeMute = backgroundBarkManager.IsPlaying();
            if (wasPlayingBeforeMute)
            {
                backgroundBarkManager.StopPlayback();
            }
            isMuted = true;
            Debug.Log("���������Ѿ���");
        }
    }
}