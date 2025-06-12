using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 1.此脚本需要绑定在子弹对象上。
/// 2.本脚本只处理子弹的移动速度、存在时间（生命周期）
/// </summary>
public class NoPool : MonoBehaviour
{
    [Tooltip("子弹移动速度")]
    public float moveSpeed = 5;
    [Tooltip("子弹最大生命")]
    public float maxLife = 5.0f;

    [Tooltip("计时器")]
    private float timer;

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        transform.Translate(Vector3 .forward * moveSpeed * Time.deltaTime);
        timer += Time.deltaTime;
        if(timer>maxLife)
        {
            Destroy(gameObject);
        }

    }
}
