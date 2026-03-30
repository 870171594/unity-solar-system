using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySetting : MonoBehaviour
{   public int[] QualityLevels = { 2, 1, 0 };
    public void Awake()
    {
        QualitySettings.SetQualityLevel(QualityLevels[0]);
    }
    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(QualityLevels[level]);
        Debug.Log("Quality level set to: " + QualityLevels[level]);
    }
}
