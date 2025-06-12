using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BastPoolTest : MonoBehaviour
{
    public float speed = 30f;
    private float timer;
    public float maxTimeLife = 3.5f;

    private void OnEnable()
    {
        timer = 0;
    }

    private void Update()
    {
        transform.Translate(transform.up * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if(timer>=maxTimeLife)
        {
            Destroy(this.gameObject);
        }
    }
}
