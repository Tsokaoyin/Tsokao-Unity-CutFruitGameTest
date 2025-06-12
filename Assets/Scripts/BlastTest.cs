using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastTest : MonoBehaviour
{
    [SerializeField]
    private BastPoolTest shootPreFabs;

    public float fireTime=0;
    public float refireTime=2f;

    
    private void Update()
    {
        fireTime += Time.deltaTime;
        if(fireTime>=refireTime)
        {
            fireTime = 0;
            BlastFire();
        }
    }

    public void BlastFire()
    {
        var shot = Instantiate(shootPreFabs, transform.position, transform.rotation);
    }
}
