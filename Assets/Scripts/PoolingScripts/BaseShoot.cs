using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShoot : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    public float maxLife = 5.0f;
    public float timer;

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        timer += Time.deltaTime;
        if(timer>=maxLife)
        {
            BasePool.Instance.ReturnToPoll(this);
        }
    }
}
