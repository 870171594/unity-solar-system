using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicTurn : MonoBehaviour
{
    // Start is called before the first frame update
    public float PublicRateSpeed;
    public Transform target;
    public float Rate=1f;
    public void OnValueChanged(float rate)
    {
        Rate = rate;
    }
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(target.position, new Vector3(0, 1, 0), PublicRateSpeed * Rate * Time.deltaTime);
    }
}
