using UnityEngine;
using System.Collections;

public class DogController : MonoBehaviour
{
    [Header("对象引用")]
    public GameObject ball; // 小球对象
    public GameObject player; // AR Camera，表示玩家位置
    public GameObject plate; // 归还球的目标位置(Plate)
    public AudioSource audioSource; // 音频播放组件
    public AudioClip callingSound; // 呼叫时播放的音效
    // 移除 backgroundBarkManager 引用

    [Header("位置设置")]
    public float plateHeightOffset = 0.1f; // Plate上方的偏移高度
    public float pickUpDistance = 1.0f; // 接近小球的距离
    public float returnDistance = 2.0f; // 接近玩家的距离
    public Vector3 initialPosition; // 初始位置（会自动保存）

    [Header("速度设置")]
    public float moveSpeed = 3.0f; // 移动速度
    public float runSpeed = 5.0f; // 奔跑速度（用于呼叫和捡球）
    public float walkSpeed = 1.5f; // 漫步速度
    public float rotationSpeed = 10.0f; // 旋转速度

    [Header("状态设置")]
    public float idleTime = 2f; // 归还后保持idle状态的时间
    public float callingStayTime = 5f; // 呼叫后待在玩家面前的时间

    [Header("随机漫步设置")]
    public bool enableRandomWalk = true; // 是否启用随机漫步
    public float walkRadius = 3.0f; // 随机漫步半径
    public float minIdleTime = 3.0f; // 最短待机时间
    public float maxIdleTime = 8.0f; // 最长待机时间
    public float minWalkTime = 2.0f; // 最短漫步时间
    public float maxWalkTime = 6.0f; // 最长漫步时间

    [Header("追球设置")]
    public float ballTrackingUpdateRate = 0.2f; // 追踪球位置的更新频率(秒)
    private Coroutine ballTrackingCoroutine; // 追踪球位置的协程

    // 动画参数
    private readonly string isRunningParam = "isRunning";
    private readonly string isWalkingParam = "isWalking";

    // 状态变量
    private bool isFetching = false; // 是否在捡球
    private bool hasBall = false; // 是否持有球
    private bool isInIdleCooldown = false; // 是否在归还后的冷却时间内
    private bool isWalking = false; // 是否在随机漫步
    private bool isIdle = true; // 是否在待机
    private bool isCalled = false; // 是否被呼叫
    private bool isReturningToInitial = false; // 是否在返回初始位置

    // 位置和计时
    private Vector3 targetPosition; // 当前目标位置
    private Vector3 randomWalkTarget; // 随机漫步目标
    private float stateTimer = 0f; // 状态计时器
    private float currentStateTime = 0f; // 当前状态持续时间

    // 组件引用
    private Animator animator;

    // 音频事件 - 供外部监听
    public delegate void AudioEventHandler();
    public event AudioEventHandler OnCallingStarted;
    public event AudioEventHandler OnCallingEnded;

    void Start()
    {
        // 获取组件
        animator = GetComponent<Animator>();

        // 如果未设置音频源，尝试获取
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && callingSound != null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 保存初始位置
        initialPosition = transform.position;

        // 设置初始动画状态
        SetRunning(false);
        SetWalking(false);

        // 如果启用随机漫步，开始随机行为
        if (enableRandomWalk)
            StartCoroutine(RandomBehavior());
    }

    void Update()
    {
        // 优先处理捡球、呼叫和返回初始位置的逻辑
        if (!isInIdleCooldown && isFetching)
        {
            HandleFetching();
        }
        else if (isCalled)
        {
            // 处理被呼叫状态
            MoveTowards(player.transform.position, runSpeed);

            // 如果接近玩家，停下并开始计时
            if (Vector3.Distance(transform.position, player.transform.position) < returnDistance)
            {
                StartCoroutine(StayWithPlayer());
            }
        }
        else if (isReturningToInitial)
        {
            // 返回初始位置
            MoveTowards(initialPosition, moveSpeed);

            // 如果接近初始位置，恢复随机行为
            if (Vector3.Distance(transform.position, initialPosition) < 0.5f)
            {
                isReturningToInitial = false;

                // 触发事件通知背景音效可以恢复
                if (OnCallingEnded != null)
                {
                    OnCallingEnded.Invoke();
                }

                // 等待一帧后再开始随机行为，避免状态冲突
                StartCoroutine(DelayedRandomBehavior());
            }
        }
    }

    // 留在玩家身边一段时间
    IEnumerator StayWithPlayer()
    {
        // 停止移动
        isCalled = false;

        // 触发调用开始事件 - 通知背景音乐暂停
        if (OnCallingStarted != null)
        {
            OnCallingStarted.Invoke();
        }

        // 播放呼叫音效
        if (audioSource != null && callingSound != null)
        {
            audioSource.clip = callingSound;
            audioSource.Play();
        }

        // 面向玩家
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

        // 设置为idle状态
        SetWalking(false);
        SetRunning(false);

        // 等待指定时间
        yield return new WaitForSeconds(callingStayTime);

        // 开始返回初始位置
        isReturningToInitial = true;
        SetWalking(true);  // 使用走路而不是跑步返回
        SetRunning(false); // 确保跑步动画关闭
    }

