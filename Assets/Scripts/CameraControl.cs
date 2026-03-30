using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 策略游戏风格摄像机控制器
/// 类似《无尽的拉格朗日》的操作方式
/// - 单指拖拽：水平平移
/// - 双指缩放：调整高度/视野
/// - 仰角固定：保持俯视角度
/// </summary>
public class CameraControl : MonoBehaviour
{
    [Header("视角设置")]
    [Tooltip("固定俯仰角（度）")]
    [Range(30f, 80f)]
    public float fixedPitchAngle = 50f;

    [Header("平移设置")]
    [Tooltip("单指拖拽平移灵敏度")]
    public float panSensitivity = 0.5f;

    [Tooltip("平移平滑速度")]
    public float panSmoothSpeed = 10f;

    [Header("缩放设置")]
    [Tooltip("双指缩放灵敏度")]
    public float zoomSensitivity = 1f;

    [Tooltip("最小摄像机高度")]
    public float minHeight = 0.5f;

    [Tooltip("最大摄像机高度")]
    public float maxHeight = 20f;

    [Tooltip("缩放平滑速度")]
    public float zoomSmoothSpeed = 5f;

    [Header("边界限制（可选）")]
    [Tooltip("是否启用边界限制")]
    public bool enableBounds = false;

    [Tooltip("最小X坐标")]
    public float minBoundX = -100f;

    [Tooltip("最大X坐标")]
    public float maxBoundX = 100f;

    [Tooltip("最小Z坐标")]
    public float minBoundZ = -100f;

    [Tooltip("最大Z坐标")]
    public float maxBoundZ = 100f;

    [Header("初始设置")]
    [Tooltip("是否在Start时使用自定义初始值")]
    public bool useCustomInitialValues = true;

    [Tooltip("初始位置")]
    public Vector3 initialPosition = new Vector3(6f, 23f, -12f);

    [Tooltip("初始Y轴旋转角度（度）")]
    public float initialRotationY = -10f;

    [Tooltip("初始俯仰角（度）")]
    public float initialPitchAngle = 50f;

    [Tooltip("还原时是否平滑过渡")]
    public bool smoothReset = true;

    [Tooltip("平滑还原速度")]
    public float resetSmoothSpeed = 2f;

    [Header("星球跟踪")]
    [Tooltip("是否正在跟踪星球")]
    public bool isTracking = false;

    [Tooltip("当前跟踪的目标星球")]
    public Transform trackingTarget;

    [Tooltip("跟踪时是否自动朝向星球")]
    public bool trackingLookAtTarget = true;

    [Tooltip("跟踪时的Y轴偏移倍数（基于星球半径）")]
    public float trackingOffsetYMultiplier = 2.5f;

    [Tooltip("跟踪时的Z轴偏移倍数（基于星球半径）")]
    public float trackingOffsetZMultiplier = -3f;

    [Tooltip("默认星球半径（当物体没有Collider时使用）")]
    public float defaultPlanetRadius = 1f;

    [Tooltip("跟踪平滑速度")]
    public float trackingSmoothSpeed = 3f;

    [Tooltip("双击判定时间间隔（秒）")]
    public float doubleClickTime = 0.3f;

    // 内部变量
    private Vector3 targetPosition;
    private float targetHeight;
    private float previousTouchDistance = 0f;
    private Vector2 previousTouchPosition = Vector2.zero;
    private bool isDragging = false;
    private int dragFingerId = -1;
    private Coroutine resetCoroutine;

    // 双击检测相关
    private float lastClickTime = 0f;
    private Vector2 lastClickPosition = Vector2.zero;

    void Start()
    {
        // 初始化目标位置和高度
        targetPosition = transform.position;
        targetHeight = transform.position.y;

        // 设置初始俯仰角
        SetFixedPitchAngle();

        // 应用自定义初始值
        if (useCustomInitialValues)
        {
            ApplyInitialValues();
        }
    }

    /// <summary>
    /// 应用自定义初始值（位置和旋转）- 瞬间完成
    /// </summary>
    public void ApplyInitialValues()
    {
        // 停止跟踪
        StopTracking();

        // 重置俯仰角到初始值
        fixedPitchAngle = initialPitchAngle;

        // 设置位置
        transform.position = initialPosition;
        targetPosition = initialPosition;
        targetHeight = initialPosition.y;

        // 设置旋转
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = initialPitchAngle;
        rotation.y = initialRotationY;
        rotation.z = 0f;
        transform.rotation = Quaternion.Euler(rotation);
    }

