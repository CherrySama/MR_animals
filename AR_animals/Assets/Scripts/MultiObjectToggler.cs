using UnityEngine;
using System.Collections.Generic;

public class MultiObjectToggler : MonoBehaviour
{
    [System.Serializable]
    public class ToggleableObject
    {
        public GameObject targetObject;           // 要控制的游戏对象
        public Vector3 spawnPosition;             // 显示时的位置
        public Quaternion spawnRotation;          // 显示时的旋转
    }

    public List<ToggleableObject> objects = new List<ToggleableObject>();  // 要控制的对象列表
    public bool startHidden = true;               // 是否在开始时隐藏所有对象

    private bool isVisible = false;               // 当前是否可见

    void Start()
    {
        // 初始化所有对象
        foreach (var obj in objects)
        {
            if (obj.targetObject != null)
            {
                // 保存初始位置和旋转（如果未手动设置）
                if (obj.spawnPosition == Vector3.zero)
                {
                    obj.spawnPosition = obj.targetObject.transform.position;
                }
                if (obj.spawnRotation == Quaternion.identity)
                {
                    obj.spawnRotation = obj.targetObject.transform.rotation;
                }

                // 根据初始设置显示或隐藏
                obj.targetObject.SetActive(!startHidden);
            }
        }

        isVisible = !startHidden;
    }

    // 切换所有对象的可见性
    public void ToggleAllObjects()
    {
        isVisible = !isVisible;

        foreach (var obj in objects)
        {
            if (obj.targetObject != null)
            {
                // 如果要显示，先设置位置再激活
                if (isVisible)
                {
                    obj.targetObject.transform.position = obj.spawnPosition;
                    obj.targetObject.transform.rotation = obj.spawnRotation;
                }

                obj.targetObject.SetActive(isVisible);
            }
        }
    }

    // 显示所有对象
    public void ShowAllObjects()
    {
        if (!isVisible)
        {
            ToggleAllObjects();
        }
    }

    // 隐藏所有对象
    public void HideAllObjects()
    {
        if (isVisible)
        {
            ToggleAllObjects();
        }
    }
}