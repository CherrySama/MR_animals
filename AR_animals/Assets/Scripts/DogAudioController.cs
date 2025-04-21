using UnityEngine;

public class DogAudioController : MonoBehaviour
{
    public DogController dogController; // ������������
    public AudioSequenceManager backgroundBarkManager; // ��������������

    private bool wasBackgroundPlaying = false; // ��¼��������״̬

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

    // ������н����¼�
    private void HandleCallingEnded()
    {
        // �ָ��������֣����֮ǰ���ڲ��ţ�
        if (backgroundBarkManager != null && wasBackgroundPlaying)
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
        // ���������ѭ�������н����ˣ����¿�ʼ����
        if (backgroundBarkManager.loop && !backgroundBarkManager.IsPlaying())
        {
            backgroundBarkManager.PreparePlaylist();
            backgroundBarkManager.StartPlayback();
        }
    }
}