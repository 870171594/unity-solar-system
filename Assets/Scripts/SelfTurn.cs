using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfTurn : MonoBehaviour
{
    // Start is called before the first frame update
    public float SelfRateSpeed;
    public float Rate=1f;
    void Start()
    {
        
    }
    public void OnValueChanged(float rate)
    {
        Rate = rate;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, SelfRateSpeed * Rate * Time.deltaTime);
    }
}
