using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject dog; // ��Inspector����קС������
    private bool hasLanded = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // ��С���������ײʱ����
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !hasLanded)
        {
            hasLanded = true;
            // ֪ͨС����ʼ���򣬴���С���λ��
            dog.GetComponent<DogController>().StartFetching(transform.position);
        }
    }
}
