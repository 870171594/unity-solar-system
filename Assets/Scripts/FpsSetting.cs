using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsSetting : MonoBehaviour
{
    public  int[] LimitFPS = { 30, 15, 60};
    public int limitFPS = 30;
    public void OnValueChanged(int value)
    {
        limitFPS = LimitFPS[value];
        Debug.Log("FPS Limit set to: " + limitFPS);
        Application.targetFrameRate = limitFPS;
    }
    
    private void Awake()
    {
        Application.targetFrameRate = limitFPS;
    }
}   