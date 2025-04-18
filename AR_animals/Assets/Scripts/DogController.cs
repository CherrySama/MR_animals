using UnityEngine;

public class DogController : MonoBehaviour
{
    public GameObject ball; // С�����
    public GameObject player; // AR Camera����ʾ���λ��
    public float pickUpDistance = 1.0f; // �ӽ�С��ľ���
    public float returnDistance = 2.0f; // �ӽ���ҵľ���
    public float moveSpeed = 3.0f; // С���ƶ��ٶ�

    private bool isFetching = false;
    private bool hasBall = false;
    private Vector3 targetPosition;

    // ��ʼ��������Ŀ��ΪС��λ��
    public void StartFetching(Vector3 ballPosition)
    {
        if (!isFetching)
        {
            isFetching = true;
            targetPosition = ballPosition;
        }
    }

    void Update()
    {
        if (isFetching)
        {
            if (!hasBall)
            {
                // ��С���ƶ�
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                // ����Ƿ�ӽ�С��
                if (Vector3.Distance(transform.position, ball.transform.position) < pickUpDistance)
                {
                    PickUpBall();
                }
            }
            else
            {
                // ������ƶ�
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                // ����Ƿ�ӽ����
                if (Vector3.Distance(transform.position, player.transform.position) < returnDistance)
                {
                    ReturnBall();
                }
            }
        }
    }

    // ����С��
    void PickUpBall()
    {
        ball.SetActive(false); // С����ʧ
        hasBall = true;
        targetPosition = player.transform.position; // ������Ŀ��Ϊ���λ��
    }

    // ��������
    void ReturnBall()
    {
        // С����������ǰ��1.5�״�
        ball.transform.position = player.transform.position + player.transform.forward * 1.5f;
        ball.SetActive(true); // С������
        hasBall = false;
        isFetching = false; // ֹͣ�ƶ�
    }
}