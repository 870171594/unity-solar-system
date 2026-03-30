using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{

    public void Quit()
    {
        #if UNITY_EDITOR
        // 编辑器模式下停止播放
            UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 发布版本中退出应用程序
            Application.Quit();
        #endif
    }
}