    /// <summary>
    /// 重置摄像机到初始值
    /// </summary>
    public void ResetCamera()
    {
        // 停止之前的还原协程
        if (resetCoroutine != null)
        {
            StopCoroutine(resetCoroutine);
        }

        if (smoothReset)
        {
            resetCoroutine = StartCoroutine(SmoothResetCoroutine());
        }
        else
        {
            ApplyInitialValues();
        }
    }

    /// <summary>
    /// 平滑还原协程
    /// </summary>
    private IEnumerator SmoothResetCoroutine()
    {
        // 停止跟踪
        StopTracking();

        Vector3 startPosition = transform.position;
        Vector3 endPosition = initialPosition;

        float startRotationY = transform.rotation.eulerAngles.y;
        float endRotationY = initialRotationY;

        float elapsedTime = 0f;
        float duration = 1f / resetSmoothSpeed;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // 使用平滑曲线让动画更自然
            t = Mathf.SmoothStep(0f, 1f, t);

            // 插值位置
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, t);
            currentPos.y = Mathf.Lerp(transform.position.y, endPosition.y, t);
            transform.position = currentPos;
            targetPosition = currentPos;
            targetHeight = endPosition.y;

            // 插值旋转
            float currentRotationY = Mathf.LerpAngle(startRotationY, endRotationY, t);
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.x = fixedPitchAngle;
            rotation.y = currentRotationY;
            rotation.z = 0f;
            transform.rotation = Quaternion.Euler(rotation);

