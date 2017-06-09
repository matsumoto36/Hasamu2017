using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubeidou : MonoBehaviour {
    private bool m_isVisibleTimer = false;
    void Start()
    {
        Invoke("DelayMethod", 3.0f);
    }
    void FixedUpdate()
    {
        if (m_isVisibleTimer)
        {
            float x = Input.GetAxis("Horizontal");

            Rigidbody rigidbody = GetComponent<Rigidbody>();

            rigidbody.AddForce(x * 10, 0, 0);
        }
    }
    public void DelayMethod()
    {
        m_isVisibleTimer = true;
    }
}