    private IEnumerator DelayedRandomBehavior()
    {
        yield return null;
        isIdle = true;
        isWalking = false;
        if (enableRandomWalk && !isFetching && !isCalled)
        {
            StartCoroutine(RandomBehavior());
        }
    }

    void HandleFetching()
    {
        if (!hasBall)
        {
            MoveTowards(targetPosition, runSpeed);
            if (Vector3.Distance(transform.position, ball.transform.position) < pickUpDistance)
            {
                PickUpBall();
            }
        }
        else
        {
            MoveTowards(player.transform.position, runSpeed);
            if (Vector3.Distance(transform.position, player.transform.position) < returnDistance)
            {
                ReturnBall();
            }
        }
    }

    IEnumerator RandomBehavior()
    {
        if (!isIdle && !isWalking) yield break;

        while (enableRandomWalk && !isFetching && !isCalled && !isReturningToInitial)
        {
            if (isIdle)
            {
                SetWalking(false);
                SetRunning(false);
                currentStateTime = Random.Range(minIdleTime, maxIdleTime);
                yield return new WaitForSeconds(currentStateTime);
                if (isFetching || isCalled || isReturningToInitial) break;
                isIdle = false;
                isWalking = true;
            }
            else if (isWalking)
            {
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection.y = 0;
                randomWalkTarget = initialPosition + randomDirection;
                randomWalkTarget.y = transform.position.y;
                SetWalking(true);
                SetRunning(false);
                currentStateTime = Random.Range(minWalkTime, maxWalkTime);
                stateTimer = 0f;
                while (stateTimer < currentStateTime)
                {
                    if (isFetching || isCalled || isReturningToInitial) break;
                    MoveTowards(randomWalkTarget, walkSpeed);
                    if (Vector3.Distance(transform.position, randomWalkTarget) < 0.3f) break;
                    stateTimer += Time.deltaTime;
                    yield return null;
                }
                if (isFetching || isCalled || isReturningToInitial) break;
                isWalking = false;
                isIdle = true;
            }
        }
    }

    public void CallDog()
    {
        if (!isFetching && !isCalled && !isReturningToInitial)
        {
            StopCoroutine(RandomBehavior());
            isCalled = true;
            isWalking = false;
            isIdle = false;
            SetWalking(false);
            SetRunning(true);
        }
    }

    public void StartFetching(Vector3 ballPosition)
    {
        if (!isFetching && !isInIdleCooldown)
        {
            StopAllCoroutines();

            // 开始追踪球的位置
            ballTrackingCoroutine = StartCoroutine(TrackBallPosition());

            // 设置状态
            isFetching = true;
            isCalled = false;
            isReturningToInitial = false;
            isWalking = false;
            isIdle = false;
            targetPosition = ballPosition;

            SetWalking(false);
            SetRunning(true);
        }
    }

    // 追踪球位置的协程
    private IEnumerator TrackBallPosition()
    {
        while (isFetching && !hasBall && ball != null && ball.activeSelf)
        {
            // 更新目标为球的当前位置
            targetPosition = ball.transform.position;

            // 等待一段时间再更新
            yield return new WaitForSeconds(ballTrackingUpdateRate);
        }
    }

    void SetWalking(bool walking)
    {
        if (animator != null)
        {
            animator.SetBool(isWalkingParam, walking);
        }
    }

    void SetRunning(bool running)
    {
        if (animator != null)
        {
            animator.SetBool(isRunningParam, running);
        }
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    void PickUpBall()
    {
        // 停止追踪球位置
        if (ballTrackingCoroutine != null)
            StopCoroutine(ballTrackingCoroutine);
        

        ball.SetActive(false);
        hasBall = true;
        targetPosition = player.transform.position;
    }

    void ReturnBall()
    {
        Vector3 platePosition = plate.transform.position;
        Renderer plateRenderer = plate.GetComponent<Renderer>();

        if (plateRenderer != null)
        {
            float plateTopY = platePosition.y;
            Collider plateCollider = plate.GetComponent<Collider>();
            if (plateCollider != null)
            {
                plateTopY = plateCollider.bounds.center.y + plateCollider.bounds.extents.y;
            }
            else
            {
                plateTopY = plateRenderer.bounds.center.y + plateRenderer.bounds.extents.y;
            }
            ball.transform.position = new Vector3(platePosition.x, plateTopY + plateHeightOffset, platePosition.z);
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
                ballRb.AddForce(Vector3.down * 0.5f, ForceMode.Impulse);
            }
        }
        else
        {
            ball.transform.position = new Vector3(platePosition.x, platePosition.y + plateHeightOffset, platePosition.z);
        }

        ball.SetActive(true);
        BallController ballController = ball.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.ResetBall();
        }
        hasBall = false;
        isFetching = false;
        SetRunning(false);
        SetWalking(false);
        StartCoroutine(IdleCooldown());
    }

    private IEnumerator IdleCooldown()
    {
        isInIdleCooldown = true;
        SetRunning(false);
        SetWalking(false);
        yield return new WaitForSeconds(idleTime);
        isInIdleCooldown = false;
        isIdle = true;
        if (enableRandomWalk && !isFetching && !isCalled && !isReturningToInitial)
        {
            StartCoroutine(RandomBehavior());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 空方法避免不必要的碰撞处理
    }
}