            yield return null;
        }

        // 确保最终值精确
        ApplyInitialValues();
        resetCoroutine = null;
    }

    void LateUpdate()
    {
        // 检测平台
        bool isMobile = Application.platform == RuntimePlatform.Android ||
                        Application.platform == RuntimePlatform.IPhonePlayer;

        if (isMobile)
        {
            // 移动端：处理触摸输入
            HandleTouchInput();
            // 检测双击选择星球
            CheckDoubleClickOnMobile();
        }
        else
        {
            // PC端：处理鼠标和键盘输入
            HandlePCInput();
            // 检测双击选择星球
            CheckDoubleClickOnPC();
        }

        // 更新跟踪状态
        UpdateTracking();

        // 平滑移动到目标位置
        SmoothMoveToTarget();

        // 确保俯仰角固定
        SetFixedPitchAngle();
    }

    /// <summary>
    /// 设置固定的俯仰角，跟踪时自动朝向星球
    /// </summary>
    private void SetFixedPitchAngle()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        float targetPitchAngle = fixedPitchAngle;
        float targetYawAngle = currentRotation.y;

        // 如果正在跟踪且启用了自动朝向
        if (isTracking && trackingTarget != null && trackingLookAtTarget)
        {
            // 计算朝向星球的角度
            Vector3 direction = trackingTarget.position - transform.position;
            targetYawAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        }

        // 平滑旋转到目标角度
        float rotationSpeed = 5f;
        float newYaw = Mathf.LerpAngle(currentRotation.y, targetYawAngle, rotationSpeed * Time.deltaTime);
        float newPitch = Mathf.LerpAngle(currentRotation.x, targetPitchAngle, rotationSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(newPitch, newYaw, 0f);
    }

    /// <summary>
    /// 检测触摸位置是否在UI元素上
    /// </summary>
    private bool IsTouchOverUI(Vector2 touchPosition)
    {
        // 使用EventSystem检测触摸位置是否有UI元素
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    /// <summary>
    /// 处理触摸输入
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            // 无触摸时重置状态
            isDragging = false;
            dragFingerId = -1;
            return;
        }

        // 单指触摸 - 平移
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 检测触摸是否在UI上，如果在UI上则不处理摄像机控制
                if (IsTouchOverUI(touch.position))
                {
                    isDragging = false;
                    dragFingerId = -1;
                    return;
                }

                isDragging = true;
                dragFingerId = touch.fingerId;
                previousTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging && touch.fingerId == dragFingerId)
            {
                HandleSingleTouchPan(touch);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (touch.fingerId == dragFingerId)
                {
                    isDragging = false;
                    dragFingerId = -1;
                }
            }
        }

        // 双指触摸 - 缩放
        if (Input.touchCount == 2)
        {
            HandlePinchZoom();
        }
    }

    /// <summary>
    /// 处理单指拖拽平移
    /// </summary>
    private void HandleSingleTouchPan(Touch touch)
    {
        Vector2 currentTouchPosition = touch.position;
        Vector2 deltaPosition = currentTouchPosition - previousTouchPosition;

        // 屏幕空间移动量（反转使拖拽更自然）
        float panX = -deltaPosition.x * panSensitivity * 0.1f;
        float panZ = -deltaPosition.y * panSensitivity * 0.1f;

        // 使用摄像机的本地方向向量进行转换
        Vector3 localMove = new Vector3(panX, 0, panZ);
        Vector3 worldMove = transform.TransformDirection(localMove);

        // 更新目标位置（保持高度不变）
        targetPosition.x += worldMove.x;
        targetPosition.z += worldMove.z;

        // 应用边界限制
        if (enableBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBoundX, maxBoundX);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBoundZ, maxBoundZ);
        }

        previousTouchPosition = currentTouchPosition;
    }

    /// <summary>
    /// 处理双指缩放
    /// 两指缩小（距离变小）= 上升（拉远视野）
    /// 两指放大（距离变大）= 下降（拉近视野）
    /// </summary>
    private void HandlePinchZoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        float currentDistance = Vector2.Distance(touch1.position, touch2.position);

        if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
        {
            previousTouchDistance = currentDistance;
        }
        else
        {
            float deltaDistance = currentDistance - previousTouchDistance;

            // 两指缩小（距离变小）= 上升（拉远视野）
            // 两指放大（距离变大）= 下降（拉近视野）
            targetHeight -= deltaDistance * zoomSensitivity * 0.1f;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);

            previousTouchDistance = currentDistance;
        }
    }

    /// <summary>
    /// 平滑移动到目标位置
    /// </summary>
    private void SmoothMoveToTarget()
    {
        // 平滑插值位置
        Vector3 currentPos = transform.position;
        Vector3 smoothPos = Vector3.Lerp(currentPos, targetPosition, Time.deltaTime * panSmoothSpeed);
        smoothPos.y = Mathf.Lerp(currentPos.y, targetHeight, Time.deltaTime * zoomSmoothSpeed);

        transform.position = smoothPos;
    }

    /// <summary>
    /// PC端输入处理（键盘和鼠标）
    /// </summary>
    private void HandlePCInput()
    {
        // WASD - 水平平移
        float panX = 0f;
        float panZ = 0f;

        if (Input.GetKey(KeyCode.W)) panZ += 1f;
        if (Input.GetKey(KeyCode.S)) panZ -= 1f;
        if (Input.GetKey(KeyCode.A)) panX -= 1f;
        if (Input.GetKey(KeyCode.D)) panX += 1f;

        if (Mathf.Abs(panX) > 0.01f || Mathf.Abs(panZ) > 0.01f)
        {
            // 使用摄像机的本地方向向量进行转换
            Vector3 localMove = new Vector3(panX, 0, panZ);
            Vector3 worldMove = transform.TransformDirection(localMove);

            targetPosition.x += worldMove.x * Time.deltaTime * moveSpeed * 10f;
            targetPosition.z += worldMove.z * Time.deltaTime * moveSpeed * 10f;
        }

        // Q/E - 升降高度
        if (Input.GetKey(KeyCode.Q))
        {
            targetHeight += Time.deltaTime * moveSpeed * 5f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            targetHeight -= Time.deltaTime * moveSpeed * 5f;
        }
        targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);

        // 鼠标滚轮 - 缩放
        float scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            targetHeight -= scrollDelta * zoomSensitivity * 2f;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        }

        // 鼠标右键拖拽 - 旋转摄像机Y轴
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            if (Mathf.Abs(mouseX) > 0.01f)
            {
                transform.Rotate(0f, mouseX * 5f, 0f, Space.World);
            }
        }

        // 鼠标中键拖拽 - 平移画面（类似移动端单指拖拽）
        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (Mathf.Abs(mouseX) > 0.001f || Mathf.Abs(mouseY) > 0.001f)
            {
                Vector3 localMove = new Vector3(-mouseX, 0, -mouseY) * panSensitivity * 10f;
                Vector3 worldMove = transform.TransformDirection(localMove);

                targetPosition.x += worldMove.x * Time.deltaTime;
                targetPosition.z += worldMove.z * Time.deltaTime;
            }
        }
    }

    // PC端移动速度
    private float moveSpeed = 5f;

    /// <summary>
    /// 设置摄像机到指定位置（用于脚本控制）
    /// </summary>
    public void SetCameraPosition(Vector3 position)
    {
        targetPosition = position;
        targetHeight = position.y;
        targetPosition.y = transform.position.y; // 保持当前高度
    }

    /// <summary>
    /// 设置摄像机高度
    /// </summary>
    public void SetCameraHeight(float height)
    {
        targetHeight = Mathf.Clamp(height, minHeight, maxHeight);
    }

    /// <summary>
    /// 设置摄像机俯仰角（用于UI Slider）
    /// </summary>
    /// <param name="angle">俯仰角度数（建议范围30-80度）</param>
    public void SetPitchAngle(float angle)
    {
        fixedPitchAngle = Mathf.Clamp(angle, 0f, 90f);
        SetFixedPitchAngle();
    }

    /// <summary>
    /// PC端双击检测
    /// </summary>
    private void CheckDoubleClickOnPC()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 检测是否点击在UI上
            if (IsTouchOverUI(Input.mousePosition))
            {
                return;
            }

            float currentTime = Time.time;
            float timeSinceLastClick = currentTime - lastClickTime;

            if (timeSinceLastClick <= doubleClickTime)
            {
                // 双击成功，尝试选择星球
                TrySelectTarget(Input.mousePosition);
            }

            lastClickTime = currentTime;
            lastClickPosition = Input.mousePosition;
        }
    }

    /// <summary>
    /// 移动端双击检测
    /// </summary>
    private void CheckDoubleClickOnMobile()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 检测是否点击在UI上
                if (IsTouchOverUI(touch.position))
                {
                    return;
                }

                float currentTime = Time.time;
                float timeSinceLastClick = currentTime - lastClickTime;

                // 检查是否为双击（时间间隔和位置都要符合）
                float positionDelta = Vector2.Distance(touch.position, lastClickPosition);
                if (timeSinceLastClick <= doubleClickTime && positionDelta < 50f)
                {
                    // 双击成功，尝试选择星球
                    TrySelectTarget(touch.position);
                }

                lastClickTime = currentTime;
                lastClickPosition = touch.position;
            }
        }
    }

    /// <summary>
    /// 尝试选择跟踪目标（使用射线检测）
    /// </summary>
    private void TrySelectTarget(Vector2 screenPosition)
    {
        // 从摄像机发射射线
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 获取被点击的物体
            Transform hitObject = hit.transform;

            // 检查是否点击了星球（可以根据需要添加更多条件）
            // 例如：检查物体是否有特定的标签或组件
            if (hitObject != null)
            {
                StartTracking(hitObject);
            }
        }
    }

    /// <summary>
    /// 开始跟踪目标
    /// </summary>
    public void StartTracking(Transform target)
    {
        trackingTarget = target;
        isTracking = true;

        // 设置目标位置为星球上方
        if (target != null)
        {
            // 获取星球半径
            float planetRadius = GetPlanetRadius(target);

            // 根据星球大小动态计算偏移量
            Vector3 targetPos = target.position;
            targetPos.y += planetRadius * trackingOffsetYMultiplier;
            targetPos.z += planetRadius * trackingOffsetZMultiplier;
            targetPosition = targetPos;
            targetHeight = targetPos.y;
        }
    }

    /// <summary>
    /// 获取星球的半径
    /// </summary>
    private float GetPlanetRadius(Transform target)
    {
        // 尝试获取SphereCollider
        SphereCollider sphereCollider = target.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            // 考虑物体的缩放
            float maxScale = Mathf.Max(target.localScale.x, Mathf.Max(target.localScale.y, target.localScale.z));
            return sphereCollider.radius * maxScale;
        }

        // 尝试获取BoxCollider
        BoxCollider boxCollider = target.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            // 使用最大的边的一半作为半径
            Vector3 size = Vector3.Scale(boxCollider.size, target.localScale);
            return Mathf.Max(size.x, Mathf.Max(size.y, size.z)) * 0.5f;
        }

        // 如果没有Collider，使用默认值
        return defaultPlanetRadius;
    }

    /// <summary>
    /// 停止跟踪
    /// </summary>
    public void StopTracking()
    {
        isTracking = false;
        trackingTarget = null;
    }

    /// <summary>
    /// 更新跟踪状态
    /// </summary>
    private void UpdateTracking()
    {
        if (isTracking && trackingTarget != null)
        {
            // 获取星球半径
            float planetRadius = GetPlanetRadius(trackingTarget);

            // 持续更新目标位置为星球上方（根据星球大小动态计算）
            Vector3 targetPos = trackingTarget.position;
            targetPos.y += planetRadius * trackingOffsetYMultiplier;
            targetPos.z += planetRadius * trackingOffsetZMultiplier;

            // 使用平滑移动
            float smoothFactor = trackingSmoothSpeed * Time.deltaTime;
            targetPosition = Vector3.Lerp(targetPosition, targetPos, smoothFactor);
            targetHeight = Mathf.Lerp(targetHeight, targetPos.y, smoothFactor);
        }
        else if (isTracking && trackingTarget == null)
        {
            // 目标不存在时停止跟踪
            isTracking = false;
        }
    }

    /// <summary>
    /// 获取当前摄像机注视点（地面投影点）
    /// </summary>
    public Vector3 GetLookAtPoint()
    {
        return new Vector3(transform.position.x, 0f, transform.position.z);
    }
}
