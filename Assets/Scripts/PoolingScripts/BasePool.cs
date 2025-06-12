using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePool : MonoBehaviour
{
    [SerializeField]
    private BaseShoot shootPrefabs;

    private Queue<BaseShoot> baseShoots = new Queue<BaseShoot>();

    public static BasePool Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Get();
    }

    public BaseShoot Get()
    {
        if(baseShoots.Count==0)
        {
            AddShoots(10);
        }

        return baseShoots.Dequeue();
    }

    public void AddShoots(int count)
    {
        for(int i=0;i<count;i++)
        {
            BaseShoot baseShoot = Instantiate(shootPrefabs);
            baseShoot.transform.SetParent(this.gameObject.transform);
            baseShoot.gameObject.SetActive(false);
            baseShoots.Enqueue(baseShoot);
        }
    }

    public void ReturnToPoll(BaseShoot shoot)
    {
        shoot.gameObject.SetActive(false);
        baseShoots.Enqueue(shoot);
    }

}
