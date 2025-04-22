using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneReset : MonoBehaviour
{
    [Header("淡入淡出设置")]
    public Material fadeMaterial; // 用于淡入淡出的材质
    public float fadeDuration = 1.0f; // 淡入淡出持续时间

    private GameObject fadeObject; // 用于淡入淡出的物体
    private Renderer fadeRenderer; // 淡入淡出物体的渲染器组件

    private void Awake()
    {
        // 确保场景启动时不要显示过渡效果
        InitializeFadeObject();
        SetFadeAlpha(0);
        fadeObject.SetActive(false);
    }

    // 初始化用于淡入淡出的3D对象
    private void InitializeFadeObject()
    {
        if (fadeObject != null) return;

        // 创建一个简单的四边形平面，在相机前方
        fadeObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fadeObject.name = "FadeQuad";

        // 设置材质
        fadeRenderer = fadeObject.GetComponent<Renderer>();
        if (fadeMaterial != null)
        {
            fadeRenderer.material = new Material(fadeMaterial);
        }
        else
        {
            // 创建一个新的透明材质
            Material mat = new Material(Shader.Find("Transparent/Diffuse"));
            mat.color = Color.black;
            fadeRenderer.material = mat;
        }

        // 禁用碰撞体
        Collider collider = fadeObject.GetComponent<Collider>();
        if (collider != null) Destroy(collider);

        // 设置为相机的子对象，并定位
        Camera arCamera = Camera.main;
        if (arCamera != null)
        {
            fadeObject.transform.parent = arCamera.transform;
            fadeObject.transform.localPosition = new Vector3(0, 0, 0.3f);
            fadeObject.transform.localRotation = Quaternion.identity;

            // 缩放以覆盖整个视野
            fadeObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        // 初始状态为隐藏
        fadeObject.SetActive(false);
    }

    // 设置淡入淡出对象的透明度
    private void SetFadeAlpha(float alpha)
    {
        if (fadeRenderer != null && fadeRenderer.material != null)
        {
            Color color = fadeRenderer.material.color;
            color.a = alpha;
            fadeRenderer.material.color = color;
        }
    }

    // 重置当前场景
    public void ResetScene()
    {
        // 获取当前活动场景
        Scene currentScene = SceneManager.GetActiveScene();

        // 重新加载当前场景
        SceneManager.LoadScene(currentScene.name);

        Debug.Log("场景已重置");
    }

    // 带淡入淡出效果的场景重置
    public void ResetSceneWithFade()
    {
        // 调用淡出再淡入的协程
        StartCoroutine(FadeAndReset());
    }

    private IEnumerator FadeAndReset()
    {
        // 确保淡入淡出对象已初始化
        InitializeFadeObject();

        // 激活淡入淡出对象
        fadeObject.SetActive(true);

        // 淡出效果（从透明到黑色）
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        // 确保完全不透明
        SetFadeAlpha(1.0f);

        // 重新加载场景
        Scene currentScene = SceneManager.GetActiveScene();

        // 异步加载场景以便在加载完成后控制淡入
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene.name);

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 等待一帧以确保场景已初始化
        yield return null;

        // 重新获取相机并重新设置淡入淡出对象
        // 因为场景重载后，之前的引用已失效
        InitializeFadeObject();
        fadeObject.SetActive(true);
        SetFadeAlpha(1.0f);

        // 淡入效果（从黑色到透明）
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Clamp01(elapsedTime / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        // 完成后隐藏淡入淡出对象
        SetFadeAlpha(0);
        fadeObject.SetActive(false);
    }

    // 可以添加一个OnDestroy方法来清理资源
    private void OnDestroy()
    {
        if (fadeObject != null)
        {
            Destroy(fadeObject);
        }
    }
}