using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    public GameObject dog;
    public string groundTag = "Ground"; // 可以在Inspector中设置地面的标签
    public float collisionCooldown = 0.5f; // 碰撞冷却时间，防止多次触发

    private bool hasLanded = false;
    private bool isInCooldown = false; // 是否在冷却期
    private Rigidbody rb;

    void Start()
    {
        // 获取刚体组件(保留注释以备后用)
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 可以在这里添加其他逻辑
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞物体是否有指定标签且球未着陆且不在冷却期
        if (collision.gameObject.CompareTag(groundTag) && !hasLanded && !isInCooldown)
        {
            hasLanded = true;

            // 通知小狗开始捡球，传入小球的位置
            DogController dogController = dog.GetComponent<DogController>();
            if (dogController != null)
            {
                dogController.StartFetching(transform.position);
            }

            // 启动冷却协程，防止短时间内多次触发
            StartCoroutine(CollisionCooldown());
        }
    }

    // 碰撞冷却协程
    private IEnumerator CollisionCooldown()
    {
        isInCooldown = true;
        yield return new WaitForSeconds(collisionCooldown);
        isInCooldown = false;
    }

    // 重置球的状态，使其可以再次触发捡球行为
    public void ResetBall()
    {
        hasLanded = false;

        // 可选：重置物理状态(保留注释以备后用)
        //if (rb != null)
        //{
        //    rb.velocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;
        //}
    }
}