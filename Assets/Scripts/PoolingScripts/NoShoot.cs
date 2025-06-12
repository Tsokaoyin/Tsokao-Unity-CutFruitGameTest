using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoShoot : MonoBehaviour
{
    [Tooltip("×Óµ¯¶ÔÏó¹ÒÔØ")]
    public NoPool shootPrefabs;

    public float fireTime=2.0f;
    public float refireTime=0;

    private void Update()
    {
        refireTime += Time.deltaTime;
        if(refireTime>=fireTime)
        {
            FireShoot();
            refireTime = 0;
        }
    }

    void FireShoot()
    {
       var shoot= Instantiate(shootPrefabs, transform.position, transform.rotation);
        shoot.transform.SetParent(this.transform);
    }
}
