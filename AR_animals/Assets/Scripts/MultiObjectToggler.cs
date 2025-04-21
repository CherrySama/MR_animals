using UnityEngine;
using System.Collections.Generic;

public class MultiObjectToggler : MonoBehaviour
{
    [System.Serializable]
    public class ToggleableObject
    {
        public GameObject targetObject;           // Ҫ���Ƶ���Ϸ����
        public Vector3 spawnPosition;             // ��ʾʱ��λ��
        public Quaternion spawnRotation;          // ��ʾʱ����ת
    }

    public List<ToggleableObject> objects = new List<ToggleableObject>();  // Ҫ���ƵĶ����б�
    public bool startHidden = true;               // �Ƿ��ڿ�ʼʱ�������ж���

    private bool isVisible = false;               // ��ǰ�Ƿ�ɼ�

    void Start()
    {
        // ��ʼ�����ж���
        foreach (var obj in objects)
        {
            if (obj.targetObject != null)
            {
                // �����ʼλ�ú���ת�����δ�ֶ����ã�
                if (obj.spawnPosition == Vector3.zero)
                {
                    obj.spawnPosition = obj.targetObject.transform.position;
                }
                if (obj.spawnRotation == Quaternion.identity)
                {
                    obj.spawnRotation = obj.targetObject.transform.rotation;
                }

                // ���ݳ�ʼ������ʾ������
                obj.targetObject.SetActive(!startHidden);
            }
        }

        isVisible = !startHidden;
    }

    // �л����ж���Ŀɼ���
    public void ToggleAllObjects()
    {
        isVisible = !isVisible;

        foreach (var obj in objects)
        {
            if (obj.targetObject != null)
            {
                // ���Ҫ��ʾ��������λ���ټ���
                if (isVisible)
                {
                    obj.targetObject.transform.position = obj.spawnPosition;
                    obj.targetObject.transform.rotation = obj.spawnRotation;
                }

                obj.targetObject.SetActive(isVisible);
            }
        }
    }

    // ��ʾ���ж���
    public void ShowAllObjects()
    {
        if (!isVisible)
        {
            ToggleAllObjects();
        }
    }

    // �������ж���
    public void HideAllObjects()
    {
        if (isVisible)
        {
            ToggleAllObjects();
        }
    }
}