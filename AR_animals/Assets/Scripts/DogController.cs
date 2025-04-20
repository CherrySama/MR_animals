using UnityEngine;

public class DogController : MonoBehaviour
{
    public GameObject ball; // С�����
    public GameObject player; // AR Camera����ʾ���λ��
    public float pickUpDistance = 1.0f; // �ӽ�С��ľ���
    public float returnDistance = 2.0f; // �ӽ���ҵľ���
    public float moveSpeed = 3.0f; // С���ƶ��ٶ�
    public float rotationSpeed = 10.0f; // С����ת�ٶ�

    private bool isFetching = false;
    private bool hasBall = false;
    private Vector3 targetPosition;
    private Animator animator; // ����������

    // Animator��������
    private readonly string isRunningParam = "isRunning";

    void Start()
    {
        // ��ȡAnimator���
        animator = GetComponent<Animator>();

        // ȷ����ʼʱ�����ܲ�״̬
        SetRunning(false);
    }

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
                MoveTowards(targetPosition);
                // ����Ƿ�ӽ�С��
                if (Vector3.Distance(transform.position, ball.transform.position) < pickUpDistance)
                {
                    PickUpBall();
                }
            }
            else
            {
                // ������ƶ�
                MoveTowards(player.transform.position);                
                // ����Ƿ�ӽ����
                if (Vector3.Distance(transform.position, player.transform.position) < returnDistance)
                {
                    ReturnBall();
                }
            }
        }
        else
        {
            // ���ڼ���״̬��ȷ��ֹͣ����
            SetRunning(false);
        }
    }

    // ��������״̬
    void SetRunning(bool running)
    {
        if (animator != null)
        {
            animator.SetBool(isRunningParam, running);
        }
    }

    // �ƶ������������ƶ�����ת
    void MoveTowards(Vector3 target)
    {
        // ���㷽��
        Vector3 direction = target - transform.position;
        direction.y = 0; // ����Y�᲻�䣬��ֹС����б

        // ֻ�е���ʵ�ʾ�����Ҫ�ƶ�ʱ����ת���ƶ�
        if (direction.magnitude > 0.1f)
        {
            // ��ת����Ŀ��
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // �ƶ�
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            // ȷ�����ƶ�ʱ�����ܲ�����
            SetRunning(true);
        }
        else
        {
            // �������Ŀ��λ�ã�ֹͣ�ܲ�����
            SetRunning(false);
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

        // ֹͣ�ƶ�ʱ����idle����
        SetRunning(false);
    }
}