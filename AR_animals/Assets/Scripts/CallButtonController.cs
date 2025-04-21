using UnityEngine;
using UnityEngine.UI;

// ���˽ű�ֱ�Ӹ��ӵ�UI��ť��
public class CallButtonController : MonoBehaviour
{
    public DogController dogController; // �Ϸ�С�����󵽴˴�

    void Start()
    {
        // ��ȡ��ť�������ӵ���¼�
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(CallDog);
        }
        else
        {
            Debug.LogError("�뽫�˽ű����ӵ�����Button�����GameObject�ϣ�");
        }

        // ����Ƿ�������DogController
        if (dogController == null)
        {
            Debug.LogWarning("����Inspector��ΪCallButtonController����DogController���ã�");
        }
    }

    // ��ť���ʱ���ô˷���
    public void CallDog()
    {
        if (dogController != null)
        {
            dogController.CallDog();
            Debug.Log("�Ѻ���С��");
        }
        else
        {
            Debug.LogError("δ����DogController���ã��޷�����С����");
        }
    }
}