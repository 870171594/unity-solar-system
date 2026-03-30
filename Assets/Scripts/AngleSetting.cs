using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 摄像机角度设置控制器
/// 绑定到Slider上，用于控制摄像机俯仰角
/// </summary>
public class AngleSetting : MonoBehaviour
{
    [Header("组件引用")]
    [Tooltip("摄像机控制脚本")]
    public CameraControl cameraControl;

    [Tooltip("关联的Slider组件（可选，会自动获取）")]
    public Slider angleSlider;

    [Header("角度设置")]
    [Tooltip("重置角度值")]
    public float resetAngle = 50f;

    private void Start()
    {
        // 如果没有手动分配Slider，尝试获取当前物体上的Slider组件
        if (angleSlider == null)
        {
            angleSlider = GetComponent<Slider>();
        }

        // 如果没有手动分配摄像机，尝试自动查找
        if (cameraControl == null)
        {
            cameraControl = Camera.main?.GetComponent<CameraControl>();

            if (cameraControl == null)
            {
                Debug.LogWarning("未找到 CameraControl 脚本，请在Inspector中手动分配。");
            }
        }

        // 如果有Slider，设置初始值
        if (angleSlider != null)
        {
            angleSlider.value = resetAngle;
        }
    }

    /// <summary>
    /// 设置摄像机俯仰角（用于UI Slider的OnValueChanged事件）
    /// </summary>
    /// <param name="angle">俯仰角度数</param>
    public void SetPitchAngle(float angle)
    {
        if (cameraControl != null)
        {
            cameraControl.SetPitchAngle(angle);
        }
        else
        {
            Debug.LogWarning("CameraControl 未设置，请在Inspector中分配摄像机控制脚本。");
        }
    }

    /// <summary>
    /// 重置角度到默认值（用于UI Button的OnClick事件）
    /// </summary>
    public void ResetAngle()
    {
        // 重置Slider的值
        if (angleSlider != null)
        {
            angleSlider.value = resetAngle;
        }

        // 同时也设置摄像机角度
        SetPitchAngle(resetAngle);
    }
}
