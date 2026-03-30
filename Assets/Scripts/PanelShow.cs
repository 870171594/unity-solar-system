using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelShow : MonoBehaviour
{   
    public GameObject Panel; // 需要在Unity编辑器中将面板对象拖到这个变量上
    public void TogglePanelVisibility()
    {
        // 切换面板的活动状态
        Panel.SetActive(!Panel.activeSelf);

    }
    public void Awake()
    {
        // 确保面板在游戏开始时是隐藏的
        Panel.SetActive(false);
    }
}
