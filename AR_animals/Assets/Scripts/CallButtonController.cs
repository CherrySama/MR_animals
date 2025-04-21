using UnityEngine;
using UnityEngine.UI;

// 将此脚本直接附加到UI按钮上
public class CallButtonController : MonoBehaviour
{
    public DogController dogController; // 拖放小狗对象到此处

    void Start()
    {
        // 获取按钮组件并添加点击事件
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(CallDog);
        }
        else
        {
            Debug.LogError("请将此脚本附加到带有Button组件的GameObject上！");
        }

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
            Debug.Log("已呼叫小狗");
        }
        else
        {
            Debug.LogError("未设置DogController引用，无法呼叫小狗！");
        }
    }
}