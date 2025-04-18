using UnityEngine;

public class DogController : MonoBehaviour
{
    public GameObject ball; // 小球对象
    public GameObject player; // AR Camera，表示玩家位置
    public float pickUpDistance = 1.0f; // 接近小球的距离
    public float returnDistance = 2.0f; // 接近玩家的距离
    public float moveSpeed = 3.0f; // 小狗移动速度

    private bool isFetching = false;
    private bool hasBall = false;
    private Vector3 targetPosition;

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
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                // 检查是否接近小球
                if (Vector3.Distance(transform.position, ball.transform.position) < pickUpDistance)
                {
                    PickUpBall();
                }
            }
            else
            {
                // 朝玩家移动
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                // 检查是否接近玩家
                if (Vector3.Distance(transform.position, player.transform.position) < returnDistance)
                {
                    ReturnBall();
                }
            }
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
    }
}