using UnityEngine;
using UnityEngine.UI;

// ���˽ű�ֱ�Ӹ��ӵ�UI��ť��
public class CallButtonController : MonoBehaviour
{
    public DogController dogController; // �Ϸ�С�����󵽴˴�

    void Start()
    {
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