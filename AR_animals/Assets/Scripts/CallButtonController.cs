using UnityEngine;
using UnityEngine.UI;

// 将此脚本直接附加到UI按钮上
public class CallButtonController : MonoBehaviour
{
    public DogController dogController; // 拖放小狗对象到此处

    void Start()
    {
        // 检查是否已设置DogController
        if (dogController == null)
        {
            Debug.LogWarning("请在Inspector中为CallButtonController设置DogController引用！");
        }
    }

    // 按钮点击时调用此方法
    public void CallDog()
    {
        if (dogController != null)
        {
            dogController.CallDog();
        }
        else
        {
            Debug.LogError("未设置DogController引用，无法呼叫小狗！");
        }
    }
}