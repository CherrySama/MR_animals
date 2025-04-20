using UnityEngine;

public class DogController : MonoBehaviour
{
    public GameObject ball; // 小球对象
    public GameObject player; // AR Camera，表示玩家位置
    public float pickUpDistance = 1.0f; // 接近小球的距离
    public float returnDistance = 2.0f; // 接近玩家的距离
    public float moveSpeed = 3.0f; // 小狗移动速度
    public float rotationSpeed = 10.0f; // 小狗旋转速度

    private bool isFetching = false;
    private bool hasBall = false;
    private Vector3 targetPosition;
    private Animator animator; // 动画控制器

    // Animator参数名称
    private readonly string isRunningParam = "isRunning";

    void Start()
    {
        // 获取Animator组件
        animator = GetComponent<Animator>();

        // 确保开始时不在跑步状态
        SetRunning(false);
    }

    // 开始捡球，设置目标为小球位置
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
                // 朝小球移动
                MoveTowards(targetPosition);
                // 检查是否接近小球
                if (Vector3.Distance(transform.position, ball.transform.position) < pickUpDistance)
                {
                    PickUpBall();
                }
            }
            else
            {
                // 朝玩家移动
                MoveTowards(player.transform.position);                
                // 检查是否接近玩家
                if (Vector3.Distance(transform.position, player.transform.position) < returnDistance)
                {
                    ReturnBall();
                }
            }
        }
        else
        {
            // 不在捡球状态，确保停止动画
            SetRunning(false);
        }
    }

    // 设置运行状态
    void SetRunning(bool running)
    {
        if (animator != null)
        {
            animator.SetBool(isRunningParam, running);
        }
    }

    // 移动函数，处理移动和旋转
    void MoveTowards(Vector3 target)
    {
        // 计算方向
        Vector3 direction = target - transform.position;
        direction.y = 0; // 保持Y轴不变，防止小狗倾斜

        // 只有当有实际距离需要移动时才旋转和移动
        if (direction.magnitude > 0.1f)
        {
            // 旋转朝向目标
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 移动
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            // 确保在移动时播放跑步动画
            SetRunning(true);
        }
        else
        {
            // 如果到达目标位置，停止跑步动画
            SetRunning(false);
        }
    }

    // 捡起小球
    void PickUpBall()
    {
        ball.SetActive(false); // 小球消失
        hasBall = true;
        targetPosition = player.transform.position; // 设置新目标为玩家位置
    }

    // 还球给玩家
    void ReturnBall()
    {
        // 小球出现在玩家前方1.5米处
        ball.transform.position = player.transform.position + player.transform.forward * 1.5f;
        ball.SetActive(true); // 小球重现
        hasBall = false;
        isFetching = false; // 停止移动

        // 停止移动时播放idle动画
        SetRunning(false);
    }
}