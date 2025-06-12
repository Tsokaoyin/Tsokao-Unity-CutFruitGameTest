using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 1.�˽ű���Ҫ�����ӵ������ϡ�
/// 2.���ű�ֻ�����ӵ����ƶ��ٶȡ�����ʱ�䣨�������ڣ�
/// </summary>
public class NoPool : MonoBehaviour
{
    [Tooltip("�ӵ��ƶ��ٶ�")]
    public float moveSpeed = 5;
    [Tooltip("�ӵ��������")]
    public float maxLife = 5.0f;

    [Tooltip("��ʱ��")]
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
