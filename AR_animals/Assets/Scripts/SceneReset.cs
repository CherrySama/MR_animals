using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneReset : MonoBehaviour
{
    [Header("���뵭������")]
    public Material fadeMaterial; // ���ڵ��뵭���Ĳ���
    public float fadeDuration = 1.0f; // ���뵭������ʱ��

    private GameObject fadeObject; // ���ڵ��뵭��������
    private Renderer fadeRenderer; // ���뵭���������Ⱦ�����

    private void Awake()
    {
        // ȷ����������ʱ��Ҫ��ʾ����Ч��
        InitializeFadeObject();
        SetFadeAlpha(0);
        fadeObject.SetActive(false);
    }

    // ��ʼ�����ڵ��뵭����3D����
    private void InitializeFadeObject()
    {
        if (fadeObject != null) return;

        // ����һ���򵥵��ı���ƽ�棬�����ǰ��
        fadeObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fadeObject.name = "FadeQuad";

        // ���ò���
        fadeRenderer = fadeObject.GetComponent<Renderer>();
        if (fadeMaterial != null)
        {
            fadeRenderer.material = new Material(fadeMaterial);
        }
        else
        {
            // ����һ���µ�͸������
            Material mat = new Material(Shader.Find("Transparent/Diffuse"));
            mat.color = Color.black;
            fadeRenderer.material = mat;
        }

        // ������ײ��
        Collider collider = fadeObject.GetComponent<Collider>();
        if (collider != null) Destroy(collider);

        // ����Ϊ������Ӷ��󣬲���λ
        Camera arCamera = Camera.main;
        if (arCamera != null)
        {
            fadeObject.transform.parent = arCamera.transform;
            fadeObject.transform.localPosition = new Vector3(0, 0, 0.3f);
            fadeObject.transform.localRotation = Quaternion.identity;

            // �����Ը���������Ұ
            fadeObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        // ��ʼ״̬Ϊ����
        fadeObject.SetActive(false);
    }

    // ���õ��뵭�������͸����
    private void SetFadeAlpha(float alpha)
    {
        if (fadeRenderer != null && fadeRenderer.material != null)
        {
            Color color = fadeRenderer.material.color;
            color.a = alpha;
            fadeRenderer.material.color = color;
        }
    }

    // ���õ�ǰ����
    public void ResetScene()
    {
        // ��ȡ��ǰ�����
        Scene currentScene = SceneManager.GetActiveScene();

        // ���¼��ص�ǰ����
        SceneManager.LoadScene(currentScene.name);

        Debug.Log("����������");
    }

    // �����뵭��Ч���ĳ�������
    public void ResetSceneWithFade()
    {
        // ���õ����ٵ����Э��
        StartCoroutine(FadeAndReset());
    }

    private IEnumerator FadeAndReset()
    {
        // ȷ�����뵭�������ѳ�ʼ��
        InitializeFadeObject();

        // ����뵭������
        fadeObject.SetActive(true);

        // ����Ч������͸������ɫ��
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        // ȷ����ȫ��͸��
        SetFadeAlpha(1.0f);

        // ���¼��س���
        Scene currentScene = SceneManager.GetActiveScene();

        // �첽���س����Ա��ڼ�����ɺ���Ƶ���
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene.name);

        // �ȴ������������
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // �ȴ�һ֡��ȷ�������ѳ�ʼ��
        yield return null;

        // ���»�ȡ������������õ��뵭������
        // ��Ϊ�������غ�֮ǰ��������ʧЧ
        InitializeFadeObject();
        fadeObject.SetActive(true);
        SetFadeAlpha(1.0f);

        // ����Ч�����Ӻ�ɫ��͸����
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Clamp01(elapsedTime / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        // ��ɺ����ص��뵭������
        SetFadeAlpha(0);
        fadeObject.SetActive(false);
    }

    // �������һ��OnDestroy������������Դ
    private void OnDestroy()
    {
        if (fadeObject != null)
        {
            Destroy(fadeObject);
        }
    }
}