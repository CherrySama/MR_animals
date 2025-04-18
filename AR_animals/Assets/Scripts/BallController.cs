using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject dog; // 在Inspector中拖拽小狗对象
    private bool hasLanded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 当小球与地面碰撞时触发
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !hasLanded)
        {
            hasLanded = true;
            // 通知小狗开始捡球，传入小球的位置
            dog.GetComponent<DogController>().StartFetching(transform.position);
        }
    }
}
